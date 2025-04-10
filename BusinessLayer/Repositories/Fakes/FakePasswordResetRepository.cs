using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Repositories.Fakes
{
    public class FakePasswordResetRepository : IPasswordResetRepository
    {
        private class ResetCode
        {
            public int UserId { get; set; }
            public string Email { get; set; }
            public string Code { get; set; }
            public DateTime ExpiryTime { get; set; }
        }

        private readonly List<ResetCode> resetCodes = new();
        private readonly Dictionary<int, string> userEmails = new()
        {
            { 1, "user1@example.com" },
            { 2, "user2@example.com" }
        };

        public void StoreResetCode(int userId, string code, DateTime expiryTime)
        {
            string email = GetEmailForUser(userId);
            if (string.IsNullOrEmpty(email))
            {
                return;
            }
            // Remove any existing code for this user
            resetCodes.RemoveAll(resetCode => resetCode.UserId == userId);

            // Add the new code
            resetCodes.Add(new ResetCode
            {
                UserId = userId,
                Email = email,
                Code = code,
                ExpiryTime = expiryTime
            });
        }

        public bool VerifyResetCode(string email, string code)
        {
            var resetCode = resetCodes.FirstOrDefault(resetCode =>
                resetCode.Email == email &&
                resetCode.Code == code &&
                resetCode.ExpiryTime > DateTime.Now);

            return resetCode != null;
        }

        public bool ResetPassword(string email, string code, string hashedPassword)
        {
            // In a fake implementation, we just verify the code is valid
            if (!VerifyResetCode(email, code))
            {
                return false;
            }

            // Remove the used code
            resetCodes.RemoveAll(resetCode => resetCode.Email == email);
            return true;
        }

        public void CleanupExpiredCodes()
        {
            resetCodes.RemoveAll(resetCode => resetCode.ExpiryTime <= DateTime.Now);
        }

        // Helper method to simulate getting email from userId
        private string GetEmailForUser(int userId)
        {
            return userEmails.TryGetValue(userId, out var email) ? email : null;
        }
    }
}