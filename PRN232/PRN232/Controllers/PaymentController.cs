using BusinessObject.Dtos.paymentDto.Request;
using BusinessObject.Dtos.paymentDto.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using Service.Interface;
using System.Security.Claims;

namespace PRN232.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Create a VIP payment link (200,000 VND)
        /// </summary>
        [HttpPost("create-coin-payment")]
        [Authorize]
        public async Task<ActionResult<CreatePaymentResponse>> CreateCoinPayment([FromBody] CreatePaymentRequest request)
        {
            try
            {
                // Get user ID from JWT token using ClaimTypes
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }


                if (request.PackageId <= 0)
                {
                    return BadRequest(new { message = "Invalid PackageId" });
                }
                var result = await _paymentService.CreateCoinPayment(userId, request.PackageId, request.Description);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Coin payment");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get payment status by order code
        /// </summary>
        [HttpGet("status/{orderCode}")]
        [Authorize]
        public async Task<ActionResult<PaymentStatusResponse>> GetPaymentStatus(long orderCode)
        {
            try
            {
                var result = await _paymentService.GetPaymentStatus(orderCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment status for OrderCode={OrderCode}", orderCode);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get user's payment history
        /// </summary>
        [HttpGet("history")]
        [Authorize]
        public async Task<ActionResult<List<PaymentStatusResponse>>> GetPaymentHistory()
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                int userId = int.Parse(userIdClaim);
                var result = await _paymentService.GetUserPaymentHistory(userId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment history");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Sync and check all pending payments for current user (will auto-upgrade to VIP if payment is successful)
        /// </summary>
        [HttpPost("sync-pending")]
        [Authorize]
        public async Task<ActionResult> SyncPendingPayments()
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                int userId = int.Parse(userIdClaim);
                var payments = await _paymentService.GetUserPaymentHistory(userId);

                int upgraded = 0;
                foreach (var payment in payments.Where(p => p.Status == "PENDING"))
                {
                    try
                    {
                        // Check status from PayOS and auto-upgrade if PAID
                        var updated = await _paymentService.GetPaymentStatus(payment.OrderCode);
                        if (updated.Status == "PAID")
                        {
                            upgraded++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Could not sync payment {OrderCode}", payment.OrderCode);
                    }
                }

                return Ok(new
                {
                    message = $"Synced pending payments. {upgraded} payment(s) confirmed.",
                    upgraded = upgraded
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing pending payments");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Manually upgrade to VIP if user has a valid PAID payment within last 30 days
        /// </summary>
        [HttpPost("upgrade-to-vip")]
        [Authorize]
        public async Task<ActionResult> UpgradeToVip()
        {
            try
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                int userId = int.Parse(userIdClaim);

                bool success = await _paymentService.UpgradeUserIfPaymentExists(userId);

                if (success)
                {
                    return Ok(new
                    {
                        message = "Successfully upgraded to VIP!",
                        success = true
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        message = "No valid payment found. Please complete a payment first.",
                        success = false
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error upgrading user to VIP");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Webhook endpoint for PayOS payment notifications
        /// </summary>
        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentWebhook([FromBody] WebhookType webhookBody)
        {
            try
            {
                _logger.LogInformation("Received webhook from PayOS");

                bool success = await _paymentService.HandlePaymentWebhook(webhookBody);

                if (success)
                {
                    return Ok(new { message = "Webhook processed successfully" });
                }
                else
                {
                    return BadRequest(new { message = "Invalid webhook data" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing webhook");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Payment success page redirect handler
        /// </summary>
        [HttpGet("success")]
        [AllowAnonymous]
        public IActionResult PaymentSuccess([FromQuery] long orderCode, [FromQuery] string status)
        {
            _logger.LogInformation("Payment success redirect: OrderCode={OrderCode}, Status={Status}", orderCode, status);

            // You can redirect to your frontend success page
            return Ok(new
            {
                message = "Payment successful! You have been upgraded to VIP.",
                orderCode = orderCode,
                status = status
            });
        }

        /// <summary>
        /// Payment cancel page redirect handler
        /// </summary>
        [HttpGet("cancel")]
        [AllowAnonymous]
        public IActionResult PaymentCancel([FromQuery] long orderCode)
        {
            _logger.LogInformation("Payment cancelled: OrderCode={OrderCode}", orderCode);

            // You can redirect to your frontend cancel page
            return Ok(new
            {
                message = "Payment was cancelled.",
                orderCode = orderCode
            });
        }
    }
}

