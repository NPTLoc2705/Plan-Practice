using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Dtos.Quiz
{

    //=============================Request=============================
    public class QuizStudentDto
    {
        public int QuizId {  get; set; }
        public int UserId {  get; set; }
    }

    public class SubmitQuizDto
    {
        public int QuizId { get; set; }
        public List<AnswerSubmitDto> Answers { get; set; }
    }

    public class AnswerSubmitDto
    {
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
    }

    //=============================Response=============================

    public class QuizInfoDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool HasAttempted { get; set; } 
        public QuizResultDto LastResult { get; set; } 
    }

    public class QuizDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<QuestionDto> Questions { get; set; }
        public int? CurrentQuizResultId { get; set; } 
    }

    public class QuestionDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public List<AnswerOptionDto> Answers { get; set; }
    }

    public class AnswerOptionDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        
    }

    public class QuizResultDto
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public string QuizTitle { get; set; }
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public decimal Percentage { get; set; }
        public DateTime CompletedAt { get; set; }
        public List<QuestionResultDto> Details { get; set; }
    }

    public class QuestionResultDto
    {
        public int QuestionId { get; set; }
        public string QuestionContent { get; set; }
        public int? UserAnswerId { get; set; }
        public string UserAnswerContent { get; set; }
        public int CorrectAnswerId { get; set; }
        public string CorrectAnswerContent { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class QuizHistoryDto
    {
        public int ResultId { get; set; }
        public int QuizId { get; set; }
        public string QuizTitle { get; set; }
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public decimal Percentage { get; set; }
        public DateTime CompletedAt { get; set; }
    }

    public class QuizStatisticsDto
    {
        public int QuizId { get; set; }
        public string QuizTitle { get; set; }
        public int TotalAttempts { get; set; }
        public decimal AverageScore { get; set; }
        public int HighestScore { get; set; }
        public int YourBestScore { get; set; }
        public int YourAttempts { get; set; }
    }
}
