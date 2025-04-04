using BusinessLayer.Models;
using BusinessLayer.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Repositories.fakes
{
    public class FakeUsersRepository : IUsersRepository
    {
        public void ChangeEmail(int userId, string newEmail)
        {
            // throw error
        }

        public void ChangePassword(int userId, string newPassword)
        {
            // throw error
        }

        public void ChangeUsername(int userId, string newUsername)
        {
            // throw error
        }

        public string CheckUserExists(string email, string username)
        {
            return "exists";
            // idk how the implementation works
        }

        public User CreateUser(User user)
        {
            return user;
        }

        public void DeleteUser(int userId)
        {
            // throw error
        }

        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            User v = new User();
            v.Username = "dante";
            users.Add(v);
            User u = new User();
            u.Username = "virhi";
            users.Add(u);
            return users;
        }

        public User? GetUserByEmail(string email)
        {
            User u = new User();
            u.Email = email;
            u.Username = "a";
            return u;
        }

        public User? GetUserById(int userId)
        {
            User u = new User();
            u.UserId = userId;
            u.Username = "b";
            return u;
        }

        public User? GetUserByUsername(string username)
        {
            User u = new User();
            u.Username = username;
            return u;
        }

        public void UpdateLastLogin(int userId)
        {
            // throw error
        }

        public User UpdateUser(User user)
        {
            return user;
        }

        public User? VerifyCredentials(string emailOrUsername)
        {
            User u = new User();
            u.Email = emailOrUsername;
            return u;
        }
    }
}
