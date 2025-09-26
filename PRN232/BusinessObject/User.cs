using BusinessObject.Quiz;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string? Username { get; set; }
        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }
        [StringLength(255)]
        public string? Password { get; set; }

        public UserRole Role { get; set; } = UserRole.Student;

        public DateTime Createdat { get; set; } = DateTime.UtcNow;

        public bool EmailVerified { get; set; } = false;
        public virtual ICollection<OtpVerify> OtpVerifies { get; set; } = new List<OtpVerify>();

        public virtual ICollection<QuizResult> QuizResults { get; set; } = new List<QuizResult>();
        
        public virtual ICollection<Quiz.Quiz> CreatedQuizzes { get; set; } = new List<Quiz.Quiz>();
    }
}


