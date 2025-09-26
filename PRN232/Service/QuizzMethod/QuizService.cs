using BusinessObject.Quiz;
using Repository.Interface;
using Service.QuizzInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.QuizzMethod
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository _quizRepository;

        public QuizService(IQuizRepository quizRepository)
        {
            _quizRepository = quizRepository ?? throw new ArgumentNullException(nameof(quizRepository));
        }

        public async Task<IEnumerable<Quiz>> GetAllQuizzesAsync()
        {
            return await _quizRepository.GetTotalQuizzesAsync();
        }

        public async Task<Quiz?> GetQuizByIdAsync(int quizId)
        {
            if (quizId <= 0)
                throw new ArgumentException("Quiz ID must be greater than 0", nameof(quizId));

            return await _quizRepository.GetQuizByIdAsync(quizId);
        }

        public async Task CreateQuizAsync(Quiz quiz)
        {
            if (quiz == null)
                throw new ArgumentNullException(nameof(quiz));

            if (string.IsNullOrWhiteSpace(quiz.Title))
                throw new ArgumentException("Quiz title is required", nameof(quiz));

            await _quizRepository.CreateQuizAsync(quiz);
           
        }

        public async Task UpdateQuizAsync(Quiz quiz)
        {  
            await _quizRepository.UpdateQuizAsync(quiz);           
        }

        public async Task DeleteQuizAsync(int quizId)
        {
            if (quizId <= 0)
                throw new ArgumentException("Quiz ID must be greater than 0", nameof(quizId));
            await _quizRepository.DeleteQuizAsync(quizId);

        }
    }
}