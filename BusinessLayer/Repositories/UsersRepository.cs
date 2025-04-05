using BusinessLayer.Data;
using BusinessLayer.Models;
using BusinessLayer.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Repositories
{
    public sealed class UsersRepository : IUsersRepository
    {
        private readonly IDataLink _dataLink;

        public UsersRepository(IDataLink datalink)
        {
            _dataLink = datalink ?? throw new ArgumentNullException(nameof(datalink));
        }

        public List<User> GetAllUsers()
        {
            try
            {
                var dataTable = _dataLink.ExecuteReader("GetAllUsers");
                return MapDataTableToUsers(dataTable);
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException("Failed to retrieve users from the database.", ex);
            }
        }

        public User? GetUserById(int userId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId)
                };

                var dataTable = _dataLink.ExecuteReader("GetUserById", parameters);
                return dataTable.Rows.Count > 0 ? MapDataRowToUser(dataTable.Rows[0]) : null;
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException($"Failed to retrieve user with ID {userId} from the database.", ex);
            }
        }

        public User UpdateUser(User user)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", user.UserId),
                    new SqlParameter("@email", user.Email),
                    new SqlParameter("@username", user.Username),
                    new SqlParameter("@developer", user.IsDeveloper)
                };

                var dataTable = _dataLink.ExecuteReader("UpdateUser", parameters);
                if (dataTable.Rows.Count == 0)
                {
                    throw new RepositoryException($"User with ID {user.UserId} not found.");
                }

                return MapDataRowToUser(dataTable.Rows[0]);
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException($"Failed to update user with ID {user.UserId}.", ex);
            }
        }

        public User CreateUser(User user)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@email", user.Email),
                    new SqlParameter("@username", user.Username),
                    new SqlParameter("@hashed_password", user.Password),
                    new SqlParameter("@developer", user.IsDeveloper)
                };

                var dataTable = _dataLink.ExecuteReader("CreateUser", parameters);
                if (dataTable.Rows.Count == 0)
                {
                    throw new RepositoryException("Failed to create user.");
                }
                return MapDataRowToUser(dataTable.Rows[0]);
            }
            catch (DatabaseOperationException ex)
            {
                Console.WriteLine($"Error creating user: {ex.Message}");
                throw new RepositoryException("Failed to create user.", ex);
            }
        }

        public void DeleteUser(int userId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId)
                };

                //_dataLink.ExecuteNonQuery("DeleteWallet", parameters);

                _dataLink.ExecuteNonQuery("DeleteUser", parameters);
                // have to delete all their assigned data
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException($"Failed to delete user with ID {userId}.", ex);
            }
        }

        public User? VerifyCredentials(string emailOrUsername)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@EmailOrUsername", emailOrUsername),
                };

                var dataTable = _dataLink.ExecuteReader("GetUserByEmailOrUsername", parameters);
                
                if (dataTable.Rows.Count > 0)
                {
                    var user = MapDataRowToUserWithPassword(dataTable.Rows[0]);
                    return user;
                }
                
                return null;
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException("Failed to verify user credentials.", ex);
            }
        }

        public User? GetUserByEmail(string email)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@email", email)
                };

                var dataTable = _dataLink.ExecuteReader("GetUserByEmail", parameters);
                return dataTable.Rows.Count > 0 ? MapDataRowToUser(dataTable.Rows[0]) : null;
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException($"Failed to retrieve user with email {email}.", ex);
            }
        }

        public User? GetUserByUsername(string username)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@username", username)
                };

                var dataTable = _dataLink.ExecuteReader("GetUserByUsername", parameters);
                return dataTable.Rows.Count > 0 ? MapDataRowToUser(dataTable.Rows[0]) : null;
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException($"Failed to retrieve user with username {username}.", ex);
            }
        }

        public string CheckUserExists(string email, string username)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@email", email),
                    new SqlParameter("@username", username)
                };

                var dataTable = _dataLink.ExecuteReader("CheckUserExists", parameters);
                if (dataTable.Rows.Count > 0)
                {
                    return dataTable.Rows[0]["ErrorType"]?.ToString();
                }
                return null;
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException("Failed to check if user exists.", ex);
            }
        }

        public void ChangeEmail(int userId, string newEmail)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId),
                    new SqlParameter("@newEmail", newEmail)
                };
                _dataLink.ExecuteNonQuery("ChangeEmailForUserId", parameters);
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException($"Failed to change email for user ID {userId}.", ex);
            }
        }
        public void ChangePassword(int userId, string newPassword)
        {
            try
            {

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId),
                    new SqlParameter("@newHashedPassword", PasswordHasher.HashPassword(newPassword))
                };
                _dataLink.ExecuteNonQuery("ChangePassword", parameters);
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException($"Failed to change password for user ID {userId}.", ex);
            }
        }
        public void ChangeUsername(int userId, string newUsername)
        {
            try
            {

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId),
                    new SqlParameter("@newUsername",newUsername)
                };
                _dataLink.ExecuteNonQuery("ChangeUsername", parameters);
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException($"Failed to change username for user ID {userId}.", ex);
            }
        }

        public void UpdateLastLogin(int userId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId)
                };

                _dataLink.ExecuteNonQuery("UpdateLastLogin", parameters);
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException($"Failed to update last login for user ID {userId}.", ex);
            }
        }

        private static List<User> MapDataTableToUsers(DataTable dataTable)
        {
            return dataTable.AsEnumerable()
                .Select(MapDataRowToUser)
                .ToList();
        }

        private static User? MapDataRowToUser(DataRow row)
        {
            if (row["user_id"] == DBNull.Value || 
                row["email"] == DBNull.Value || 
                row["username"] == DBNull.Value)
            {
                return null;
            }

            return new User
            {
                UserId = Convert.ToInt32(row["user_id"]),
                Username = row["username"].ToString(),
                Email = row["email"].ToString(),
                IsDeveloper = row["developer"] != DBNull.Value ? Convert.ToBoolean(row["developer"]) : false,
                CreatedAt = row["created_at"] != DBNull.Value ? Convert.ToDateTime(row["created_at"]) : DateTime.MinValue,
                LastLogin = row["last_login"] != DBNull.Value ? row["last_login"] as DateTime? : null
            };
        }

        private static User? MapDataRowToUserWithPassword(DataRow row)
        {
            if (row["user_id"] == DBNull.Value || 
                row["email"] == DBNull.Value || 
                row["username"] == DBNull.Value || 
                row["hashed_password"] == DBNull.Value)
            {
                return null;
            }

            var user = new User
            {
                UserId = Convert.ToInt32(row["user_id"]),
                Email = row["email"].ToString(),
                Username = row["username"].ToString(),
                IsDeveloper = row["developer"] != DBNull.Value ? Convert.ToBoolean(row["developer"]) : false,
                CreatedAt = row["created_at"] != DBNull.Value ? Convert.ToDateTime(row["created_at"]) : DateTime.MinValue,
                LastLogin = row["last_login"] != DBNull.Value ? row["last_login"] as DateTime? : null,
                Password = row["hashed_password"].ToString()
            };

            return user;
        }
    }

    //public class RepositoryException : Exception
    //{
    //    public RepositoryException(string message) : base(message) { }
    //    public RepositoryException(string message, Exception innerException)
    //        : base(message, innerException) { }
    //}
}