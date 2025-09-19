using BusinessObject;
using DAL;
using Microsoft.EntityFrameworkCore;
using Google.Apis.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly PlantPraticeDbContext  _context;
        public UserRepository(PlantPraticeDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<User> CreateAsync(User user)
        {
            user.Id = 0;
            _context.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users
                 .AnyAsync(u => u.Email == email);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users
                 .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                  .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users
                 .AnyAsync(u => u.Username == username);
        }

        public async Task<User> FindOrCreateUserFromGoogleAsync(GoogleJsonWebSignature.Payload payload)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == payload.Email);

            if (user == null)
            {
                user = new User
                {
                    Username = payload.Name,
                    Email = payload.Email,
                    Password = "", // No password for Google users
                    Role = UserRole.Student, // Default role
                    Createdat = DateTime.UtcNow,
                    EmailVerified = true // Google auth implies verified email
                };

                await CreateAsync(user);
                user = await GetByEmailAsync(payload.Email); // Reload with Role if needed
            }

            return user;
        }

    }

}
