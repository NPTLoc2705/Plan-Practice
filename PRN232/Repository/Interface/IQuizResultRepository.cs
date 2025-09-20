using BusinessObject.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IQuizResultRepository
    {
        Task<IEnumerable<QuizResult>> GetQuizResultsAsync();
        Task<QuizResult?> GetQuizResultByIdAsync(int quizResultId);
        Task CreateQuizResultAsync(QuizResult quizResult);
        Task UpdateQuizResultAsync(QuizResult quizResult);
        Task DeleteQuizResultAsync(int quizResultId);
    }
}
