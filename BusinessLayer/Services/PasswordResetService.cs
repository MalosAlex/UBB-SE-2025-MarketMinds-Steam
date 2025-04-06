using System;
using System.IO;
using System.Threading.Tasks;
using BusinessLayer.Exceptions;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Validators;
using BusinessLayer.Repositories;

namespace BusinessLayer.Services
{
    public interface IPasswordResetService
    {
        Task<(bool isValid, string message)> SendResetCode(string email);
        (bool isValid, string message) VerifyResetCode(string email, string code);
        (bool isValid, string message) ResetPassword(string email, string code, string newPassword);
        void CleanupExpiredCodes();
    }

    public class PasswordResetService : IPasswordResetService
    {
        private readonly string resetCodesPath;
        private readonly IUserService userService;
        private readonly PasswordResetValidator validator;
        private readonly PasswordResetRepository passwordResetRepository;

        public PasswordResetService(PasswordResetRepository passwordResetRepository, IUserService userService)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            validator = new PasswordResetValidator();
            resetCodesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ResetCodes");
            Directory.CreateDirectory(resetCodesPath);
            passwordResetRepository = passwordResetRepository ?? throw new ArgumentNullException(nameof(passwordResetRepository));
        }

        public async Task<(bool isValid, string message)> SendResetCode(string email)
        {
            try
            {
                var validationResult = validator.ValidateEmail(email);
                if (!validationResult.isValid)
                {
                    return validationResult;
                }

                CleanupExpiredCodes();

                var code = GenerateResetCode();
                var filePath = GetResetCodeFilePath(email);
                await File.WriteAllTextAsync(filePath, code);

                // Set up auto-deletion after 15 minutes
                _ = Task.Delay(TimeSpan.FromMinutes(15)).ContinueWith(_ =>
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                });

                return (true, "Reset code sent successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to send reset code: {ex.Message}");
            }
        }

        public (bool isValid, string message) VerifyResetCode(string email, string code)
        {
            try
            {
                var emailValidation = validator.ValidateEmail(email);
                if (!emailValidation.isValid)
                {
                    return emailValidation;
                }

                var codeValidation = validator.ValidateResetCode(code);
                if (!codeValidation.isValid)
                {
                    return codeValidation;
                }

                var filePath = GetResetCodeFilePath(email);
                if (!File.Exists(filePath))
                {
                    return (false, "Reset code has expired or does not exist.");
                }

                var storedCode = File.ReadAllText(filePath).Trim();
                return (storedCode == code, storedCode == code ? "Code verified successfully." : "Invalid reset code.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to verify reset code: {ex.Message}");
            }
        }

        public (bool isValid, string message) ResetPassword(string email, string code, string newPassword)
        {
            try
            {
                var verificationResult = VerifyResetCode(email, code);
                if (!verificationResult.isValid)
                {
                    return verificationResult;
                }

                var passwordValidation = validator.ValidatePassword(newPassword);
                if (!passwordValidation.isValid)
                {
                    return passwordValidation;
                }

                var user = userService.GetUserByEmail(email);
                if (user == null)
                {
                    return (false, "User not found.");
                }

                userService.UpdateUserPassword(user.UserId, newPassword);
                File.Delete(GetResetCodeFilePath(email));
                return (true, "Password reset successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to reset password: {ex.Message}");
            }
        }

        public void CleanupExpiredCodes()
        {
            try
            {
                if (!Directory.Exists(resetCodesPath))
                {
                    return;
                }

                var files = Directory.GetFiles(resetCodesPath);
                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    if (DateTime.UtcNow - fileInfo.CreationTimeUtc > TimeSpan.FromMinutes(15))
                    {
                        File.Delete(file);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error but don't throw - this is a cleanup operation
                Console.WriteLine($"Error during cleanup: {ex.Message}");
            }
        }

        private string GenerateResetCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private string GetResetCodeFilePath(string email)
        {
            return Path.Combine(resetCodesPath, $"{email.ToLower()}_reset_code.txt");
        }
    }
}