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
    public class QuizResultRepository : IQuizResultRepository
    {
        private readonly QuizResultDAO _dao;

        public QuizResultRepository(QuizResultDAO dao)
        {
            _dao = dao;
        }
        public async Task CreateQuizResultAsync(QuizResult quizResult)
        {
            await _dao.CreateQuizResultAsync(quizResult);
        }

        public async Task DeleteQuizResultAsync(int quizResultId)
        {
            await _dao.DeleteQuizResultAsync(quizResultId);
        }

        public async Task<QuizResult?> GetQuizResultByIdAsync(int quizResultId)
        {
           return await _dao.GetQuizResultByIdAsync(quizResultId);
        }

        public async Task<IEnumerable<QuizResult>> GetQuizResultsAsync()
        {
           return await _dao.GetQuizResultsAsync();
        }

        public async Task UpdateQuizResultAsync(QuizResult quizResult)
        {
           await _dao.UpdateQuizResultAsync(quizResult);
        }
    }
}
