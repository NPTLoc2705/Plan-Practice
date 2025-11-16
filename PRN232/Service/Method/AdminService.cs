using BusinessObject;
using BusinessObject.Dtos.Admin;
using Repository.Interface;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Method
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<List<UserListDto>> GetAllUsersAsync(UserRole? roleFilter = null)
        {
            var users = await _adminRepository.GetAllUsersByRoleAsync(roleFilter);
            return users.Select(u => MapToUserListDto(u)).ToList();
        }

        public async Task<List<UserListDto>> GetAllStudentsAsync()
        {
            var students = await _adminRepository.GetAllStudentsAsync();
            return students.Select(u => MapToUserListDto(u)).ToList();
        }

        public async Task<List<UserListDto>> GetAllTeachersAsync()
        {
            var teachers = await _adminRepository.GetAllTeachersAsync();
            return teachers.Select(u => MapToUserListDto(u)).ToList();
        }

        public async Task<(List<UserListDto> users, int totalCount)> GetPaginatedUsersAsync(
            int pageNumber, int pageSize, string? searchTerm = null, UserRole? roleFilter = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var (users, totalCount) = await _adminRepository.GetPaginatedUsersAsync(
                pageNumber, pageSize, searchTerm, roleFilter);

            var userDtos = users.Select(u => MapToUserListDto(u)).ToList();

            return (userDtos, totalCount);
        }

        public async Task<UserDetailDto> GetUserByIdAsync(int userId)
        {
            var user = await _adminRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            return MapToUserDetailDto(user);
        }

        public async Task<UserDetailDto> UpdateUserAsync(int userId, UpdateUserDto updateDto)
        {
            var user = await _adminRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            // Update username if provided
            if (!string.IsNullOrWhiteSpace(updateDto.Username))
                user.Username = updateDto.Username;

            // Update phone if provided
            if (!string.IsNullOrWhiteSpace(updateDto.Phone))
                user.Phone = updateDto.Phone;

            // Update role if provided
            if (!string.IsNullOrWhiteSpace(updateDto.Role))
            {
                if (Enum.TryParse<UserRole>(updateDto.Role, true, out var parsedRole))
                {
                    // Prevent removing the last admin
                    if (user.Role == UserRole.Admin && parsedRole != UserRole.Admin)
                    {
                        var adminCount = (await _adminRepository.GetAllUsersByRoleAsync(UserRole.Admin)).Count;
                        if (adminCount <= 1)
                            throw new Exception("Cannot change role. At least one admin must exist.");
                    }
                    user.Role = parsedRole;
                }
                else
                {
                    throw new Exception($"Invalid role: {updateDto.Role}");
                }
            }

            var updatedUser = await _adminRepository.UpdateUserAsync(user);

            return MapToUserDetailDto(updatedUser);
        }

        public async Task<string> BanUserAsync(int userId)
        {
            var user = await _adminRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            // Prevent banning admins
            if (user.Role == UserRole.Admin)
                throw new Exception("Cannot ban admin users");

            await _adminRepository.ToggleBanUserAsync(userId, true);
            return $"User {user.Username} has been banned successfully";
        }

        public async Task<string> UnbanUserAsync(int userId)
        {
            var user = await _adminRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            await _adminRepository.ToggleBanUserAsync(userId, false);
            return $"User {user.Username} has been unbanned successfully";
        }

      

        // Helper mapping methods
        private UserListDto MapToUserListDto(User user)
        {
            return new UserListDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role.ToString(),
                CreatedAt = user.Createdat,
                IsBanned = user.IsBanned,
                BannedAt = user.BannedAt,
                EmailVerified = user.EmailVerified
            };
        }

        private UserDetailDto MapToUserDetailDto(User user)
        {
            return new UserDetailDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role.ToString(),
                CreatedAt = user.Createdat,
                IsBanned = user.IsBanned,
                BannedAt = user.BannedAt,
                EmailVerified = user.EmailVerified
            };
        }
    }
}
