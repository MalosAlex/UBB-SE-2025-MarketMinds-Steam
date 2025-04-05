using BusinessLayer.Data;
using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace BusinessLayer.Repositories
{
    public interface IPasswordResetRepository
    {
        void StoreResetCode(int userId, string code, DateTime expiryTime);
        bool VerifyResetCode(string email, string code);
        bool ResetPassword(string email, string code, string hashedPassword);
        void CleanupExpiredCodes();
    }

    public class PasswordResetRepository : IPasswordResetRepository
    {
        private readonly IDataLink _dataLink;

        public PasswordResetRepository(IDataLink dataLink)
        {
            _dataLink = dataLink ?? throw new ArgumentNullException(nameof(dataLink));
        }

        public void StoreResetCode(int userId, string code, DateTime expiryTime)
        {
            try
            {
                // First, delete any existing reset codes for this user
                DeleteExistingResetCodes(userId);

                // Then store the new reset code
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@userId", userId),
                    new SqlParameter("@resetCode", code),
                    new SqlParameter("@expirationTime", expiryTime)
                };

                _dataLink.ExecuteNonQuery("StorePasswordResetCode", parameters);
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException($"Failed to store reset code for user {userId}.", ex);
            }
        }

        private void DeleteExistingResetCodes(int userId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@userId", userId)
                };

                _dataLink.ExecuteNonQuery("DeleteExistingResetCodes", parameters);
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException($"Failed to delete existing reset codes for user {userId}.", ex);
            }
        }



        public bool VerifyResetCode(string email, string code)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@email", email),
                    new SqlParameter("@resetCode", code)
                };

                // Get the reset code data from database
                DataTable result = _dataLink.ExecuteReader("GetResetCodeData", parameters);

                // Implement business logic in the application layer
                if (result.Rows.Count > 0)
                {
                    DataRow row = result.Rows[0];
                    DateTime expirationTime = (DateTime)row["expiration_time"];
                    bool used = (bool)row["used"];

                    // Check if code is valid, unexpired, and unused
                    return !used && expirationTime > DateTime.UtcNow;
                }

                return false;
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException("Failed to verify reset code.", ex);
            }
        }

        public bool ResetPassword(string email, string code, string hashedPassword)
        {
            try
            {
                // First verify the reset code is valid
                if (!VerifyResetCode(email, code))
                {
                    return false;
                }

                // Get the user ID for the email
                var userIdParams = new SqlParameter[]
                {
                    new SqlParameter("@email", email)
                };
                var userTable = _dataLink.ExecuteReader("GetUserByEmail", userIdParams);

                if (userTable.Rows.Count == 0)
                {
                    return false;
                }

                int userId = (int)userTable.Rows[0]["user_id"];

                // Update the password and remove the reset code
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@userId", userId),
                    new SqlParameter("@resetCode", code),
                    new SqlParameter("@newPassword", hashedPassword)
                };

                var result = _dataLink.ExecuteScalar<int>("UpdatePasswordAndRemoveResetCode", parameters);
                return result == 1;
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException("Failed to reset password.", ex);
            }
        }

        public void CleanupExpiredCodes()
        {
            try
            {
                _dataLink.ExecuteNonQuery("CleanupResetCodes");
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException("Failed to cleanup expired reset codes.", ex);
            }
        }
    }
} 