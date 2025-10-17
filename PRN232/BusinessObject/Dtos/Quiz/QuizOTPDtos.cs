using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Dtos.Quiz
{
    public class GenerateOTPDto
    {
        public int QuizId { get; set; }
        public int ExpiryMinutes { get; set; } = 60;
        public int? MaxUsage { get; set; } // null = unlimited
    }

    public class ValidateOTPDto
    {
        [Required]
        [StringLength(10)]
        public string OTPCode { get; set; }
    }

    public class QuizOTPDto
    {
        public int Id { get; set; }
        public string OTPCode { get; set; }
        public int QuizId { get; set; }
        public string QuizTitle { get; set; }
        public string TeacherName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsExpired => DateTime.UtcNow > ExpiresAt;
        public int UsageCount { get; set; }
        public int? MaxUsage { get; set; }
        public string Status
        {
            get
            {
                if (!IsActive) return "Revoked";
                if (IsExpired) return "Expired";
                if (MaxUsage.HasValue && UsageCount >= MaxUsage.Value) return "Max Usage Reached";
                return "Active";
            }
        }
    }

    public class OTPValidationResultDto
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public QuizDetailDto Quiz { get; set; }
    }

    public class OTPAccessLogDto
    {
        public int Id { get; set; }
        public string StudentName { get; set; }
        public int StudentId { get; set; }
        public DateTime AccessedAt { get; set; }
    }
}