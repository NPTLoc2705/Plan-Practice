using BusinessObject;
using DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class OtpVerifyRepository : IOtpVerifyRepository
    {
        private readonly OtpVerifyDAO otpVerifyDAO;
        public OtpVerifyRepository(OtpVerifyDAO otpVerifyDAO)
        {
            this.otpVerifyDAO = otpVerifyDAO;
        }
        public Task CreateAsync(OtpVerify otp)
        => otpVerifyDAO.CreateAsync(otp);

        public Task<int> DeleteExpiredOtpsAsync()
        => otpVerifyDAO.DeleteExpiredOtpsAsync();

        public Task<OtpVerify> DeleteUsedOtpAsync(string email, string otpCode)
        => otpVerifyDAO.DeleteUsedOtpAsync(email, otpCode);

        public Task<List<OtpVerify>> GetRecentOtpsAsync(string email, OptPurpose purpose, TimeSpan timeWindow)
        => otpVerifyDAO.GetRecentOtpsAsync(email, purpose, timeWindow);

        public Task<(OtpVerify? otp, string? error)> GetValidOtpWithDetailAsync(string email, string otp, OptPurpose purpose)
        => otpVerifyDAO.GetValidOtpWithDetailAsync(email, otp, purpose);

        public Task InvalidatePreviousOtpsAsync(string email, OptPurpose purpose)
        => otpVerifyDAO.InvalidatePreviousOtpsAsync(email, purpose);

        public Task MarkOtpAsUsedAsync(int otpId)
        => otpVerifyDAO.MarkOtpAsUsedAsync(otpId);
    }
}
