using BusinessObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL
{
    public class AdminDAO
    {
        private readonly PlantPraticeDbContext _context;

        public AdminDAO(PlantPraticeDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Get all users with optional role filter
        public async Task<List<User>> GetAllUsersByRoleAsync(UserRole? role = null)
        {
            var query = _context.Users.AsQueryable();

            if (role.HasValue)
            {
                query = query.Where(u => u.Role == role.Value);
            }

            return await query
                .OrderByDescending(u => u.Createdat)
                .ToListAsync();
        }

        // Get all students
        public async Task<List<User>> GetAllStudentsAsync()
        {
            return await _context.Users
                .Where(u => u.Role == UserRole.Student)
                .OrderByDescending(u => u.Createdat)
                .ToListAsync();
        }

        // Get all teachers
        public async Task<List<User>> GetAllTeachersAsync()
        {
            return await _context.Users
                .Where(u => u.Role == UserRole.Teacher)
                .OrderByDescending(u => u.Createdat)
                .ToListAsync();
        }

        // Get user by ID
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        // Update user
        public async Task<User> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // Ban/Unban user
        public async Task<User> ToggleBanUserAsync(int userId, bool isBanned)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            user.IsBanned = isBanned;
            user.BannedAt = isBanned ? DateTime.UtcNow : null;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // Get paginated users by role
        public async Task<(List<User> users, int totalCount)> GetPaginatedUsersByRoleAsync(
            UserRole role, int pageNumber, int pageSize, string? searchTerm = null)
        {
            var query = _context.Users.Where(u => u.Role == role);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(u =>
                    (u.Username != null && u.Username.Contains(searchTerm)) ||
                    (u.Email != null && u.Email.Contains(searchTerm)) ||
                    (u.Phone != null && u.Phone.Contains(searchTerm)));
            }

            var totalCount = await query.CountAsync();

            var users = await query
                .OrderByDescending(u => u.Createdat)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalCount);
        }

        // Get all users (all roles) with pagination
        public async Task<(List<User> users, int totalCount)> GetPaginatedUsersAsync(
            int pageNumber, int pageSize, string? searchTerm = null, UserRole? roleFilter = null)
        {
            var query = _context.Users.AsQueryable();

            if (roleFilter.HasValue)
            {
                query = query.Where(u => u.Role == roleFilter.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(u =>
                    (u.Username != null && u.Username.Contains(searchTerm)) ||
                    (u.Email != null && u.Email.Contains(searchTerm)) ||
                    (u.Phone != null && u.Phone.Contains(searchTerm)));
            }

            var totalCount = await query.CountAsync();

            var users = await query
                .OrderByDescending(u => u.Createdat)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalCount);
        }

       
    }
}