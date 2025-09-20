using BusinessObject.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IAnswerRepository
    {
        Task<IEnumerable<Answer>> GetAnswersAsync();
        Task<Answer> GetAnswerByIdAsync(int answerId);
        Task CreateAnswerAsync(Answer answer);
        Task UpdateAnswerAsync(Answer answer);
        Task DeleteAnswerAsync(int answerId);
    }
}
