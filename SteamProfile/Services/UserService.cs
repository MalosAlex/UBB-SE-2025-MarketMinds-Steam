using SteamProfile.Models;
using SteamProfile.Repositories;
using SteamProfile.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using SteamProfile.Utils;

namespace SteamProfile.Services
{
    public class UserService
    {
        private readonly UsersRepository _usersRepository;
        private readonly SessionService _sessionService;

        public UserService(UsersRepository usersRepository, SessionService sessionService)
        {
            _usersRepository = usersRepository ?? throw new ArgumentNullException(nameof(usersRepository));
            _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        }

        public List<User> GetAllUsers()
        {
            return _usersRepository.GetAllUsers();
        }

        public User GetUserById(int userId) => _usersRepository.GetUserById(userId);

        public User GetUserByEmail(string email) => _usersRepository.GetUserByEmail(email);

        public User GetUserByUsername(string username) => _usersRepository.GetUserByUsername(username);
        public void ValidateUserAndEmail(string email, string username)
        {
            // Check if user already exists
            var errorType = _usersRepository.CheckUserExists(email, username);

            if (!string.IsNullOrEmpty(errorType))
            {
                switch (errorType)
                {
                    case "EMAIL_EXISTS":
                        throw new EmailAlreadyExistsException(email);
                    case "USERNAME_EXISTS":
                        throw new UsernameAlreadyTakenException(username);
                    default:
                        throw new UserValidationException($"Unknown validation error: {errorType}");
                }
            }
        }

        public User CreateUser(User user)
        {
            ValidateUserAndEmail(user.Email, user.Username);

            // Hash the password before passing it to the repository
            user.Password = PasswordHasher.HashPassword(user.Password);
            return _usersRepository.CreateUser(user);
        }

        public User UpdateUser(User user)
        {
            return _usersRepository.UpdateUser(user);
        }

        public void DeleteUser(int userId)
        {
            _usersRepository.DeleteUser(userId);

        }
        public bool acceptChanges(int user_id, string givenPassword)
        {
            User user = _usersRepository.GetUserById(user_id);

            if (PasswordHasher.VerifyPassword(givenPassword, user.Password))
            {
                return true;
            }
            return false;
        }

        public void updateUserEmail(int userId, string newEmail)
        {
            _usersRepository.ChangeEmail(userId, newEmail);
        }
        public void updateUserPassword(int userId, string newPassword)
        {
            _usersRepository.ChangePassword(userId, newPassword);
        }
        public void updateUserUsername(int userId, string newUsername)
        {
            _usersRepository.ChangeUsername(userId, newUsername);
        }

        public User? Login(string emailOrUsername, string password)
        {
            var user = _usersRepository.VerifyCredentials(emailOrUsername);
            if (user != null)
            {
                if (PasswordHasher.VerifyPassword(password, user.Password)) // Check the password against the hashed password
                {
                    _sessionService.CreateNewSession(user);

                    // update last login time for user
                    _usersRepository.UpdateLastLogin(user.UserId);
                }
                else
                    return null;
            }
            return user;
        }

        public void Logout()
        {
            _sessionService.EndSession();
        }

        public User? GetCurrentUser()
        {
            return _sessionService.GetCurrentUser();
        }

        public bool IsUserLoggedIn()
        {
            return _sessionService.IsUserLoggedIn();
        }

        internal bool UpdateUserUsername(string username, string currentPassword)
        {
            if (this.VerifyUserPassword(currentPassword))
            {
                _usersRepository.ChangeUsername(GetCurrentUser().UserId, username);
                return true;
            }
            return false;
        }

        internal bool UpdateUserPassword(string password, string currentPassword)
        {
            if (this.VerifyUserPassword(currentPassword))
            {
                _usersRepository.ChangePassword(GetCurrentUser().UserId, password);
                return true;
            }
            return false;
        }

        internal bool UpdateUserEmail(string email, string currentPassword)
        {
            if (this.VerifyUserPassword(currentPassword))
            {
                _usersRepository.ChangeEmail(GetCurrentUser().UserId, email);
                return true;
            }
            return false;
        }

        internal bool VerifyUserPassword(string password)
        {
            string email = this.GetCurrentUser().Email;
            var user = _usersRepository.VerifyCredentials(email);
            if (user != null)
            {
                if (PasswordHasher.VerifyPassword(password, user.Password)) // Check the password against the hashed password
                {
                    return true;
                }
            }
            return false;
        }
    }
}