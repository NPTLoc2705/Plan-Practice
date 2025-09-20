using BusinessObject.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<Question>> GetQuestionsAsync();
        Task<Question?> GetQuestionByIdAsync(int questionId);
        Task CreateQuestionAsync(Question question);
        Task UpdateQuestionAsync(Question question);
        Task DeleteQuestionAsync(int questionId);
    }
}
