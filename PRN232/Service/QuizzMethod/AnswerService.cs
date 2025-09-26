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

        public async Task CreateAnswerAsync(Answer answer)
        {
            if (answer == null)
                throw new ArgumentNullException(nameof(answer));

            if (string.IsNullOrWhiteSpace(answer.Content))
                throw new ArgumentException("Answer content is required", nameof(answer));

            if (answer.QuestionId <= 0)
                throw new ArgumentException("Question ID must be greater than 0", nameof(answer));

            await _answerRepository.CreateAnswerAsync(answer);
            
        }

        public async Task UpdateAnswerAsync(Answer answer)
        {
      

            await _answerRepository.UpdateAnswerAsync(answer);
            
        }

        public async Task DeleteAnswerAsync(int answerId)
        {          
            await _answerRepository.DeleteAnswerAsync(answerId);         
        }
    }
}