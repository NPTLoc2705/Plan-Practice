using BusinessObject.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.QuizzInterface
{
    public interface IQuizService
    {
        Task<IEnumerable<Quiz>> GetAllQuizzesAsync();
        Task<Quiz?> GetQuizByIdAsync(int quizId);
        Task CreateQuizAsync(Quiz quiz);
        Task UpdateQuizAsync(Quiz quiz);
        Task DeleteQuizAsync(int quizId);
    }
}