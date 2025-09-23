using BusinessObject.Quiz;
using DAL;
using DAL.QuizDAO;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Method
{
    public class UserAnswerRepository : IUserAnswerRepository
    {
        private readonly UserAnswerDAO _dao;
        public UserAnswerRepository(UserAnswerDAO dao)
        {
            _dao = dao;
        }
        public async Task CreateUserAnswerAsync(UserAnswer userAnswer)
        {
           await _dao.CreateUserAnswerAsync(userAnswer);
        }

        public async Task DeleteUserAnswerAsync(int userAnswerId)
        {
           await _dao.DeleteUserAnswerAsync(userAnswerId);

        }

        public async Task<UserAnswer?> GetUserAnswerByIdAsync(int userAnswerId)
        {
            
           return await _dao.GetUserAnswerByIdAsync(userAnswerId);  
        }

        public async Task<IEnumerable<UserAnswer>> GetUserAnswersAsync()
        {
           return await _dao.GetUserAnswersAsync();
        }

        public async Task UpdateUserAnswerAsync(UserAnswer userAnswer)
        {
           await _dao.UpdateUserAnswerAsync(userAnswer);    
        }
    }
}
