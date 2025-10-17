using BusinessObject.Quiz;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.QuizDAO
{
    public class QuizDAO
    {
        private readonly PlantPraticeDbContext _context;

        public QuizDAO(PlantPraticeDbContext context)
        {
            _context = context;
        }
        public async Task CreateQuizAsync(Quiz quiz)
        {
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteQuizAsync(int quizId)
        {
            var quiz = _context.Quizzes.FirstOrDefault(q => q.Id == quizId);
            if (quiz != null)
            {
                _context.Quizzes.Remove(quiz);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Quiz?> GetQuizByIdAsync(int quizId)
        {
            return await _context.Quizzes.FirstOrDefaultAsync(q => q.Id == quizId);

        }

        public async Task<IEnumerable<Quiz>> GetTotalQuizzesAsync()
        {
            return await _context.Quizzes.ToListAsync();
        }

        public async Task UpdateQuizAsync(Quiz quiz)
        {
            _context.Update(quiz);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Quiz>> GetQuizzesByTeacherAsync(int teacherId)
        {
            return await _context.Quizzes
                .Where(q => q.CreatedBy == teacherId)
                .Include(q => q.QuizResults)
                .Include(q => q.Questions)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<QuizResult>> GetQuizResultsByQuizIdAsync(int quizId)
        {
            return await _context.QuizResults
                .Where(qr => qr.QuizId == quizId)
                .Include(qr => qr.User)
                .ToListAsync();
        }
    }

}
