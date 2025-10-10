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
    public class QuizOTPAccessRepository : IQuizOTPAccessRepository
    {
        private readonly QuizOTPAccessDAO _quizAccessOTPDAO;
        public QuizOTPAccessRepository(QuizOTPAccessDAO QuizAccessOTPDAO)
        {
            this._quizAccessOTPDAO = QuizAccessOTPDAO;
        }

        public Task<QuizOTPAccess> CreateAsync(QuizOTPAccess access)
       => _quizAccessOTPDAO.CreateAsync(access);

        public Task<int> GetAccessCountByOTPIdAsync(int otpId)
        => _quizAccessOTPDAO.GetAccessCountByOTPIdAsync(otpId);

        public Task<IEnumerable<QuizOTPAccess>> GetAllAsync()
        => _quizAccessOTPDAO.GetAllAsync();

        public Task<QuizOTPAccess> GetByIdAsync(int id)
        => _quizAccessOTPDAO.GetByIdAsync(id);

        public Task<IEnumerable<QuizOTPAccess>> GetByOTPIdAsync(int otpId)
        => _quizAccessOTPDAO.GetByOTPIdAsync(otpId);

        public Task<IEnumerable<QuizOTPAccess>> GetByStudentIdAsync(int studentId)
        => _quizAccessOTPDAO.GetByStudentIdAsync(studentId);

        public Task<QuizOTPAccess> GetLatestAccessAsync(int otpId, int studentId)
        => _quizAccessOTPDAO.GetLatestAccessAsync(otpId, studentId);

        public Task<bool> HasStudentAccessedOTPAsync(int otpId, int studentId)
        => _quizAccessOTPDAO.HasStudentAccessedOTPAsync(otpId, studentId);
    }
}
