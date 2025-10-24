using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Quiz
{
   
    public class QuizResult
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public int QuizId { get; set; }

        public int Score { get; set; }

        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("QuizId")]
        public virtual Quizs Quiz { get; set; }

        // Navigation property: Lưu các câu trả lời của người dùng
        public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
    }
   
   
}
