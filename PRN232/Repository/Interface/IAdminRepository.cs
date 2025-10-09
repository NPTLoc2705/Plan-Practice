using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interface
{
    public interface IAdminRepository
    {
        Task<List<User>> GetAllUsersByRoleAsync(UserRole? role = null);
        Task<List<User>> GetAllStudentsAsync();
        Task<List<User>> GetAllTeachersAsync();
        Task<User?> GetUserByIdAsync(int userId);
        Task<User> UpdateUserAsync(User user);
        Task<User> ToggleBanUserAsync(int userId, bool isBanned);
        Task<(List<User> users, int totalCount)> GetPaginatedUsersByRoleAsync(UserRole role, int pageNumber, int pageSize, string? searchTerm = null);
        Task<(List<User> users, int totalCount)> GetPaginatedUsersAsync(int pageNumber, int pageSize, string? searchTerm = null, UserRole? roleFilter = null);
    }
}
