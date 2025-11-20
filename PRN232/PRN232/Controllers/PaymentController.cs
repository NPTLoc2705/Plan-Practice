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
        [HttpPost]
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
                message = "You have been purchased successfully.",
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

