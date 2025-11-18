using BCrypt.Net;
using BusinessObject;
using BusinessObject.Dtos.Auth;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Utilities.Collections;
using Repository.Interface;
using Service.Interface;
using Service.JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using static System.Net.WebRequestMethods;


namespace Service.Method
{
    // Business logic:
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IOtpVerifyRepository _otpRepository;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        public UserService(IUserRepository userRepository, IJwtService jwtService, IOtpVerifyRepository otpVerifyRepository, IEmailSender emailSender, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _otpRepository = otpVerifyRepository;
            _emailSender = emailSender;
            _configuration = configuration;
        }


        public async Task<(string token, User user)> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
                throw new Exception("Invalid credentials");
            if (user.IsBanned)
            {
                throw new Exception("Your account has been banned. Please contact support.");
            }
            var token = _jwtService.GenerateToken(user);
            return (token, user);
        }

        public async Task<string> SendRegistrationOtpAsync(RegisterDto registerDto)
        {
            // Check if email already exists
            var emailExists = await _userRepository.EmailExistsAsync(registerDto.Email);
            if (emailExists)
            {
                throw new Exception("Email already exists");
            }
            // Generate 6-digit OTP
            var otpCode = GenerateOtp();

            // Invalidate any previous registration OTPs for this email
            await _otpRepository.InvalidatePreviousOtpsAsync(registerDto.Email, OptPurpose.Register);

            // Create new OTP record
            var otpVerify = new OtpVerify
            {
                Email = registerDto.Email,
                Otp = otpCode,
                Purpose = OptPurpose.Register,
                CreatedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddMinutes(1), // 3 minutes expiry
                IsUsed = false
            };

            await _otpRepository.CreateAsync(otpVerify);
            var subject = "P&P System Login Verification Code";
            var body = $@"
<html>
  <body style='margin:0; padding:0; background-color:#f4f6f8; font-family:Arial, sans-serif;'>
    <table role='presentation' style='width:100%; border-collapse:collapse; background-color:#f4f6f8;'>
      <tr>
        <td align='center' style='padding:40px 0;'>
          <table role='presentation' style='width:100%; max-width:600px; background:#ffffff; border-radius:10px; padding:40px; text-align:center; box-shadow:0 4px 10px rgba(0,0,0,0.05);'>
            <tr>
              <td>

                <h2 style='color:#4a6cf7; margin-bottom:20px;'>Study Plan & Practice System</h2>

                <p style='font-size:16px; color:#333; margin-bottom:20px;'>
                  Hello,<br/><br/>
                  To continue setting up your account, please use the verification code below:
                </p>

                <p style='font-size:32px; font-weight:bold; letter-spacing:4px; color:#4a6cf7; margin:25px 0;'>
                  {otpCode}
                </p>

                <p style='font-size:14px; color:#555; margin-bottom:20px;'>
                  This code will expire in <strong>3 minutes</strong>.<br/>
                  For your security, please do not share it with anyone.
                </p>

                <hr style='border:none; border-top:1px solid #eee; margin:30px 0;' />

                <p style='font-size:14px; color:#888;'>
                  If you did not request this verification, you may ignore this email.
                </p>

                <p style='font-size:14px; color:#555; margin-top:20px;'>
                  Best regards,<br/>
                  <strong>Study Plan & Practice System Team</strong>
                </p>

              </td>
            </tr>
          </table>
        </td>
      </tr>
    </table>
  </body>
</html>";
            await _emailSender.SendEmailAsync(registerDto.Email, subject, body);
            // TODO: Send email with OTP code here
            // For now, we'll just return a message

            return $"OTP sent to {registerDto.Email}. Please check your email and verify within 3 minutes.";
        }
        public async Task<string> VerifyRegistrationAsync(VerifyRegistrationRequest request)
        {
            // Get valid OTP
            var (validOtp, errorMessage) = await _otpRepository.GetValidOtpWithDetailAsync(
       request.Email, request.Otp, OptPurpose.Register);

            if (validOtp == null)
            {
                throw new Exception(errorMessage ?? "Invalid OTP");
            }

            // Double-check that email doesn't exist (race condition protection)
            var emailExists = await _userRepository.EmailExistsAsync(request.Email);
            if (emailExists)
            {
                throw new Exception("Email already exists");
            }

            // Create the user using stored data
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                Role = UserRole.Student, // Default role
                Createdat = DateTime.UtcNow,
                EmailVerified = true, // Since they verified via OTP
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            await _userRepository.CreateAsync(user);





            // Mark OTP as used
            await _otpRepository.MarkOtpAsUsedAsync(validOtp.Id);


            await _otpRepository.DeleteUsedOtpAsync(request.Email, request.Otp);

            return "Registration successful. Please log in with your credentials.";
        }

        public async Task<LoginResponse> GoogleLoginAsync(GoogleAuthDto googleAuthDto)
        {
            try
            {
                // Verify Google ID token
                var payload = await GoogleJsonWebSignature.ValidateAsync(
                    googleAuthDto.IdToken,
                    new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = new[] { _configuration["GoogleAuth:ClientId"] } // Assuming IConfiguration is injected
                    });

                // Find or create user
                var user = await _userRepository.FindOrCreateUserFromGoogleAsync(payload);
                if (user.IsBanned)
                {
                    throw new Exception("Your account has been banned. Please contact support.");
                }
                // Generate JWT token
                var token = _jwtService.GenerateToken(user);

                return new LoginResponse
                {
                    Token = token,
                    user = user
                };
            }
            catch (Exception)
            {
                throw new Exception("Invalid Google token");
            }
        }

        public async Task<string> ResendRegistrationOtpAsync(string email)
        {
            //prevent too many requests
            var recentOtps = await _otpRepository.GetRecentOtpsAsync(email, OptPurpose.Register, TimeSpan.FromMinutes(1));
            if (recentOtps.Count >= 2) // Allow max 2 requests per minute
            {
                throw new Exception("Too many OTP requests. Please wait before trying again.");
            }
            // Check if email already exists (prevent resend for registered users)
            var emailExists = await _userRepository.EmailExistsAsync(email);
            if (emailExists)
            {
                throw new Exception("Email already registered");
            }

            var otpCode = GenerateOtp();

            // Invalidate any previous registration OTPs for this email
            await _otpRepository.InvalidatePreviousOtpsAsync(email, OptPurpose.Register);

            // Create new OTP record
            var otpVerify = new OtpVerify
            {
                Email = email,
                Otp = otpCode,
                Purpose = OptPurpose.Register,
                CreatedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddMinutes(3), // 3 minutes expiry
                IsUsed = false
            };

            await _otpRepository.CreateAsync(otpVerify);


            var subject = "P&P System Login Verification Code";
            var body = $@"
<html>
  <body style='margin:0; padding:0; background-color:#f4f6f8; font-family:Arial, sans-serif;'>
    <table role='presentation' style='width:100%; border-collapse:collapse; background-color:#f4f6f8;'>
      <tr>
        <td align='center' style='padding:40px 0;'>
          <table role='presentation' style='width:100%; max-width:600px; background:#ffffff; border-radius:10px; padding:40px; text-align:center; box-shadow:0 4px 10px rgba(0,0,0,0.05);'>
            <tr>
              <td>

                <h2 style='color:#4a6cf7; margin-bottom:20px;'>Study Plan & Practice System</h2>

                <p style='font-size:16px; color:#333; margin-bottom:20px;'>
                  Hello,<br/><br/>
                  To continue setting up your account, please use the verification code below:
                </p>

                <p style='font-size:32px; font-weight:bold; letter-spacing:4px; color:#4a6cf7; margin:25px 0;'>
                  {otpCode}
                </p>

                <p style='font-size:14px; color:#555; margin-bottom:20px;'>
                  This code will expire in <strong>3 minutes</strong>.<br/>
                  For your security, please do not share it with anyone.
                </p>

                <hr style='border:none; border-top:1px solid #eee; margin:30px 0;' />

                <p style='font-size:14px; color:#888;'>
                  If you did not request this verification, you may ignore this email.
                </p>

                <p style='font-size:14px; color:#555; margin-top:20px;'>
                  Best regards,<br/>
                  <strong>Study Plan & Practice System Team</strong>
                </p>

              </td>
            </tr>
          </table>
        </td>
      </tr>
    </table>
  </body>
</html>";
            await _emailSender.SendEmailAsync(email, subject, body);
            // TODO: Send email with new OTP code here
            return $"New OTP sent to {email}. Please check your email and verify within 3 minutes.";
        }

        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); // 6-digit OTP
        }

        public async Task<User> UpdateUserProfile(int userId, UpdateUserProfileDto updateDto)
        {
            // Get existing user
            var user = await _userRepository.GetUserById(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Update username if provided
            if (!string.IsNullOrWhiteSpace(updateDto.UserName))
            {
                user.Username = updateDto.UserName;
            }


            // Update phone if provided
            if (!string.IsNullOrWhiteSpace(updateDto.Phone))
            {
                user.Phone = updateDto.Phone;
            }

            // Handle password change
            if (!string.IsNullOrWhiteSpace(updateDto.NewPassword))
            {
                // Old password is required to change password
                if (string.IsNullOrWhiteSpace(updateDto.OldPassword))
                {
                    throw new Exception("Old password is required to set a new password");
                }

                // Verify old password
                if (!BCrypt.Net.BCrypt.Verify(updateDto.OldPassword, user.Password))
                {
                    throw new Exception("Old password is incorrect");
                }

                // Validate new password (add your validation rules)
                if (updateDto.NewPassword.Length < 1)
                {
                    throw new Exception("New password must be at least 6 characters long");
                }

                // Hash and update new password
                user.Password = BCrypt.Net.BCrypt.HashPassword(updateDto.NewPassword);
            }

            // Save changes
            await _userRepository.UpdateAsync(user);

            // Return updated user (without password)
            return user;
        }
    

     public async Task<User> UpdateTeacherRole(string email)
        {
            // Get existing user
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            user.Role = UserRole.Teacher;
            // Save changes
            await _userRepository.UpdateAsync(user);

            // Return updated user (without password)
            return user;
        }
    }

}
