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
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;

        public QuestionService(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository ?? throw new ArgumentNullException(nameof(questionRepository));
        }

        public async Task<IEnumerable<Question>> GetAllQuestionsAsync()
        {
            return await _questionRepository.GetQuestionsAsync();
        }

        public async Task<Question?> GetQuestionByIdAsync(int questionId)
        {
            if (questionId <= 0)
                throw new ArgumentException("Question ID must be greater than 0", nameof(questionId));

            return await _questionRepository.GetQuestionByIdAsync(questionId);
        }

        public async Task<IEnumerable<Question>> GetQuestionsByQuizIdAsync(int quizId)
        {
            if (quizId <= 0)
                throw new ArgumentException("Quiz ID must be greater than 0", nameof(quizId));

            var allQuestions = await _questionRepository.GetQuestionsAsync();
            return allQuestions.Where(q => q.QuizId == quizId);
        }

        public async Task<Question> CreateQuestionAsync(Question question)
        {
            if (question == null)
                throw new ArgumentNullException(nameof(question));

            if (string.IsNullOrWhiteSpace(question.Content))
                throw new ArgumentException("Question content is required", nameof(question));

            if (question.QuizId <= 0)
                throw new ArgumentException("Quiz ID must be greater than 0", nameof(question));

            await _questionRepository.CreateQuestionAsync(question);
            return question;
        }

        public async Task<Question> UpdateQuestionAsync(Question question)
        {
            if (question == null)
                throw new ArgumentNullException(nameof(question));

            if (question.Id <= 0)
                throw new ArgumentException("Question ID must be greater than 0", nameof(question));

            if (string.IsNullOrWhiteSpace(question.Content))
                throw new ArgumentException("Question content is required", nameof(question));

            var existingQuestion = await _questionRepository.GetQuestionByIdAsync(question.Id);
            if (existingQuestion == null)
                throw new InvalidOperationException($"Question with ID {question.Id} not found");

            await _questionRepository.UpdateQuestionAsync(question);
            return question;
        }

        public async Task<bool> DeleteQuestionAsync(int questionId)
        {
            if (questionId <= 0)
                throw new ArgumentException("Question ID must be greater than 0", nameof(questionId));

            var existingQuestion = await _questionRepository.GetQuestionByIdAsync(questionId);
            if (existingQuestion == null)
                return false;

            await _questionRepository.DeleteQuestionAsync(questionId);
            return true;
        }
    }
}