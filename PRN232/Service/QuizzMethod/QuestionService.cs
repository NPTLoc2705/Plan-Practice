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

        public async Task CreateQuestionAsync(Question question)
        {
            if (question == null)
                throw new ArgumentNullException(nameof(question));

            if (string.IsNullOrWhiteSpace(question.Content))
                throw new ArgumentException("Question content is required", nameof(question));

            if (question.QuizId <= 0)
                throw new ArgumentException("Quiz ID must be greater than 0", nameof(question));

            await _questionRepository.CreateQuestionAsync(question);
           
        }

        public async Task UpdateQuestionAsync(Question question)
        {
    

            await _questionRepository.UpdateQuestionAsync(question);
            
        }

        public async Task DeleteQuestionAsync(int questionId)
        {
            if (questionId <= 0)
                throw new ArgumentException("Question ID must be greater than 0", nameof(questionId));


            await _questionRepository.DeleteQuestionAsync(questionId);
           
        }
    }
}