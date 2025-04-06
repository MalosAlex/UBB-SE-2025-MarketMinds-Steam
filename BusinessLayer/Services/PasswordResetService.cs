using BusinessLayer.Repositories;
using BusinessLayer.Services.Interfaces;

namespace BusinessLayer.Services
{
    public interface IPasswordResetService
    {
        string GenerateResetCode(string email);
        bool VerifyResetCode(string email, string code);
        bool ResetPassword(string email, string code, string newPassword);
        void CleanupExpiredCodes();
    }

    public class PasswordResetService : IPasswordResetService
    {
        private readonly PasswordResetRepository resetRepository;
        private readonly IUserService userService;

        public PasswordResetService(
            PasswordResetRepository resetRepository,
            IUserService userService)
        {
            this.resetRepository = resetRepository;
            this.userService = userService;
        }

        public string GenerateResetCode(string email)
        {
            var user = userService.GetUserByEmail(email);
            if (user == null)
            {
                return null;
            }

            // Generate a random 6-digit code
            var resetCode = new Random().Next(100000, 999999).ToString();

            // Store the reset code in the database
            resetRepository.StoreResetCode(
                user.UserId,
                resetCode,
                DateTime.UtcNow.AddMinutes(15));

            return resetCode;
        }

        public bool VerifyResetCode(string email, string code)
        {
            return resetRepository.VerifyResetCode(email, code);
        }

        public bool ResetPassword(string email, string code, string newPassword)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
            return resetRepository.ResetPassword(email, code, hashedPassword);
        }

        public void CleanupExpiredCodes()
        {
            resetRepository.CleanupExpiredCodes();
        }
    }
}