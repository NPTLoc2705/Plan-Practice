using BusinessObject.Quiz;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Student
{
    public class UserAnswerStudentDAO
    {
        private readonly PlantPraticeDbContext _context;

        public UserAnswerStudentDAO(PlantPraticeDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserAnswer>> GetUserAnswersByQuizResultIdAsync(int quizResultId)
        {
            return await _context.UserAnswers
                .Include(ua => ua.Question)
                .Include(ua => ua.Answer)
                .Where(ua => ua.QuizResultId == quizResultId)
                .ToListAsync();
        }


        public async Task CreateUserAnswersAsync(IEnumerable<UserAnswer> userAnswers)
        {
            _context.UserAnswers.AddRange(userAnswers);
            await _context.SaveChangesAsync();
        }


    }
}
