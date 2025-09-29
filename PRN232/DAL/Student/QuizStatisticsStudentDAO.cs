using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Student
{
    public class QuizStatisticsStudentDAO
    {
        private readonly PlantPraticeDbContext _context;

        public QuizStatisticsStudentDAO(PlantPraticeDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetUserBestScoreAsync(int userId, int quizId)
        {
            var results = await _context.QuizResults
                .Where(r => r.UserId == userId && r.QuizId == quizId && r.Score > 0)
                .ToListAsync();

            return results.Any() ? results.Max(r => r.Score) : 0;
        }

        public async Task<int> GetUserAttemptCountAsync(int userId, int quizId)
        {
            return await _context.QuizResults
                .CountAsync(r => r.UserId == userId && r.QuizId == quizId && r.Score > 0);
        }
    }
}

