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
        private readonly UserDAO userDAO;
        public UserRepository(UserDAO userDAO)
        {
            this.userDAO = userDAO;
        }
        public Task<User> CreateAsync(User user)=> userDAO.CreateAsync(user);



        public Task<bool> EmailExistsAsync(string email) => userDAO.EmailExistsAsync(email);
        

        public Task<User> FindOrCreateUserFromGoogleAsync(GoogleJsonWebSignature.Payload payload) => userDAO.FindOrCreateUserFromGoogleAsync(payload);



        public Task<User?> GetByEmailAsync(string email) => userDAO.GetByEmailAsync(email);



        public Task<User?> GetByUsernameAsync(string username) => userDAO.GetByUsernameAsync(username);



        public Task UpdateAsync(User user)
       => userDAO.UpdateAsync(user);

        public Task<bool> UsernameExistsAsync(string username)
        => userDAO.UsernameExistsAsync(username);
    }

}
