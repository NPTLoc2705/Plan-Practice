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
        Task CreateQuizResultAsync(QuizResult quizResult);
        Task UpdateQuizResultAsync(QuizResult quizResult);
        Task DeleteQuizResultAsync(int quizResultId);
    }
}