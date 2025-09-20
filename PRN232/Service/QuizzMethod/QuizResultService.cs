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
    public class QuizResultService : IQuizResultService
    {
        private readonly IQuizResultRepository _quizResultRepository;

        public QuizResultService(IQuizResultRepository quizResultRepository)
        {
            _quizResultRepository = quizResultRepository ?? throw new ArgumentNullException(nameof(quizResultRepository));
        }

        public async Task<IEnumerable<QuizResult>> GetAllQuizResultsAsync()
        {
            return await _quizResultRepository.GetQuizResultsAsync();
        }

        public async Task<QuizResult?> GetQuizResultByIdAsync(int quizResultId)
        {
            if (quizResultId <= 0)
                throw new ArgumentException("Quiz Result ID must be greater than 0", nameof(quizResultId));

            return await _quizResultRepository.GetQuizResultByIdAsync(quizResultId);
        }

        public async Task<IEnumerable<QuizResult>> GetQuizResultsByUserIdAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("User ID must be greater than 0", nameof(userId));

            var allResults = await _quizResultRepository.GetQuizResultsAsync();
            return allResults.Where(r => r.UserId == userId);
        }

        public async Task<IEnumerable<QuizResult>> GetQuizResultsByQuizIdAsync(int quizId)
        {
            if (quizId <= 0)
                throw new ArgumentException("Quiz ID must be greater than 0", nameof(quizId));

            var allResults = await _quizResultRepository.GetQuizResultsAsync();
            return allResults.Where(r => r.QuizId == quizId);
        }

        public async Task<QuizResult> CreateQuizResultAsync(QuizResult quizResult)
        {
            if (quizResult == null)
                throw new ArgumentNullException(nameof(quizResult));

            if (quizResult.UserId <= 0)
                throw new ArgumentException("User ID must be greater than 0", nameof(quizResult));

            if (quizResult.QuizId <= 0)
                throw new ArgumentException("Quiz ID must be greater than 0", nameof(quizResult));

            if (quizResult.Score < 0)
                throw new ArgumentException("Score cannot be negative", nameof(quizResult));

            await _quizResultRepository.CreateQuizResultAsync(quizResult);
            return quizResult;
        }

        public async Task<QuizResult> UpdateQuizResultAsync(QuizResult quizResult)
        {
            if (quizResult == null)
                throw new ArgumentNullException(nameof(quizResult));

            if (quizResult.Id <= 0)
                throw new ArgumentException("Quiz Result ID must be greater than 0", nameof(quizResult));

            var existingResult = await _quizResultRepository.GetQuizResultByIdAsync(quizResult.Id);
            if (existingResult == null)
                throw new InvalidOperationException($"Quiz Result with ID {quizResult.Id} not found");

            await _quizResultRepository.UpdateQuizResultAsync(quizResult);
            return quizResult;
        }

        public async Task<bool> DeleteQuizResultAsync(int quizResultId)
        {
            if (quizResultId <= 0)
                throw new ArgumentException("Quiz Result ID must be greater than 0", nameof(quizResultId));

            var existingResult = await _quizResultRepository.GetQuizResultByIdAsync(quizResultId);
            if (existingResult == null)
                return false;

            await _quizResultRepository.DeleteQuizResultAsync(quizResultId);
            return true;
        }
    }
}