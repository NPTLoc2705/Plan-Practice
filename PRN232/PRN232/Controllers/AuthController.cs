using BusinessObject.Dtos.Auth;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interface;
using Service.Interface.Template;
using System.Security.Claims;

namespace PRN232.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IOptService _optService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> SendRegistrationOtp([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var message = await _optService.SendRegistrationOtpAsync(registerDto);

                return Ok(new
                {
                    message = message,
                    email = registerDto.Email
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("registration/verification")]
        public async Task<IActionResult> VerifyRegistration([FromBody] VerifyRegistrationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var message = await _optService.VerifyRegistrationAsync(request);

                return Ok(new
                {
                    message = message,
                    redirectTo = "/login" // Indicate to frontend to redirect to login
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("registration/otp/resend")]
        public async Task<IActionResult> ResendRegistrationOtp([FromBody] ResendOtpRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var message = await _optService.ResendRegistrationOtpAsync(request.Email);

                return Ok(new
                {
                    message = message,
                    email = request.Email
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleAuthDto googleAuthDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _userService.GoogleLoginAsync(googleAuthDto);
                return Ok(response);
            }
            catch (InvalidJwtException ex)
            {
                return Unauthorized(new { message = "Invalid Google token" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var (token, user) = await _userService.LoginAsync(loginDto);


                return Ok(new
                {
                    message = "Login successful",
                    token = token,
                    user = new
                    {
                        id = user.Id,
                        username = user.Username,
                        email = user.Email,
                        role = user.Role,
                        createdAt = user.Createdat
                    }
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        

    }
}
