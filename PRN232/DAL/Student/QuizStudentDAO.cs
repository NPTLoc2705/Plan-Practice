using BusinessObject.Quiz;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Student
{
    public class QuizStudentDAO
    {
        private readonly PlantPraticeDbContext _context;

        public QuizStudentDAO(PlantPraticeDbContext context)
        {
            _context = context;
        }






        public async Task<Quizs> GetQuizWithQuestionsAndAnswersAsync(int quizId)
        {
            return await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == quizId);
        }

    }
}
