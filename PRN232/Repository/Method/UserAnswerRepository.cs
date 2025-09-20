using BusinessObject.Quiz;
using DAL;
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
        private readonly PlantPraticeDbContext _context;
        public UserAnswerRepository(PlantPraticeDbContext context)
        {
            _context = context;
        }
        public async Task CreateUserAnswerAsync(UserAnswer userAnswer)
        {
           _context.UserAnswers.Add(userAnswer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAnswerAsync(int userAnswerId)
        {
           var userAnswer = _context.UserAnswers.Find(userAnswerId);
            if (userAnswer != null)
            {
                _context.UserAnswers.Remove(userAnswer);
                await _context.SaveChangesAsync();
            }
          
        }

        public async Task<UserAnswer?> GetUserAnswerByIdAsync(int userAnswerId)
        {
            
            return await _context.UserAnswers.FirstOrDefaultAsync(u=>u.AnswerId ==userAnswerId);
        }

        public async Task<IEnumerable<UserAnswer>> GetUserAnswersAsync()
        {
            return await _context.UserAnswers.ToListAsync();
        }

        public async Task UpdateUserAnswerAsync(UserAnswer userAnswer)
        {
            _context.UserAnswers.Update(userAnswer);
            await _context.SaveChangesAsync();
        }
    }
}
