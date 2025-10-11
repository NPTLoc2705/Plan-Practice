using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Quiz;

namespace BusinessObject
{
    public class QuizOTP
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string OTPCode { get; set; }

        [Required]
        public int QuizId { get; set; }

        [Required]
        public int CreatedByTeacherId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime ExpiresAt { get; set; }

        public bool IsActive { get; set; } = true;

        public int UsageCount { get; set; } = 0;

        public int? MaxUsage { get; set; } // null = unlimited

        // Navigation properties
        [ForeignKey("QuizId")]
        public virtual BusinessObject.Quiz.Quiz Quiz { get; set; }

        [ForeignKey("CreatedByTeacherId")]
        public virtual User CreatedByTeacher { get; set; }

        public virtual ICollection<QuizOTPAccess> AccessLogs { get; set; } = new List<QuizOTPAccess>();
    }
}