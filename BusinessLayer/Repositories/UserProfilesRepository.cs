using BusinessLayer.Data;
using BusinessLayer.Models;
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Repositories
{
    public class UserProfilesRepository : IUserProfilesRepository
    {
        private readonly IDataLink _dataLink;

        public UserProfilesRepository(IDataLink dataLink)
        {
            _dataLink = dataLink ?? throw new ArgumentNullException(nameof(dataLink));
        }

        public UserProfile? GetUserProfileByUserId(int userId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId)
                };

                var dataTable = _dataLink.ExecuteReader("GetUserProfileByUserId", parameters);
                return dataTable.Rows.Count > 0 ? MapDataRowToUserProfile(dataTable.Rows[0]) : null;
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException($"Failed to retrieve user profile with ID {userId} from the database.", ex);
            }
        }

        public UserProfile? UpdateProfile(UserProfile profile)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@profile_id", profile.ProfileId),
                    new SqlParameter("@user_id", profile.UserId),
                    new SqlParameter("@profile_picture", (object?)profile.ProfilePicture ?? DBNull.Value),
                    new SqlParameter("@bio", (object?)profile.Bio ?? DBNull.Value)
                };

                var dataTable = _dataLink.ExecuteReader("UpdateUserProfile", parameters);
                return dataTable.Rows.Count > 0 ? MapDataRowToUserProfile(dataTable.Rows[0]) : null;
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException($"Failed to update profile for user {profile.UserId}.", ex);
            }
        }

        public UserProfile? CreateProfile(int userId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId)
                };

                var dataTable = _dataLink.ExecuteReader("CreateUserProfile", parameters);
                return dataTable.Rows.Count > 0 ? MapDataRowToUserProfile(dataTable.Rows[0]) : null;
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException($"Failed to create profile for user {userId}.", ex);
            }
        }

        public void UpdateProfileBio(int user_id, string bio)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", user_id),
                    new SqlParameter("@bio", bio)

                };

                var dataTable = _dataLink.ExecuteReader("UpdateUserProfileBio", parameters);
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException($"Failed to update profile for user {user_id}.", ex);
            }
        }

        public void UpdateProfilePicture(int user_id, string picture)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", user_id),
                    new SqlParameter("@profile_picture", picture)
                };

                var dataTable = _dataLink.ExecuteReader("UpdateUserProfilePicture", parameters);
            }
            catch (DatabaseOperationException ex)
            {
                throw new RepositoryException($"Failed to update profile for user {user_id}.", ex);
            }
        }


        private static UserProfile MapDataRowToUserProfile(DataRow row)
        {
            return new UserProfile
            {
                ProfileId = Convert.ToInt32(row["profile_id"]),
                UserId = Convert.ToInt32(row["user_id"]),
                ProfilePicture = row["profile_picture"] as string,
                Bio = row["bio"] as string,
                LastModified = Convert.ToDateTime(row["last_modified"])
            };
        }
    }
}