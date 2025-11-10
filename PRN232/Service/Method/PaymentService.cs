using BusinessObject.Dtos.paymentDto.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Net.payOS.Types;
using Net.payOS;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Service.Method
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepo _paymentRepo;
        private readonly IPackageRepo _packageRepo;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentService> _logger;
        private readonly PayOS _payOS;

        public PaymentService(IPaymentRepo paymentRepo, IPackageRepo packageRepo ,IConfiguration configuration, ILogger<PaymentService> logger)
        {
            _paymentRepo = paymentRepo;
            _packageRepo = packageRepo;
            _configuration = configuration;
            _logger = logger;

            // Initialize PayOS SDK
            string clientId = _configuration["PayOS:ClientId"];
            string apiKey = _configuration["PayOS:ApiKey"];
            string checksumKey = _configuration["PayOS:ChecksumKey"];

            _payOS = new PayOS(clientId, apiKey, checksumKey);
        }

        public async Task<CreatePaymentResponse> CreateCoinPayment(int userId, int packageId, string description)
        {
            try
            {
                // Get package details from database
                var package = await _packageRepo.FindPackageById(packageId);

                if (package == null)
                {
                    throw new Exception($"Package with ID {packageId} not found or is inactive");
                }

                string shortDescription = description;
                if (string.IsNullOrEmpty(shortDescription))
                {
                    // Create a concise default description
                    shortDescription = $"{package.Name} - {package.CoinAmount}c";
                }
                if (shortDescription.Length > 25)
                {
                    shortDescription = shortDescription.Substring(0, 25);
                }
                // Generate unique order code (timestamp based)
                long orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                // Create payment record in database first
                var payment = await _paymentRepo.CreatePayment(
                    userId: userId,
                    packageId: packageId,
                    orderCode: orderCode,
                    amount: package.Price,
                    description: shortDescription
                );

                // Create PayOS payment link
                var returnUrl = _configuration["PayOS:ReturnUrl"] ?? "http://localhost:4000/payment/success";
                var cancelUrl = _configuration["PayOS:CancelUrl"] ?? "http://localhost:4000/payment/cancel";
                var paymentData = new PaymentData(
                    orderCode: (int)orderCode,
                    amount: package.Price,
                    description:shortDescription,
                    items: new List<ItemData>
                    {
                new ItemData(package.Name, 1, package.Price)
                    },
                    cancelUrl: cancelUrl,
                    returnUrl: returnUrl
                );

                CreatePaymentResult createPaymentResult = await _payOS.createPaymentLink(paymentData);

                // Update payment with PayOS payment link ID
                await _paymentRepo.UpdatePaymentStatus(
                    orderCode,
                    "PENDING",
                    null,
                    createPaymentResult.paymentLinkId
                );

                _logger.LogInformation("Payment link created: OrderCode={OrderCode}, UserId={UserId}, Package={PackageName}, Amount={Amount}, CheckoutUrl={CheckoutUrl}",
                    orderCode, userId, package.Name, package.Price, createPaymentResult.checkoutUrl);

                return new CreatePaymentResponse
                {
                    CheckoutUrl = createPaymentResult.checkoutUrl,
                    OrderCode = orderCode,
                    PaymentLinkId = createPaymentResult.paymentLinkId,
                    Amount = package.Price,
                    Status = "PENDING"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment link for UserId={UserId}, PackageId={PackageId}", userId, packageId);
                throw new Exception("Failed to create payment link. Please try again later.");
            }
        }


      

        public async Task<PaymentStatusResponse> GetPaymentStatus(long orderCode)
        {
            try
            {
                var payment = await _paymentRepo.GetPaymentByOrderCode(orderCode);

                if (payment == null)
                {
                    throw new Exception($"Payment not found for OrderCode: {orderCode}");
                }

                // Optionally sync with PayOS
                try
                {
                    PaymentLinkInformation paymentInfo = await _payOS.getPaymentLinkInformation((int)orderCode);

                    // Update local status if different
                    if (paymentInfo.status != payment.Status)
                    {
                        string newStatus = paymentInfo.status;
                        await _paymentRepo.UpdatePaymentStatus(
                            orderCode,
                            newStatus,
                            paymentInfo.transactions?.FirstOrDefault()?.reference
                        );

                      

                        payment.Status = newStatus;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not sync payment status with PayOS for OrderCode={OrderCode}", orderCode);
                }

                return new PaymentStatusResponse
                {
                    OrderCode = payment.OrderCode,
                    Amount = payment.Amount,
                    Status = payment.Status,
                    Description = payment.Description,
                    CreatedAt = payment.CreatedAt,
                    PaidAt = payment.PaidAt,
                    TransactionCode = payment.TransactionCode
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment status for OrderCode={OrderCode}", orderCode);
                throw;
            }
        }

        public async Task<List<PaymentStatusResponse>> GetUserPaymentHistory(int userId)
        {
            try
            {
                var payments = await _paymentRepo.GetUserPayments(userId);

                return payments.Select(p => new PaymentStatusResponse
                {
                    OrderCode = p.OrderCode,
                    Amount = p.Amount,
                    Status = p.Status,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt,
                    PaidAt = p.PaidAt,
                    TransactionCode = p.TransactionCode
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment history for UserId={UserId}", userId);
                throw;
            }
        }

        public async Task<bool> HandlePaymentWebhook(WebhookType webhookBody)
        {
            try
            {
                // Verify webhook signature
                WebhookData verifiedData = _payOS.verifyPaymentWebhookData(webhookBody);

                if (verifiedData == null)
                {
                    _logger.LogWarning("Invalid webhook signature");
                    return false;
                }

                _logger.LogInformation("Webhook received: OrderCode={OrderCode}, Status={Status}",
                    verifiedData.orderCode, verifiedData.code);

                // Update payment status based on webhook
                string status = verifiedData.code == "00" ? "PAID" : "CANCELLED";

                var payment = await _paymentRepo.UpdatePaymentStatus(
                    verifiedData.orderCode,
                    status,
                    verifiedData.code
                );

                // Upgrade user to VIP if payment successful
                if (status == "PAID")
                {
                    _logger.LogInformation("Payment successful and coins added: OrderCode={OrderCode}, UserId={UserId}",
                    verifiedData.orderCode, payment.UserId);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling payment webhook");
                return false;
            }
        }

        public async Task<bool> UpgradeUserIfPaymentExists(int userId)
        {
            try
            {
                // Get all user's payments
                var payments = await _paymentRepo.GetUserPayments(userId);

                // Check if user has any PAID payment within last 30 days
                var hasPaidPayment = payments.Any(p =>
                    p.Status == "PAID" &&
                    p.PaidAt.HasValue &&
                    p.PaidAt.Value >= DateTime.UtcNow.AddDays(-30));

                if (hasPaidPayment)
                {
                    _logger.LogInformation("User has paid payments: UserId={UserId}", userId);
                    return true;
                }
                else
                {
                    _logger.LogWarning("No valid payment found for user upgrade: UserId={UserId}", userId);
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error upgrading user: UserId={UserId}", userId);
                return false;
            }
        }
    }
}
