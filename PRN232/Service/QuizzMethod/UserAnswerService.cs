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
    public class UserAnswerService : IUserAnswerService
    {
        private readonly IUserAnswerRepository _userAnswerRepository;

        public UserAnswerService(IUserAnswerRepository userAnswerRepository)
        {
            _userAnswerRepository = userAnswerRepository ?? throw new ArgumentNullException(nameof(userAnswerRepository));
        }

        public async Task<IEnumerable<UserAnswer>> GetAllUserAnswersAsync()
        {
            return await _userAnswerRepository.GetUserAnswersAsync();
        }

        public async Task<UserAnswer?> GetUserAnswerByIdAsync(int userAnswerId)
        {
            if (userAnswerId <= 0)
                throw new ArgumentException("User Answer ID must be greater than 0", nameof(userAnswerId));

            return await _userAnswerRepository.GetUserAnswerByIdAsync(userAnswerId);
        }

        public async Task<IEnumerable<UserAnswer>> GetUserAnswersByQuizResultIdAsync(int quizResultId)
        {
            if (quizResultId <= 0)
                throw new ArgumentException("Quiz Result ID must be greater than 0", nameof(quizResultId));

            var allUserAnswers = await _userAnswerRepository.GetUserAnswersAsync();
            return allUserAnswers.Where(ua => ua.QuizResultId == quizResultId);
        }

        public async Task<IEnumerable<UserAnswer>> GetUserAnswersByQuestionIdAsync(int questionId)
        {
            if (questionId <= 0)
                throw new ArgumentException("Question ID must be greater than 0", nameof(questionId));

            var allUserAnswers = await _userAnswerRepository.GetUserAnswersAsync();
            return allUserAnswers.Where(ua => ua.QuestionId == questionId);
        }

        public async Task CreateUserAnswerAsync(UserAnswer userAnswer)
        {
            if (userAnswer == null)
                throw new ArgumentNullException(nameof(userAnswer));

            if (userAnswer.QuizResultId <= 0)
                throw new ArgumentException("Quiz Result ID must be greater than 0", nameof(userAnswer));

            if (userAnswer.QuestionId <= 0)
                throw new ArgumentException("Question ID must be greater than 0", nameof(userAnswer));

            if (userAnswer.AnswerId <= 0)
                throw new ArgumentException("Answer ID must be greater than 0", nameof(userAnswer));

            await _userAnswerRepository.CreateUserAnswerAsync(userAnswer);
            
        }

        public async Task UpdateUserAnswerAsync(UserAnswer userAnswer)
        {
        

            await _userAnswerRepository.UpdateUserAnswerAsync(userAnswer);
            
        }

        public async Task DeleteUserAnswerAsync(int userAnswerId)
        {
            if (userAnswerId <= 0)
                throw new ArgumentException("User Answer ID must be greater than 0", nameof(userAnswerId));


            await _userAnswerRepository.DeleteUserAnswerAsync(userAnswerId);
            
        }
    }
}