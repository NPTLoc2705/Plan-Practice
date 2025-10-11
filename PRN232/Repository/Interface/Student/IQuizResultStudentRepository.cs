using BusinessObject.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface.Student
{
    public interface IQuizResultStudentRepository
    {
        Task<IEnumerable<QuizResult>> GetQuizResultsByQuizIdAsync(int quizId);
        Task<bool> HasUserAttemptedQuizAsync(int userId, int quizId);
        Task<QuizResult> GetLatestQuizResultByUserAndQuizAsync(int userId, int quizId);
        Task<QuizResult> GetQuizResultWithDetailsAsync(int quizResultId);
        Task<IEnumerable<QuizResult>> GetCompletedQuizResultsByUserAsync(int userId);
        Task<QuizResult> GetInProgressQuizResultAsync(int userId, int quizId);
    }
}
