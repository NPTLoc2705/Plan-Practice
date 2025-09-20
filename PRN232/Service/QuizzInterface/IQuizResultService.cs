using BusinessObject.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.QuizzInterface
{
    public interface IQuizResultService
    {
        Task<IEnumerable<QuizResult>> GetAllQuizResultsAsync();
        Task<QuizResult?> GetQuizResultByIdAsync(int quizResultId);
        Task<IEnumerable<QuizResult>> GetQuizResultsByUserIdAsync(int userId);
        Task<IEnumerable<QuizResult>> GetQuizResultsByQuizIdAsync(int quizId);
        Task<QuizResult> CreateQuizResultAsync(QuizResult quizResult);
        Task<QuizResult> UpdateQuizResultAsync(QuizResult quizResult);
        Task<bool> DeleteQuizResultAsync(int quizResultId);
    }
}