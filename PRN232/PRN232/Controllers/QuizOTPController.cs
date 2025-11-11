using BusinessObject.Dtos.Quiz;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.QuizzInterface;
using System.Security.Claims;

namespace PRN232.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuizOTPController : ControllerBase
    {
        private readonly IQuizOTPService _otpService;
        private readonly ILogger<QuizOTPController> _logger;

        public QuizOTPController(IQuizOTPService otpService, ILogger<QuizOTPController> logger)
        {
            _otpService = otpService;
            _logger = logger;
        }

        // ===== TEACHER ENDPOINTS =====

        /// <summary>
        /// Generate OTP for a quiz (Teacher only)
        /// </summary>
        [HttpPost("generate")]
        [Authorize(Roles = "Teacher")]
        [ProducesResponseType(typeof(QuizOTPDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GenerateOTP([FromBody] GenerateOTPDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid data",
                        errors = ModelState
                    });
                }

                var teacherId = GetCurrentUserId();
                var result = await _otpService.GenerateOTPAsync(dto.QuizId, teacherId, dto);

                _logger.LogInformation($"Teacher {teacherId} generated OTP for quiz {dto.QuizId}");

                return Ok(new
                {
                    success = true,
                    data = result,
                    message = $"OTP generated successfully: {result.OTPCode}"
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while generating OTP");
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt");
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating OTP");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while generating OTP"
                });
            }
        }

        /// <summary>
        /// Get all OTPs created by the teacher
        /// </summary>
        [HttpGet("my-otps")]
        [Authorize(Roles = "Teacher")]
        [ProducesResponseType(typeof(IEnumerable<QuizOTPDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyOTPs()
        {
            try
            {
                var teacherId = GetCurrentUserId();
                var otps = await _otpService.GetTeacherOTPsAsync(teacherId);

                return Ok(new
                {
                    success = true,
                    data = otps,
                    count = otps.Count(),
                    message = "OTPs retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving teacher OTPs");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving OTPs"
                });
            }
        }

        /// <summary>
        /// Get OTPs for a specific quiz
        /// </summary>
        [HttpGet("quiz/{quizId}")]
        [Authorize(Roles = "Teacher")]
        [ProducesResponseType(typeof(IEnumerable<QuizOTPDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetQuizOTPs(int quizId)
        {
            try
            {
                var teacherId = GetCurrentUserId();
                var otps = await _otpService.GetQuizOTPsAsync(quizId, teacherId);

                return Ok(new
                {
                    success = true,
                    data = otps,
                    count = otps.Count(),
                    message = $"OTPs for quiz {quizId} retrieved successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving OTPs for quiz {quizId}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving quiz OTPs"
                });
            }
        }

        /// <summary>
        /// Revoke/Deactivate an OTP
        /// </summary>
        [HttpDelete("{otpId}")]
        [Authorize(Roles = "Teacher")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RevokeOTP(int otpId)
        {
            try
            {
                var teacherId = GetCurrentUserId();
                var success = await _otpService.RevokeOTPAsync(otpId, teacherId);

                if (!success)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "OTP not found"
                    });
                }

                _logger.LogInformation($"Teacher {teacherId} revoked OTP {otpId}");

                return Ok(new
                {
                    success = true,
                    message = "OTP revoked successfully"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error revoking OTP {otpId}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while revoking OTP"
                });
            }
        }

        /// <summary>
        /// Extend OTP expiry time
        /// </summary>
        [HttpPatch("{otpId}/extend")]
        [Authorize(Roles = "Teacher")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ExtendOTPExpiry(int otpId, [FromBody] ExtendOTPDto dto)
        {
            try
            {
                var teacherId = GetCurrentUserId();
                var success = await _otpService.ExtendOTPExpiryAsync(otpId, teacherId, dto.AdditionalMinutes);

                if (!success)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "OTP not found"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = $"OTP expiry extended by {dto.AdditionalMinutes} minutes"
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error extending OTP {otpId}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while extending OTP"
                });
            }
        }

        /// <summary>
        /// Regenerate OTP with same settings
        /// </summary>
        [HttpPost("{otpId}/regenerate")]
        [Authorize(Roles = "Teacher")]
        [ProducesResponseType(typeof(QuizOTPDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> RegenerateOTP(int otpId)
        {
            try
            {
                var teacherId = GetCurrentUserId();
                var newOTP = await _otpService.RegenerateOTPAsync(otpId, teacherId);

                _logger.LogInformation($"Teacher {teacherId} regenerated OTP {otpId}");

                return Ok(new
                {
                    success = true,
                    data = newOTP,
                    message = $"New OTP generated: {newOTP.OTPCode}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error regenerating OTP {otpId}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while regenerating OTP"
                });
            }
        }

       

        // ===== STUDENT ENDPOINTS =====

        /// <summary>
        /// Validate OTP and get quiz (Student only)
        /// </summary>
        [HttpPost("validate")]
       // [Authorize(Roles = "Student")]
        [ProducesResponseType(typeof(OTPValidationResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ValidateOTP([FromBody] ValidateOTPDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid data",
                        errors = ModelState
                    });
                }

                var studentId = GetCurrentUserId();

                var result = await _otpService.ValidateOTPAsync(dto.OTPCode, studentId);

                if (!result.IsValid)
                {
                    _logger.LogWarning($"Student {studentId} failed OTP validation: {dto.OTPCode}");
                    return BadRequest(new
                    {
                        success = false,
                        message = result.Message
                    });
                }

                _logger.LogInformation($"Student {studentId} validated OTP: {dto.OTPCode}");

                return Ok(new
                {
                    success = true,
                    data = result,
                    message = "OTP validated successfully. You can now take the quiz."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating OTP");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while validating OTP"
                });
            }
        }

        /// <summary>
        /// Get quiz by OTP for taking (Student only)
        /// </summary>
        [HttpGet("take/{otpCode}")]
       // [Authorize(Roles = "Student")]
        [ProducesResponseType(typeof(QuizDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetQuizByOTP(string otpCode)
        {
            try
            {
                var studentId = GetCurrentUserId();
                var quiz = await _otpService.GetQuizByOTPAsync(otpCode, studentId);

                return Ok(new
                {
                    success = true,
                    data = quiz,
                    message = "Quiz retrieved successfully"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting quiz with OTP {otpCode}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while retrieving quiz"
                });
            }
        }

        // ===== ADMIN ENDPOINTS (Optional) =====

        /// <summary>
        /// Cleanup expired OTPs (Admin/System job)
        /// </summary>
        [HttpPost("cleanup")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CleanupExpiredOTPs()
        {
            try
            {
                var count = await _otpService.CleanupExpiredOTPsAsync();

                _logger.LogInformation($"Cleaned up {count} expired OTPs");

                return Ok(new
                {
                    success = true,
                    message = $"Cleaned up {count} expired OTPs"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up expired OTPs");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred during cleanup"
                });
            }
        }

        // Helper method
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }

            if (!int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("Invalid User ID format");
            }

            return userId;
        }
    }

    // Additional DTOs
    public class ExtendOTPDto
    {
        public int AdditionalMinutes { get; set; } = 30;
    }
}