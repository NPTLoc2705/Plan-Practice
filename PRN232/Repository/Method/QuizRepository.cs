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
    public class QuizRepository : IQuizRepository
    {
        private readonly QuizDAO _dao;

        public QuizRepository(QuizDAO dao)
        {
            _dao = dao;
        }
        public async Task CreateQuizAsync(Quiz quiz)
        {
            await _dao.CreateQuizAsync(quiz);
        }

        public async Task DeleteQuizAsync(int quizId)
        {
            await _dao.DeleteQuizAsync(quizId);
        }

        public async Task<Quiz?> GetQuizByIdAsync(int quizId)
        {
           return await _dao.GetQuizByIdAsync(quizId);

        }

        public async Task<IEnumerable<Quiz>> GetTotalQuizzesAsync()
        {
           return await _dao.GetTotalQuizzesAsync();
        }

        public async Task UpdateQuizAsync(Quiz quiz)
        {
           await _dao.UpdateQuizAsync(quiz);
        }
    }
}
