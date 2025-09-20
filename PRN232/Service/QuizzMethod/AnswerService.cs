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
    public class AnswerService : IAnswerService
    {
        private readonly IAnswerRepository _answerRepository;

        public AnswerService(IAnswerRepository answerRepository)
        {
            _answerRepository = answerRepository ?? throw new ArgumentNullException(nameof(answerRepository));
        }

        public async Task<IEnumerable<Answer>> GetAllAnswersAsync()
        {
            return await _answerRepository.GetAnswersAsync();
        }

        public async Task<Answer?> GetAnswerByIdAsync(int answerId)
        {
            if (answerId <= 0)
                throw new ArgumentException("Answer ID must be greater than 0", nameof(answerId));

            return await _answerRepository.GetAnswerByIdAsync(answerId);
        }

        public async Task<IEnumerable<Answer>> GetAnswersByQuestionIdAsync(int questionId)
        {
            if (questionId <= 0)
                throw new ArgumentException("Question ID must be greater than 0", nameof(questionId));

            var allAnswers = await _answerRepository.GetAnswersAsync();
            return allAnswers.Where(a => a.QuestionId == questionId);
        }

        public async Task<Answer> CreateAnswerAsync(Answer answer)
        {
            if (answer == null)
                throw new ArgumentNullException(nameof(answer));

            if (string.IsNullOrWhiteSpace(answer.Content))
                throw new ArgumentException("Answer content is required", nameof(answer));

            if (answer.QuestionId <= 0)
                throw new ArgumentException("Question ID must be greater than 0", nameof(answer));

            await _answerRepository.CreateAnswerAsync(answer);
            return answer;
        }

        public async Task<Answer> UpdateAnswerAsync(Answer answer)
        {
            if (answer == null)
                throw new ArgumentNullException(nameof(answer));

            if (answer.Id <= 0)
                throw new ArgumentException("Answer ID must be greater than 0", nameof(answer));

            if (string.IsNullOrWhiteSpace(answer.Content))
                throw new ArgumentException("Answer content is required", nameof(answer));

            var existingAnswer = await _answerRepository.GetAnswerByIdAsync(answer.Id);
            if (existingAnswer == null)
                throw new InvalidOperationException($"Answer with ID {answer.Id} not found");

            await _answerRepository.UpdateAnswerAsync(answer);
            return answer;
        }

        public async Task<bool> DeleteAnswerAsync(int answerId)
        {
            if (answerId <= 0)
                throw new ArgumentException("Answer ID must be greater than 0", nameof(answerId));

            var existingAnswer = await _answerRepository.GetAnswerByIdAsync(answerId);
            if (existingAnswer == null)
                return false;

            await _answerRepository.DeleteAnswerAsync(answerId);
            return true;
        }
    }
}