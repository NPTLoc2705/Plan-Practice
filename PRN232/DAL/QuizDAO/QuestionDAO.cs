using BusinessObject.Quiz;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.QuizDAO
{
    public class QuestionDAO
    {
        private readonly PlantPraticeDbContext _context;

        public QuestionDAO(PlantPraticeDbContext context)
        {
            _context = context;
        }
        public async Task CreateQuestionAsync(Question question)
        {
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteQuestionAsync(int questionId)
        {
            var question = _context.Questions.FirstOrDefault(q => q.Id == questionId);
            if (question != null)
            {
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Question?> GetQuestionByIdAsync(int questionId)
        {
            return await _context.Questions.FirstOrDefaultAsync(q => q.Id == questionId);
        }

        public async Task<IEnumerable<Question>> GetQuestionsAsync()
        {
            return await _context.Questions.ToListAsync();
        }

        public async Task UpdateQuestionAsync(Question question)
        {
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Question>> GetQuestionsByQuizIdAsync(int quizId)
        {
            return await _context.Questions.Where(q => q.QuizId == quizId).ToListAsync();
        }
    }

}
