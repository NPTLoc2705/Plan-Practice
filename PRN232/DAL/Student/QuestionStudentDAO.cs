using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Student
{
    public class QuestionStudentDAO
    {
  

        private readonly PlantPraticeDbContext _context;
        public QuestionStudentDAO(PlantPraticeDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int> GetQuestionCountByQuizIdAsync(int quizId)
        {
            if (_context == null)
                throw new Exception("DbContext is null in QuestionStudentDAO");

            if (_context.Questions == null)
                throw new Exception("Questions DbSet is null in QuestionStudentDAO");

            return await _context.Questions
                .CountAsync(q => q.QuizId == quizId);
        }
    }

}
