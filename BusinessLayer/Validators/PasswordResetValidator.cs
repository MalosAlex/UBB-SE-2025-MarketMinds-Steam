using System;
using System.Text.RegularExpressions;

namespace BusinessLayer.Validators
{
    public class PasswordResetValidator
    {
        // === Constants ===
        private const int ResetCodeLength = 6;
        private const int MinimumPasswordLength = 8;

        private const string EmailPattern = @"^[a-zA-Z0-9][a-zA-Z0-9._%+-]*@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        private const string ResetCodePattern = @"^\d{6}$";
        private const string UpperCasePattern = @"[A-Z]";
        private const string LowerCasePattern = @"[a-z]";
        private const string DigitPattern = @"[0-9]";
        private const string SpecialCharPattern = @"[!@#$%^&*(),.?""':{}|<>]";

        // === Error Messages ===
        private const string EmailRequiredMessage = "Email address is required.";
        private const string InvalidEmailMessage = "Invalid email format.";

        private const string ResetCodeRequiredMessage = "Reset code is required.";
        private static readonly string InvalidResetCodeMessage = $"Reset code must be a {ResetCodeLength}-digit number.";

        private const string PasswordRequiredMessage = "New password is required.";
        private static readonly string PasswordTooShortMessage = $"Password must be at least {MinimumPasswordLength} characters long.";
        private const string PasswordMissingUpperMessage = "Password must contain at least one uppercase letter.";
        private const string PasswordMissingLowerMessage = "Password must contain at least one lowercase letter.";
        private const string PasswordMissingDigitMessage = "Password must contain at least one digit.";
        private const string PasswordMissingSpecialCharMessage = "Password must contain at least one special character.";

        // === Validation Methods ===
        public (bool isValid, string message) ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return (false, EmailRequiredMessage);
            }

            if (!Regex.IsMatch(email, EmailPattern))
            {
                return (false, InvalidEmailMessage);
            }

            return (true, string.Empty);
        }

        public (bool isValid, string message) ValidateResetCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return (false, ResetCodeRequiredMessage);
            }

            if (!Regex.IsMatch(code, ResetCodePattern))
            {
                return (false, InvalidResetCodeMessage);
            }

            return (true, string.Empty);
        }

        public (bool isValid, string message) ValidatePassword(string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                return (false, PasswordRequiredMessage);
            }

            if (newPassword.Length < MinimumPasswordLength)
            {
                return (false, PasswordTooShortMessage);
            }

            if (!Regex.IsMatch(newPassword, UpperCasePattern))
            {
                return (false, PasswordMissingUpperMessage);
            }

            if (!Regex.IsMatch(newPassword, LowerCasePattern))
            {
                return (false, PasswordMissingLowerMessage);
            }

            if (!Regex.IsMatch(newPassword, DigitPattern))
            {
                return (false, PasswordMissingDigitMessage);
            }

            if (!Regex.IsMatch(newPassword, SpecialCharPattern))
            {
                return (false, PasswordMissingSpecialCharMessage);
            }

            return (true, string.Empty);
        }
    }
}
