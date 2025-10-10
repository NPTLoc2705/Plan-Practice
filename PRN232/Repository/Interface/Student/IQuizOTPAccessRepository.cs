using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface.Student
{
    public interface IQuizOTPAccessRepository
    {
        //CRUD
        Task<QuizOTPAccess> GetByIdAsync(int id);
        Task<IEnumerable<QuizOTPAccess>> GetAllAsync();
        Task<QuizOTPAccess> CreateAsync(QuizOTPAccess access);



        Task<IEnumerable<QuizOTPAccess>> GetByOTPIdAsync(int otpId);
        Task<IEnumerable<QuizOTPAccess>> GetByStudentIdAsync(int studentId);
        Task<bool> HasStudentAccessedOTPAsync(int otpId, int studentId);
        Task<int> GetAccessCountByOTPIdAsync(int otpId);
        Task<QuizOTPAccess> GetLatestAccessAsync(int otpId, int studentId);
    }
}
