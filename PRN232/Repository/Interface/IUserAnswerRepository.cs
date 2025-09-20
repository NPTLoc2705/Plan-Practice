using BusinessObject.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IUserAnswerRepository
    {
        Task<IEnumerable<UserAnswer>> GetUserAnswersAsync();
        Task<UserAnswer?> GetUserAnswerByIdAsync(int userAnswerId);
        Task CreateUserAnswerAsync(UserAnswer userAnswer);
        Task UpdateUserAnswerAsync(UserAnswer userAnswer);
        Task DeleteUserAnswerAsync(int userAnswerId);
    }
}
