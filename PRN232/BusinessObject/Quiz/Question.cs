using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Quiz
{
    public class Question
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string Content { get; set; }

        public int QuizId { get; set; }

        [ForeignKey("QuizId")]
        public virtual Quizs Quiz { get; set; }

        // Navigation property: Một câu hỏi có nhiều lựa chọn
        public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}
