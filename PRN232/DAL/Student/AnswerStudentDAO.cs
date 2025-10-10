using BusinessObject.Quiz;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Student
{
    public class AnswerStudentDAO
    {
        private readonly PlantPraticeDbContext _context;

        public AnswerStudentDAO(PlantPraticeDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Answer>> GetAnswersByQuestionIdAsync(int questionId)
        {
            return await _context.Answers
                .Where(a => a.QuestionId == questionId)
                .ToListAsync();
        }

        private async Task<Answer> GetAnswerByIdAsync(int answerId)
        {
            return await _context.Answers
                .FirstOrDefaultAsync(a => a.Id == answerId);
        }

        public async Task<Answer> GetCorrectAnswerByQuestionIdAsync(int questionId)
        {
            return await _context.Answers
                .FirstOrDefaultAsync(a => a.QuestionId == questionId && a.IsCorrect);
        }

        public async Task<bool> IsAnswerCorrectAsync(int answerId)
        {
            var answer = await GetAnswerByIdAsync(answerId);
            return answer?.IsCorrect ?? false;
        }


    }
}
