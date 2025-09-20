using BusinessObject.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.QuizzInterface
{
    public interface IUserAnswerService
    {
        Task<IEnumerable<UserAnswer>> GetAllUserAnswersAsync();
        Task<UserAnswer?> GetUserAnswerByIdAsync(int userAnswerId);
        Task<IEnumerable<UserAnswer>> GetUserAnswersByQuizResultIdAsync(int quizResultId);
        Task<IEnumerable<UserAnswer>> GetUserAnswersByQuestionIdAsync(int questionId);
        Task<UserAnswer> CreateUserAnswerAsync(UserAnswer userAnswer);
        Task<UserAnswer> UpdateUserAnswerAsync(UserAnswer userAnswer);
        Task<bool> DeleteUserAnswerAsync(int userAnswerId);
    }
}