using BusinessObject.Quiz;
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

       

        public async Task<int> GetQuestionCountByQuizIdAsync(int quizId)
        {
            return await _context.Questions
                .CountAsync(q => q.QuizId == quizId);
        }
    }

}
