using BusinessObject.Dtos.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.QuizzInterface
{
    public interface IQuizOTPService
    {
        // Teacher operations
        Task<QuizOTPDto> GenerateOTPAsync(int quizId, int teacherId, GenerateOTPDto dto);
        Task<IEnumerable<QuizOTPDto>> GetTeacherOTPsAsync(int teacherId);
        Task<IEnumerable<QuizOTPDto>> GetQuizOTPsAsync(int quizId, int teacherId);
        Task<bool> RevokeOTPAsync(int otpId, int teacherId);
        Task<IEnumerable<OTPAccessLogDto>> GetOTPAccessLogsAsync(int otpId, int teacherId);

        // Student operations
        Task<OTPValidationResultDto> ValidateOTPAsync(string otpCode, int studentId, string ipAddress, string userAgent);
        Task<QuizDetailDto> GetQuizByOTPAsync(string otpCode, int studentId);

        // System operations
        Task<int> CleanupExpiredOTPsAsync();
        Task<bool> ExtendOTPExpiryAsync(int otpId, int teacherId, int additionalMinutes);
        Task<QuizOTPDto> RegenerateOTPAsync(int otpId, int teacherId);
    }
}
