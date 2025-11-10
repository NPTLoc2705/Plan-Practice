using BusinessObject.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class PaymentDAO
    {
        private readonly PlantPraticeDbContext _context;
        private readonly ILogger<PaymentDAO> _logger;

        public PaymentDAO(PlantPraticeDbContext context, ILogger<PaymentDAO> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Payment> CreatePayment(int userId, long orderCode, int packageId ,int amount, string description)
        {
            var payment = new Payment
            {
                UserId = userId,
                OrderCode = orderCode,
                PackageId = packageId,
                Amount = amount,
                Description = description,
                Status = "PENDING",
                CreatedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Payment created: OrderCode={OrderCode}, UserId={UserId}, Amount={Amount}",
                orderCode, userId, amount);

            return payment;
        }

        public async Task<bool> AddCoinToUser(int userId, int coinAmount)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found for adding coins: UserId={UserId}", userId);
                    return false;
                }
                user.CoinBalance += coinAmount;
                await _context.SaveChangesAsync();
                _logger.LogInformation("Added {CoinAmount} coins to UserId={UserId}", coinAmount, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding coins to UserId={UserId}", userId);
                return false;
            }
        }

        public async Task<Payment> GetPaymentByOrderCode(long orderCode)
        {
            return await _context.Payments
                .Include(p => p.User)
                .Include(p => p.Package)
                .FirstOrDefaultAsync(p => p.OrderCode == orderCode);
        }

        public async Task<Payment> GetPaymentById(int id)
        {
            return await _context.Payments
                .Include(p => p.User)
                .Include(p => p.Package)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Payment>> GetUserPayments(int userId)
        {
            return await _context.Payments
                .Include(p => p.Package)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Payment> UpdatePaymentStatus(long orderCode, string status, string transactionCode = null, string paymentLinkId = null)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.OrderCode == orderCode);

            if (payment == null)
            {
                throw new Exception($"Payment not found for OrderCode: {orderCode}");
            }

            payment.Status = status;

            if (status == "PAID")
            {
                payment.PaidAt = DateTime.UtcNow;
                if(payment.PackageId != null)
                {
                    await AddCoinToUser(payment.UserId, payment.Package.CoinAmount);
                }
            }

            if (!string.IsNullOrEmpty(transactionCode))
            {
                payment.TransactionCode = transactionCode;
            }

            if (!string.IsNullOrEmpty(paymentLinkId))
            {
                payment.PaymentLinkId = paymentLinkId;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Payment status updated: OrderCode={OrderCode}, Status={Status}",
                orderCode, status);

            return payment;
        }

        //public async Task<bool> UpgradeUserToVip(int userId)
        //{
        //    try
        //    {
        //        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

        //        if (user == null)
        //        {
        //            _logger.LogWarning("User not found for upgrade: UserId={UserId}", userId);
        //            return false;
        //        }

        //        // RoleId = 2 is VIP based on your Role seeding
        //        user.RoleId = 2;
        //        await _context.SaveChangesAsync();

        //        _logger.LogInformation("User upgraded to VIP: UserId={UserId}", userId);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error upgrading user to VIP: UserId={UserId}", userId);
        //        return false;
        //    }
        //}
    }
}
