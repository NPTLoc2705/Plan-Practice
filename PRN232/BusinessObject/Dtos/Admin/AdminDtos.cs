using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Dtos.Admin
{
    // List DTO for displaying users
    public class UserListDto
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsBanned { get; set; }
        public DateTime? BannedAt { get; set; }
        public bool EmailVerified { get; set; }
    }

    // Detail DTO for single user
    public class UserDetailDto
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsBanned { get; set; }
        public DateTime? BannedAt { get; set; }
        public bool EmailVerified { get; set; }
    }

    // Update DTO
    public class UpdateUserDto
    {
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters long")]
        [MaxLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
        public string? Username { get; set; }

        [MaxLength(15, ErrorMessage = "Phone number cannot exceed 15 digits")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Phone number must contain only digits")]
        public string? Phone { get; set; }

        // Allow admin to change role
        [RegularExpression(@"^(Student|Teacher|Admin)$", ErrorMessage = "Role must be Student, Teacher, or Admin")]
        public string? Role { get; set; }
    }

    // Pagination response
    public class PaginatedResponse<T>
    {
        public List<T> Data { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }

    // Statistics DTO
    public class UserStatisticsDto
    {
        public int TotalUsers { get; set; }
        public int Students { get; set; }
        public int Teachers { get; set; }
        public int Admins { get; set; }
        public int BannedUsers { get; set; }
        public int VerifiedUsers { get; set; }
    }
}