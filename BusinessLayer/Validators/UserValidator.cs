using System;
using System.Linq;
using System.Text.RegularExpressions;
using BusinessLayer.Models;

namespace BusinessLayer.Validators
{
    public static class UserValidator
    {
        private const string EmailFormatRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        private const int MinimumPasswordLength = 8;
        private const string SpecialCharacters = "@_.,/%^#$!%*?&";
        private const string InvalidUsernameMessage = "User Username is not valid.";
        private const string InvalidPasswordMessage = "User Password is not valid.";
        private const string InvalidEmailMessage = "User Email is not valid.";

        public static bool IsValidUsername(string username)
        {
            return !string.IsNullOrEmpty(username);
        }

        public static bool IsPasswordValid(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < MinimumPasswordLength)
            {
                return false;
            }

            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasLowerCase = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecialChar = password.Any(ch => SpecialCharacters.Contains(ch));

            return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;
        }

        public static bool IsEmailValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            return Regex.IsMatch(email, EmailFormatRegex);
        }

        public static void ValidateUser(User user)
        {
            if (!IsValidUsername(user.Username))
            {
                throw new InvalidOperationException(InvalidUsernameMessage);
            }

            if (!IsPasswordValid(user.Password))
            {
                throw new InvalidOperationException(InvalidPasswordMessage);
            }

            if (!IsEmailValid(user.Email))
            {
                throw new InvalidOperationException(InvalidEmailMessage);
            }
        }
    }
}
