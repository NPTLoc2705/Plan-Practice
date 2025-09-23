using BusinessObject.Quiz;
using DAL;
using DAL.QuizDAO;
using Microsoft.EntityFrameworkCore;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Method
{
    public class AnswerRepository : IAnswerRepository
    {
        private readonly AnswerDAO _dao;
        public AnswerRepository(AnswerDAO dao)
        {
            _dao = dao;
        }
        public async Task CreateAnswerAsync(Answer answer)
        {
          await _dao.CreateAnswerAsync(answer);
        }

        public async Task DeleteAnswerAsync(int answerId)
        {
          await _dao.DeleteAnswerAsync(answerId);
        }

        public async Task<Answer> GetAnswerByIdAsync(int answerId)
        {
            return await _dao.GetAnswerByIdAsync(answerId);
        }

        public async Task<IEnumerable<Answer>> GetAnswersAsync()
        {
           return await _dao.GetAnswersAsync();
        }

        public async Task UpdateAnswerAsync(Answer answer)
        {
            await _dao.UpdateAnswerAsync(answer);
        }
    }
}
