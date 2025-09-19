using BusinessObject.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject;
namespace Service
{
    public interface IUserService
    {
        Task<(string token, User user)> LoginAsync(LoginDto loginDto);
        Task<string> SendRegistrationOtpAsync(RegisterDto registerDto);
        Task<string> VerifyRegistrationAsync(VerifyRegistrationRequest request);
        Task<string> ResendRegistrationOtpAsync(string email);
        Task<LoginResponse> GoogleLoginAsync(GoogleAuthDto googleAuthDto);
    }
}
