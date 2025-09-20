using BusinessObject.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.QuizzInterface
{
    public interface IQuizManagementService
    {
        Task<QuizWithDetails> GetQuizWithQuestionsAndAnswersAsync(int quizId);
        Task<QuizWithDetails> CreateCompleteQuizAsync(CreateQuizRequest request);
        Task<QuizResult> SubmitQuizAnswersAsync(SubmitQuizRequest request);
        Task<IEnumerable<QuizResult>> GetUserQuizHistoryAsync(int userId);
        Task<QuizStatistics> GetQuizStatisticsAsync(int quizId);
    }

    public class QuizWithDetails
    {
        public Quiz Quiz { get; set; }
        public List<QuestionWithAnswers> Questions { get; set; } = new List<QuestionWithAnswers>();
    }

    public class QuestionWithAnswers
    {
        public Question Question { get; set; }
        public List<Answer> Answers { get; set; } = new List<Answer>();
    }

    public class CreateQuizRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<CreateQuestionRequest> Questions { get; set; } = new List<CreateQuestionRequest>();
    }

    public class CreateQuestionRequest
    {
        public string Content { get; set; }
        public List<CreateAnswerRequest> Answers { get; set; } = new List<CreateAnswerRequest>();
    }

    public class CreateAnswerRequest
    {
        public string Content { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class SubmitQuizRequest
    {
        public int UserId { get; set; }
        public int QuizId { get; set; }
        public List<UserAnswerSubmission> Answers { get; set; } = new List<UserAnswerSubmission>();
    }

    public class UserAnswerSubmission
    {
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
    }

    public class QuizStatistics
    {
        public int TotalAttempts { get; set; }
        public double AverageScore { get; set; }
        public int HighestScore { get; set; }
        public int LowestScore { get; set; }
        public List<QuestionStatistics> QuestionStats { get; set; } = new List<QuestionStatistics>();
    }

    public class QuestionStatistics
    {
        public int QuestionId { get; set; }
        public string Content { get; set; }
        public int TotalAnswers { get; set; }
        public int CorrectAnswers { get; set; }
        public double CorrectPercentage { get; set; }
    }
}