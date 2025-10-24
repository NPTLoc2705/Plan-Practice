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
        public async Task CreateQuizAsync(Quizs quiz)
        {
            await _dao.CreateQuizAsync(quiz);
        }

        public async Task DeleteQuizAsync(int quizId)
        {
            await _dao.DeleteQuizAsync(quizId);
        }

        public async Task<Quizs?> GetQuizByIdAsync(int quizId)
        {
           return await _dao.GetQuizByIdAsync(quizId);

        }

        public async Task<IEnumerable<Quizs>> GetTotalQuizzesAsync()
        {
           return await _dao.GetTotalQuizzesAsync();
        }

        public async Task UpdateQuizAsync(Quizs quiz)
        {
           await _dao.UpdateQuizAsync(quiz);
        }

        public async Task<IEnumerable<Quizs>> GetQuizzesByTeacherAsync(int teacherId)
        {
            return null;
        }

        public async Task<IEnumerable<QuizResult>> GetQuizResultsByQuizIdAsync(int quizId)
        {
            return await _dao.GetQuizResultsByQuizIdAsync(quizId);
        }
    }
}
