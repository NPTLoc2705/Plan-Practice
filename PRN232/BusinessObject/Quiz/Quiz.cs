using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BusinessObject.Quiz
{
    public class Quiz
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property: Một Quiz có nhiều câu hỏi
        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

        // Navigation property: Một Quiz có nhiều kết quả
        public virtual ICollection<QuizResult> QuizResults { get; set; } = new List<QuizResult>();
    }
}
