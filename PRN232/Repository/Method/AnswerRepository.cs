using BusinessObject.Quiz;
using DAL;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Method
{
    public class AnswerRepository : IAnswerRepository
    {
        private readonly PlantPraticeDbContext _context;
        public AnswerRepository(PlantPraticeDbContext context)
        {
            _context = context;
        }
        public async Task CreateAnswerAsync(Answer answer)
        {
            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAnswerAsync(int answerId)
        {
            var answer = _context.Answers.FirstOrDefault(a => a.Id == answerId);
            if (answer != null)
            {
                _context.Answers.Remove(answer);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Answer> GetAnswerByIdAsync(int answerId)
        {
            return await _context.Answers.FirstOrDefaultAsync(a => a.Id == answerId);
        }

        public async Task<IEnumerable<Answer>> GetAnswersAsync()
        {
            return await _context.Answers.ToListAsync();
        }

        public async Task UpdateAnswerAsync(Answer answer)
        {
            _context.Update(answer);
            await _context.SaveChangesAsync();
        }
    }
}
