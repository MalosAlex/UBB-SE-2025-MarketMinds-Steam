using System;
using System.Text.RegularExpressions;

namespace BusinessLayer.Validators
{
    public class PasswordResetValidator
    {
        // === Constants ===

        // Lengths
        private const int ResetCodeLength = 6;
        private const int MinimumPasswordLength = 8;

        // Regex patterns
        private const string EmailPattern = @"^[a-zA-Z0-9][a-zA-Z0-9._%+-]*@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        private const string ResetCodePattern = @"^\d{6}$";
        private const string UppercasePattern = @"[A-Z]";
        private const string LowercasePattern = @"[a-z]";
        private const string DigitPattern = @"[0-9]";
        private const string SpecialCharacterPattern = @"[!@#$%^&*(),.?""':{}|<>]";

        // Error messages
        private const string EmailRequiredMessage = "Email address is required.";
        private const string InvalidEmailFormatMessage = "Invalid email format.";
        private const string ResetCodeRequiredMessage = "Reset code is required.";
        private const string InvalidResetCodeMessage = "Reset code must be a 6-digit number.";
        private const string PasswordRequiredMessage = "New password is required.";
        private const string PasswordTooShortMessage = "Password must be at least 8 characters long.";
        private const string PasswordMissingUppercaseMessage = "Password must contain at least one uppercase letter.";
        private const string PasswordMissingLowercaseMessage = "Password must contain at least one lowercase letter.";
        private const string PasswordMissingDigitMessage = "Password must contain at least one digit.";
        private const string PasswordMissingSpecialCharMessage = "Password must contain at least one special character.";

        // === Validation Methods ===
        public (bool isValid, string errorMessage) ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return (false, EmailRequiredMessage);
            }

            if (!Regex.IsMatch(email, EmailPattern))
            {
                return (false, InvalidEmailFormatMessage);
            }

            return (true, string.Empty);
        }

        public (bool isValid, string errorMessage) ValidateResetCode(string code)
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

        public (bool isValid, string errorMessage) ValidatePassword(string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                return (false, PasswordRequiredMessage);
            }

            if (newPassword.Length < MinimumPasswordLength)
            {
                return (false, PasswordTooShortMessage);
            }

            if (!Regex.IsMatch(newPassword, UppercasePattern))
            {
                return (false, PasswordMissingUppercaseMessage);
            }

            if (!Regex.IsMatch(newPassword, LowercasePattern))
            {
                return (false, PasswordMissingLowercaseMessage);
            }

            if (!Regex.IsMatch(newPassword, DigitPattern))
            {
                return (false, PasswordMissingDigitMessage);
            }

            if (!Regex.IsMatch(newPassword, SpecialCharacterPattern))
            {
                return (false, PasswordMissingSpecialCharMessage);
            }

            return (true, string.Empty);
        }
    }
}
