using System;

namespace BusinessLayer.Repositories.Interfaces
{
    public interface IPasswordResetRepository
    {
        void StoreResetCode(string email, string code, DateTime expirationTime);
        bool VerifyResetCode(string email, string code);
        void DeleteResetCode(string email);
        void CleanupExpiredCodes();
    }
}