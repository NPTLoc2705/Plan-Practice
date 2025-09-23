using BusinessObject.Quiz;
using DAL;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using DAL.QuizDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Method
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly QuestionDAO _dao;

        public QuestionRepository(QuestionDAO dao)
        {
            _dao = dao;
        }
        public async Task CreateQuestionAsync(Question question)
        {
            await _dao.CreateQuestionAsync(question);
        }

        public async Task DeleteQuestionAsync(int questionId)
        {
          await _dao.DeleteQuestionAsync(questionId);
        }

        public async Task<Question?> GetQuestionByIdAsync(int questionId)
        {
           return await _dao.GetQuestionByIdAsync(questionId);
        }

        public async Task<IEnumerable<Question>> GetQuestionsAsync()
        {
           return await _dao.GetQuestionsAsync();
        }

        public async Task UpdateQuestionAsync(Question question)
        {
           await _dao.UpdateQuestionAsync(question);
        }
    }
}
