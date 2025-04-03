using SteamProfile.Data;
using SteamProfile.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Windows.System;

namespace SteamProfile.Repositories
{
    public class SessionRepository
    {
        private readonly DataLink _dataLink;

        public SessionRepository(DataLink dataLink)
        {
            _dataLink = dataLink ?? throw new ArgumentNullException(nameof(dataLink));
        }

        public SessionDetails CreateSession(int userId)
        {
            // Create parameters for the stored procedure
            var parameters = new SqlParameter[]
            {
                  new SqlParameter("@user_id", userId)
            };

            // Execute the stored procedure
            DataTable result = _dataLink.ExecuteReader("CreateSession", parameters);

            // Process the result
            if (result.Rows.Count > 0)
            {
                DataRow row = result.Rows[0];
                return new SessionDetails
                {
                    SessionId = (Guid)row["session_id"],
                    UserId = (int)row["user_id"],
                    CreatedAt = (DateTime)row["created_at"],
                    ExpiresAt = (DateTime)row["expires_at"]
                };
            }

            throw new InvalidOperationException("Failed to create session");
        }

        public void DeleteUserSessions(int userId)
        {
            var parameters = new SqlParameter[]
            {
                  new SqlParameter("@user_id", userId)
            };

            _dataLink.ExecuteNonQuery("DeleteUserSessions", parameters);
        }

        public void DeleteSession(Guid sessionId)
        {
            var parameters = new SqlParameter[]
            {
                  new SqlParameter("@session_id", sessionId)
            };

            _dataLink.ExecuteNonQuery("DeleteSession", parameters);
        }

        public SessionDetails GetSessionById(Guid sessionId)
        {
            var parameters = new SqlParameter[]
            {
                  new SqlParameter("@session_id", sessionId)
            };

            DataTable result = _dataLink.ExecuteReader("GetSessionById", parameters);

            if (result.Rows.Count > 0)
            {
                DataRow row = result.Rows[0];
                return new SessionDetails
                {
                    SessionId = (Guid)row["session_id"],
                    UserId = (int)row["user_id"],
                    CreatedAt = (DateTime)row["created_at"],
                    ExpiresAt = (DateTime)row["expires_at"]
                };
            }

            return null;
        }

        public UserWithSessionDetails GetUserFromSession(Guid sessionId)
        {
            var parameters = new SqlParameter[]
            {
                  new SqlParameter("@session_id", sessionId)
            };

            DataTable result = _dataLink.ExecuteReader("GetUserFromSession", parameters);

            if (result.Rows.Count > 0)
            {
                DataRow row = result.Rows[0];
                return new UserWithSessionDetails
                {
                    SessionId = sessionId,
                    CreatedAt = (DateTime)row["created_at"],
                    ExpiresAt = (DateTime)row["expires_at"],
                    UserId = (int)row["user_id"],
                    Username = (string)row["username"],
                    Email = (string)row["email"],
                    Developer = (bool)row["developer"],
                    UserCreatedAt = (DateTime)row["created_at"],
                    LastLogin = row["last_login"] == DBNull.Value ? null : (DateTime?)row["last_login"]
                };
            }

            return null;
        }

        public List<Guid> GetExpiredSessions()
        {
            var expiredSessions = new List<Guid>();

            DataTable result = _dataLink.ExecuteReader("GetExpiredSessions", null);

            foreach (DataRow row in result.Rows)
            {
                expiredSessions.Add((Guid)row["session_id"]);
            }

            return expiredSessions;
        }
    }

    // DTOs to transfer data between layers
    public class SessionDetails
    {
        public Guid SessionId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class UserWithSessionDetails
    {
        public Guid SessionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool Developer { get; set; }
        public DateTime UserCreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
    }
}