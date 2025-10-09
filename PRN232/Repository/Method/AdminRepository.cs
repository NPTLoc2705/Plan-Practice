using BusinessObject;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Method
{
    public class AdminRepository : IAdminRepository
    {
        private readonly DAL.AdminDAO _adminDAO;

        public AdminRepository(DAL.AdminDAO adminDAO)
        {
            _adminDAO = adminDAO;
        }

        public async Task<List<User>> GetAllUsersByRoleAsync(UserRole? role = null)
        {
            return await _adminDAO.GetAllUsersByRoleAsync(role);
        }

        public async Task<List<User>> GetAllStudentsAsync()
        {
            return await _adminDAO.GetAllStudentsAsync();
        }

        public async Task<List<User>> GetAllTeachersAsync()
        {
            return await _adminDAO.GetAllTeachersAsync();
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _adminDAO.GetUserByIdAsync(userId);
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            return await _adminDAO.UpdateUserAsync(user);
        }

        public async Task<User> ToggleBanUserAsync(int userId, bool isBanned)
        {
            return await _adminDAO.ToggleBanUserAsync(userId, isBanned);
        }

        public async Task<(List<User> users, int totalCount)> GetPaginatedUsersByRoleAsync(UserRole role, int pageNumber, int pageSize, string? searchTerm = null)
        {
            return await _adminDAO.GetPaginatedUsersByRoleAsync(role, pageNumber, pageSize, searchTerm);
        }

        public async Task<(List<User> users, int totalCount)> GetPaginatedUsersAsync(int pageNumber, int pageSize, string? searchTerm = null, UserRole? roleFilter = null)
        {
            return await _adminDAO.GetPaginatedUsersAsync(pageNumber, pageSize, searchTerm, roleFilter);
        }

      
    }
}
