using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Dtos.Auth
{
    public class Authen
    {

    }
    public class RegisterDto
    {
        [Required]
        
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

    }
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
    public class LoginResponse
    {
        public string Token { get; set; }
        public User user { get; set; }
    }
    public class SendOtpRequest
    {
        public string Email { get; set; }
        public OptPurpose Purpose { get; set; }
    }

    public class VerifyOtpRequest
    {
        public string Email { get; set; }
        public string Otp { get; set; }
        // public OptPurpose Purpose { get; set; }
    }

    public class ResendOtpRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        //public OptPurpose Purpose { get; set; }
    }
    public class VerifyRegistrationRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 8)]
        public string Password { get; set; }
        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string Otp { get; set; }
    }
    public class GoogleAuthDto
    {
        [Required]
        public string IdToken { get; set; }
    }
}
