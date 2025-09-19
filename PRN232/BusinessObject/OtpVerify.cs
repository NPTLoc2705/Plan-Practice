using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BusinessObject
{
    public enum OptPurpose
    {
        Register,
        ForgotPassword
    }
    public class OtpVerify
    {
        [Key]
        public int Id { get; set; }
        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; } = string.Empty;
        [Required]
        [StringLength(6)]
        public string? Otp { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiredAt { get; set; }
        public OptPurpose Purpose { get; set; }
        public bool IsUsed { get; set; } = false;
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }


    }
}