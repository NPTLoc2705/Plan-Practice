using BusinessObject.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface.Template
{
    public interface IOptService
    {
        Task<string> ResendRegistrationOtpAsync(string email);

        Task<string> VerifyRegistrationAsync(VerifyRegistrationRequest request);

        Task<string> SendRegistrationOtpAsync(RegisterDto registerDto);
    }
}
