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
         Task<IEnumerable<Quizs>> GetTotalQuizzesAsync();
         Task<Quizs?> GetQuizByIdAsync(int quizId);
         Task CreateQuizAsync(Quizs quiz);
         Task UpdateQuizAsync(Quizs quiz);
         Task DeleteQuizAsync(int quizId);
         Task<IEnumerable<Quizs>> GetQuizzesByTeacherAsync(int teacherId);
         Task<IEnumerable<QuizResult>> GetQuizResultsByQuizIdAsync(int quizId);
    }
}
