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
        [StringLength(50)]
        public string? Username { get; set; }

        [StringLength(15)]
        [Phone]
        public string? Phone { get; set; }

        // Allow admin to change role
        public UserRole? Role { get; set; }
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