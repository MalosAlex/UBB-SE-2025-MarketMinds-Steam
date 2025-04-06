﻿using System.Data;
using BusinessLayer.Data;
using BusinessLayer.Models;
using Microsoft.Data.SqlClient;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Exceptions;

namespace BusinessLayer.Repositories
{
    public class UserProfilesRepository : IUserProfilesRepository
    {
        private readonly IDataLink dataLink;

        public UserProfilesRepository(IDataLink dataLink)
        {
            this.dataLink = dataLink ?? throw new ArgumentNullException(nameof(dataLink));
        }

        public UserProfile? GetUserProfileByUserId(int userId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId)
                };

                var dataTable = dataLink.ExecuteReader("GetUserProfileByUserId", parameters);
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

                var dataTable = dataLink.ExecuteReader("UpdateUserProfile", parameters);
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

                var dataTable = dataLink.ExecuteReader("CreateUserProfile", parameters);
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
                var dataTable = dataLink.ExecuteReader("UpdateUserProfileBio", parameters);
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

                var dataTable = dataLink.ExecuteReader("UpdateUserProfilePicture", parameters);
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