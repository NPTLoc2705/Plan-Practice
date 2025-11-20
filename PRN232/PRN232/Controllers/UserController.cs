using BusinessObject.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using Service.Method;
using System.Security.Claims;

namespace PRN232.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICoinService _coinService;
        public UserController(IUserService userService, ICoinService coinService)
        {
            _userService = userService;
            _coinService = coinService;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID in token.");
            }
            return userId;
        }

        [HttpGet("profile")]
        [Authorize]
        public IActionResult GetProfile()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Invalid user identity" });
                }

                return Ok(new
                {
                    userId = userId,
                    username = User.FindFirst(ClaimTypes.Name)?.Value,
                    email = User.FindFirst(ClaimTypes.Email)?.Value,
                    role = User.FindFirst(ClaimTypes.Role)?.Value,
                    createdAt = User.FindFirst("Createdat")?.Value
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("profile")]
        [Authorize] // Assuming you have JWT authentication
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto updateDto)
        {
            try
            {
                // Get userId from JWT token claims
                var userIdClaim = User.FindFirst("userId")?.Value
                                  ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var updatedUser = await _userService.UpdateUserProfile(userId, updateDto);

                // Return user without password
                return Ok(new
                {
                    message = "Profile updated successfully",
                    user = new
                    {
                        updatedUser.Id,
                        updatedUser.Username,
                        updatedUser.Email,
                        updatedUser.Phone,
                        updatedUser.EmailVerified
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Fixed: Changed from absolute route to relative route
        [HttpPut("update-teacher")]
        public async Task<IActionResult> UpdateTeacherRole(string email)
        {
            try
            {
                var user = await _userService.UpdateTeacherRole(email);
                return Ok(new
                {
                    message = "Role updated to Teacher successfully",
                    user = new
                    {
                        user.Id,
                        user.Username,
                        user.Email,
                        user.Role
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("coin/balance")]
        [Authorize]
        public async Task<IActionResult> GetCoinBalance()
        {
            try
            {
                var userId = GetCurrentUserId();
                var balance = await _coinService.GetUserCoinBalance(userId);

                return Ok(new
                {
                    success = true,
                    coinBalance = balance
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Error getting coin balance" });
            }
        }
    }
}
