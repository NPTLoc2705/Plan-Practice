using BusinessObject;
using DAL.Student;
using Repository.Interface.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Method.Student
{
    public class QuizOTPRepository : IQuizOTPRepository
    {
        private readonly QuizOTPDAO _quizOTPDAO;
        public QuizOTPRepository(QuizOTPDAO quizOTPDAO)
        {
            this._quizOTPDAO = quizOTPDAO;
        }

        public Task<QuizOTP> CreateAsync(QuizOTP quizOTP) => _quizOTPDAO.CreateAsync(quizOTP);



        public Task<int> DeactivateExpiredOTPsAsync() => _quizOTPDAO.DeactivateExpiredOTPsAsync();



        public Task<bool> DeleteAsync(int id) => _quizOTPDAO.DeleteAsync(id);


        public Task<QuizOTP> GetActiveOTPByCodeAsync(string otpCode) => _quizOTPDAO.GetActiveOTPByCodeAsync(otpCode);



        public Task<IEnumerable<QuizOTP>> GetActiveOTPsAsync() => _quizOTPDAO.GetActiveOTPsAsync();



        public Task<IEnumerable<QuizOTP>> GetAllAsync() => _quizOTPDAO.GetAllAsync();
     

        public Task<QuizOTP> GetByIdAsync(int id) => _quizOTPDAO.GetByIdAsync(id);




        public Task<QuizOTP> GetByOTPCodeAsync(string otpCode) => _quizOTPDAO.GetByOTPCodeAsync(otpCode);



        public Task<IEnumerable<QuizOTP>> GetByQuizIdAsync(int quizId) => _quizOTPDAO.GetByQuizIdAsync(quizId);



        public Task<IEnumerable<QuizOTP>> GetByTeacherIdAsync(int teacherId) => _quizOTPDAO.GetByTeacherIdAsync(teacherId);



        public Task<IEnumerable<QuizOTP>> GetExpiredOTPsAsync() => _quizOTPDAO.GetExpiredOTPsAsync();


        public Task<bool> IsOTPCodeExistsAsync(string otpCode) => _quizOTPDAO.IsOTPCodeExistsAsync(otpCode);



        public Task<QuizOTP> UpdateAsync(QuizOTP quizOTP) => _quizOTPDAO.UpdateAsync(quizOTP);

        public async Task DeleteExpiredOtpsAsync() => _quizOTPDAO.DeleteExpiredOtpAsynce();
        



    }
}
