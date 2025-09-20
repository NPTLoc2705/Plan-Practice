using BusinessObject.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.QuizzInterface
{
    public interface IQuestionService
    {
        Task<IEnumerable<Question>> GetAllQuestionsAsync();
        Task<Question?> GetQuestionByIdAsync(int questionId);
        Task<IEnumerable<Question>> GetQuestionsByQuizIdAsync(int quizId);
        Task<Question> CreateQuestionAsync(Question question);
        Task<Question> UpdateQuestionAsync(Question question);
        Task<bool> DeleteQuestionAsync(int questionId);
    }
}