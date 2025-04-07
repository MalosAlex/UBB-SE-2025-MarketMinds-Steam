using System.Threading.Tasks;
using BusinessLayer.Repositories.Fakes;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Services.Interfaces;

namespace BusinessLayer.Services.Fakes
{
    public class FakePasswordResetService : IPasswordResetService
    {
        private readonly FakePasswordResetRepository passwordResetRepository;
        private readonly FakeUserService userService;

        public FakePasswordResetService()
        {
            passwordResetRepository = new FakePasswordResetRepository();
            userService = new FakeUserService();
        }

        public async Task<(bool isValid, string message)> SendResetCode(string email)
        {
            try
            {
                var user = userService.GetUserByEmail(email);
                if (user == null)
                {
                    return (false, "User not found");
                }

                var code = GenerateResetCode();
                var expirationTime = DateTime.Now.AddMinutes(30);
                passwordResetRepository.StoreResetCode(user.UserId, code, expirationTime);

                // In a real implementation, this would send an email
                // For testing, we'll just return success
                return (true, "Reset code sent successfully");
            }
            catch (Exception)
            {
                return (false, "Failed to send reset code");
            }
        }

        public (bool isValid, string message) VerifyResetCode(string email, string code)
        {
            try
            {
                if (passwordResetRepository.VerifyResetCode(email, code))
                {
                    return (true, "Code verified successfully");
                }
                return (false, "Invalid or expired code");
            }
            catch (Exception)
            {
                return (false, "Failed to verify code");
            }
        }

        public (bool isValid, string message) ResetPassword(string email, string code, string newPassword)
        {
            try
            {
                if (!passwordResetRepository.VerifyResetCode(email, code))
                {
                    return (false, "Invalid or expired code");
                }

                var user = userService.GetUserByEmail(email);
                if (user == null)
                {
                    return (false, "User not found");
                }

                userService.UpdateUserPassword(user.UserId, newPassword);

                return (true, "Password reset successfully");
            }
            catch (Exception)
            {
                return (false, "Failed to reset password");
            }
        }

        public void CleanupExpiredCodes()
        {
            passwordResetRepository.CleanupExpiredCodes();
        }

        private string GenerateResetCode()
        {
            // Generate a 6-digit code
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}