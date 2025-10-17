using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject
{
    public class QuizOTPAccess
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OTPId { get; set; }

        [Required]
        public int StudentId { get; set; }

        //[Required]
        public DateTime AccessedAt { get; set; } = DateTime.UtcNow;

        //public string IPAddress { get; set; }

        //public string UserAgent { get; set; }

        // Navigation properties
        [ForeignKey("OTPId")]
        public virtual QuizOTP OTP { get; set; }

        [ForeignKey("StudentId")]
        public virtual User Student { get; set; }
    }
}
