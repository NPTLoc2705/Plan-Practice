using BusinessObject.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.QuizzInterface
{
    public interface IAnswerService
    {
        Task<IEnumerable<Answer>> GetAllAnswersAsync();
        Task<Answer?> GetAnswerByIdAsync(int answerId);
        Task<IEnumerable<Answer>> GetAnswersByQuestionIdAsync(int questionId);
        Task CreateAnswerAsync(Answer answer);
        Task UpdateAnswerAsync(Answer answer);
        Task DeleteAnswerAsync(int answerId);

    }
}