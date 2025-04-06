using BusinessLayer.Repositories;
using System;
using BCrypt.Net;
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
        private readonly PasswordResetRepository _resetRepository;
        private readonly IUserService _userService;

        public PasswordResetService(
            PasswordResetRepository resetRepository,
            IUserService userService)
        {
            _resetRepository = resetRepository;
            _userService = userService;
        }

     
        public string GenerateResetCode(string email)
        {
            var user = _userService.GetUserByEmail(email);
            if (user == null) return null;

            // Generate a random 6-digit code
            var resetCode = new Random().Next(100000, 999999).ToString();
            
            // Store the reset code in the database
            _resetRepository.StoreResetCode(
                user.UserId, 
                resetCode, 
                DateTime.UtcNow.AddMinutes(15)
            );
            
            return resetCode;
        }

        public bool VerifyResetCode(string email, string code)
        {
            return _resetRepository.VerifyResetCode(email, code);
        }

        public bool ResetPassword(string email, string code, string newPassword)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
            return _resetRepository.ResetPassword(email, code, hashedPassword);
        }

        public void CleanupExpiredCodes()
        {
            _resetRepository.CleanupExpiredCodes();
        }
    }
} 