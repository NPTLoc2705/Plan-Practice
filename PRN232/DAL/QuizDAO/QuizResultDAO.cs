using BusinessObject.Quiz;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.QuizDAO
{
    public class QuizResultDAO
    {
        private readonly PlantPraticeDbContext _context;

        public QuizResultDAO(PlantPraticeDbContext context)
        {
            _context = context;
        }
        public async Task CreateQuizResultAsync(QuizResult quizResult)
        {
            _context.QuizResults.Add(quizResult);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteQuizResultAsync(int quizResultId)
        {
            var quizResult = _context.QuizResults.FirstOrDefault(q => q.Id == quizResultId);
            if (quizResult != null)
            {
                _context.QuizResults.Remove(quizResult);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<QuizResult?> GetQuizResultByIdAsync(int quizResultId)
        {
            return await _context.QuizResults.FirstOrDefaultAsync(q => q.Id == quizResultId);
        }

        public async Task<IEnumerable<QuizResult>> GetQuizResultsAsync()
        {
            return await _context.QuizResults.ToListAsync();
        }

        public Task UpdateQuizResultAsync(QuizResult quizResult)
        {
            _context.QuizResults.Update(quizResult);
            return _context.SaveChangesAsync();
        }
    }

}
