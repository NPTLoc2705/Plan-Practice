using BusinessObject.Quiz;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Student
{
    public class QuizResultStudentDAO
    {
        private readonly PlantPraticeDbContext _context;
        public QuizResultStudentDAO(PlantPraticeDbContext context)
        {
            _context = context;
        }


        public async Task<IEnumerable<QuizResult>> GetQuizResultsByQuizIdAsync(int quizId)
        {
            return await _context.QuizResults
                .Where(r => r.QuizId == quizId)
                .ToListAsync();
        }



        public async Task<bool> HasUserAttemptedQuizAsync(int userId, int quizId)
        {
            return await _context.QuizResults
                .AnyAsync(r => r.UserId == userId && r.QuizId == quizId && r.Score > 0);
        }

        public async Task<QuizResult> GetLatestQuizResultByUserAndQuizAsync(int userId, int quizId)
        {
            return await _context.QuizResults
                .Where(r => r.UserId == userId && r.QuizId == quizId)
                .OrderByDescending(r => r.CompletedAt)
                .FirstOrDefaultAsync();
        }



        public async Task<QuizResult> GetQuizResultWithDetailsAsync(int quizResultId)
        {
            return await _context.QuizResults
                .Include(r => r.Quiz)
                    .ThenInclude(q => q.Questions)
                    .ThenInclude(q => q.Answers)
                .Include(r => r.UserAnswers)
                    .ThenInclude(ua => ua.Answer)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == quizResultId);
        }

        public async Task<IEnumerable<QuizResult>> GetCompletedQuizResultsByUserAsync(int userId)
        {
            return await _context.QuizResults
                .Include(r => r.Quiz)
                .Where(r => r.UserId == userId && r.Score != null)
                .OrderByDescending(r => r.CompletedAt)
                .ToListAsync();
        }

        public async Task<QuizResult> GetInProgressQuizResultAsync(int userId, int quizId)
        {
            return await _context.QuizResults
                .Include(r => r.UserAnswers)
                .FirstOrDefaultAsync(r => r.UserId == userId && r.QuizId == quizId && r.Score == 0);
        }
    }

}
