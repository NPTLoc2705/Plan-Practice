using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IOtpVerifyRepository
    {
        Task CreateAsync(OtpVerify otp);
        Task InvalidatePreviousOtpsAsync(string email, OptPurpose purpose);
        Task MarkOtpAsUsedAsync(int otpId);
        Task<OtpVerify> DeleteUsedOtpAsync(string email, string otpCode);
        Task<List<OtpVerify>> GetRecentOtpsAsync(string email, OptPurpose purpose, TimeSpan timeWindow);
        Task<(OtpVerify? otp, string? error)> GetValidOtpWithDetailAsync(string email, string otp, OptPurpose purpose);
        Task<int> DeleteExpiredOtpsAsync();
    }
}
