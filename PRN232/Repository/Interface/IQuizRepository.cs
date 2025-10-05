using BusinessObject.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IQuizRepository
    {
         Task<IEnumerable<Quiz>> GetTotalQuizzesAsync();
         Task<Quiz?> GetQuizByIdAsync(int quizId);
         Task CreateQuizAsync(Quiz quiz);
         Task UpdateQuizAsync(Quiz quiz);
         Task DeleteQuizAsync(int quizId);
         Task<IEnumerable<Quiz>> GetQuizzesByTeacherAsync(int teacherId);
         Task<IEnumerable<QuizResult>> GetQuizResultsByQuizIdAsync(int quizId);
    }
}
