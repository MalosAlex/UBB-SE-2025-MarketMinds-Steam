using BusinessLayer.Models;
using BusinessLayer.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using BusinessLayer.Utils;

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
            if (email != "" && username == "")
            {
                return "EMAIL_EXISTS";
            }
            else if (email == "" && username != "")
            {
                return "USERNAME_EXISTS";
            }
            else if (email == "" && username == "")
            {
                return "OTHER_ERROR";
            }
            else return "";
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
            u.Password = PasswordHasher.HashPassword("password");
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
            if (emailOrUsername == null)
                return null;
            User u = new User();
            u.Email = emailOrUsername;
            u.Password = PasswordHasher.HashPassword("password");
            return u;
        }
    }
}
