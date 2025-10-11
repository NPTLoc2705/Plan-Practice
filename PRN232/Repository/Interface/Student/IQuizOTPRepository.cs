using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface.Student
{
    public interface IQuizOTPRepository
    {
        //CRUD
        Task<QuizOTP> GetByIdAsync(int id);
        Task<QuizOTP> GetByOTPCodeAsync(string otpCode);
        Task<IEnumerable<QuizOTP>> GetAllAsync();
        Task<QuizOTP> CreateAsync(QuizOTP quizOTP);
        Task<QuizOTP> UpdateAsync(QuizOTP quizOTP);
        Task<bool> DeleteAsync(int id);

        Task<IEnumerable<QuizOTP>> GetByTeacherIdAsync(int teacherId);
        Task<IEnumerable<QuizOTP>> GetByQuizIdAsync(int quizId);
        Task<IEnumerable<QuizOTP>> GetActiveOTPsAsync();
        Task<bool> IsOTPCodeExistsAsync(string otpCode);
        Task<QuizOTP> GetActiveOTPByCodeAsync(string otpCode);
        Task<IEnumerable<QuizOTP>> GetExpiredOTPsAsync();
        Task<int> DeactivateExpiredOTPsAsync();
    }
}
