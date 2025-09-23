using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Dtos
{
    using System;
    using System.Collections.Generic;

    public class AnswerDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool IsCorrect { get; set; }
        public int QuestionId { get; set; }
    }

    public class QuestionDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int QuizId { get; set; }
        public List<AnswerDto> Answers { get; set; } = new List<AnswerDto>();
    }

    public class QuizDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
        public List<QuizResultDto> QuizResults { get; set; } = new List<QuizResultDto>();
    }

    public class QuizResultDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int QuizId { get; set; }
        public int Score { get; set; }
        public DateTime CompletedAt { get; set; }
        public List<UserAnswerDto> UserAnswers { get; set; } = new List<UserAnswerDto>();
    }

    public class UserAnswerDto
    {
        public int Id { get; set; }
        public int QuizResultId { get; set; }
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
    }
}
