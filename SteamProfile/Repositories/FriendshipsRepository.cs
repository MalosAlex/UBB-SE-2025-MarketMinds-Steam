using SteamProfile.Data;

using SteamProfile.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SteamProfile.Repositories
{
    public class FriendshipsRepository
    {
        private readonly DataLink _dataLink;

        public FriendshipsRepository(DataLink dataLink)
        {
            _dataLink = dataLink ?? throw new ArgumentNullException(nameof(dataLink));
        }

        public List<Friendship> GetAllFriendships(int userId)
        {
            try
            {
                Debug.WriteLine($"Getting friends for user {userId}");
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId)
                };

                Debug.WriteLine("Executing GetFriendsForUser stored procedure");
                var dataTable = _dataLink.ExecuteReader("GetFriendsForUser", parameters);
                Debug.WriteLine($"Got {dataTable.Rows.Count} rows from database");

                var friendships = new List<Friendship>();
                foreach (DataRow row in dataTable.Rows)
                {
                    var friendship = new Friendship
                    {
                        FriendshipId = Convert.ToInt32(row["friendship_id"]),
                        UserId = Convert.ToInt32(row["user_id"]),
                        FriendId = Convert.ToInt32(row["friend_id"])
                    };

                    // Get friend's username and profile picture
                    var friendParams = new SqlParameter[]
                    {
                        new SqlParameter("@user_id", friendship.FriendId)
                    };
                    var friendData = _dataLink.ExecuteReader("GetUserById", friendParams);
                    if (friendData.Rows.Count > 0)
                    {
                        friendship.FriendUsername = friendData.Rows[0]["username"].ToString();
                        // Get profile picture from UserProfiles
                        var profileParams = new SqlParameter[]
                        {
                            new SqlParameter("@user_id", friendship.FriendId)
                        };
                        var profileData = _dataLink.ExecuteReader("GetUserProfileByUserId", profileParams);
                        if (profileData.Rows.Count > 0)
                        {
                            friendship.FriendProfilePicture = profileData.Rows[0]["profile_picture"].ToString();
                        }
                    }

                    friendships.Add(friendship);
                }

                // Sort by username
                friendships = friendships.OrderBy(f => f.FriendUsername).ToList();
                Debug.WriteLine($"Mapped {friendships.Count} friendships");
                return friendships;
            }
            catch (SqlException ex)
            {
                Debug.WriteLine($"SQL Error: {ex.Message}");
                Debug.WriteLine($"Error Number: {ex.Number}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw new RepositoryException("Database error while retrieving friendships.", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected Error: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw new RepositoryException("An unexpected error occurred while retrieving friendships.", ex);
            }
        }

        public void AddFriendship(int userId, int friendId)
        {
            try
            {
                // Validate that users exist
                var userParams = new SqlParameter[] { new SqlParameter("@user_id", userId) };
                var friendParams = new SqlParameter[] { new SqlParameter("@user_id", friendId) };
                
                var userData = _dataLink.ExecuteReader("GetUserById", userParams);
                var friendData = _dataLink.ExecuteReader("GetUserById", friendParams);

                if (userData.Rows.Count == 0)
                    throw new RepositoryException($"User with ID {userId} does not exist.");
                if (friendData.Rows.Count == 0)
                    throw new RepositoryException($"User with ID {friendId} does not exist.");

                // Check if friendship already exists in either direction
                var existingFriendships = GetAllFriendships(userId);
                if (existingFriendships.Any(f => f.FriendId == friendId))
                    throw new RepositoryException("Friendship already exists.");

                // Add the friendship (both directions will be handled by the stored procedure)
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId),
                    new SqlParameter("@friend_id", friendId)
                };
                _dataLink.ExecuteNonQuery("AddFriend", parameters);
            }
            catch (SqlException ex)
            {
                Debug.WriteLine($"SQL Error: {ex.Message}");
                throw new RepositoryException("Database error while adding friendship.", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected Error: {ex.Message}");
                throw new RepositoryException("An unexpected error occurred while adding friendship.", ex);
            }
        }

        public Friendship GetFriendshipById(int friendshipId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@friendshipId", friendshipId)
                };
                var dataTable = _dataLink.ExecuteReader("GetFriendshipById", parameters);
                return dataTable.Rows.Count > 0 ? MapDataRowToFriendship(dataTable.Rows[0]) : null;
            }
            catch (SqlException ex)
            {
                throw new RepositoryException("Database error while retrieving friendship by ID.", ex);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving friendship by ID.", ex);
            }
        }

        public void RemoveFriendship(int friendshipId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@friendship_id", friendshipId)
                };
                _dataLink.ExecuteNonQuery("RemoveFriend", parameters);
            }
            catch (SqlException ex)
            {
                Debug.WriteLine($"SQL Error: {ex.Message}");
                throw new RepositoryException("Database error while removing friendship.", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected Error: {ex.Message}");
                throw new RepositoryException("An unexpected error occurred while removing friendship.", ex);
            }
        }

        public int GetFriendshipCount(int userId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId)
                };
                return _dataLink.ExecuteScalar<int>("GetFriendshipCountForUser", parameters);
            }
            catch (SqlException ex)
            {
                Debug.WriteLine($"SQL Error: {ex.Message}");
                throw new RepositoryException("Database error while retrieving friendship count.", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected Error: {ex.Message}");
                throw new RepositoryException("An unexpected error occurred while retrieving friendship count.", ex);
            }
        }

        public int? GetFriendshipId(int userId, int friendId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId),
                    new SqlParameter("@friend_id", friendId)
                };
                var result = _dataLink.ExecuteScalar<int?>("GetFriendshipId", parameters);
                return result;
            }
            catch (SqlException ex)
            {
                Debug.WriteLine($"SQL Error: {ex.Message}");
                throw new RepositoryException("Database error while retrieving friendship ID.", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected Error: {ex.Message}");
                throw new RepositoryException("An unexpected error occurred while retrieving friendship ID.", ex);
            }
        }

        private static List<Friendship> MapDataTableToFriendships(DataTable dataTable)
        {
            try
            {
                Debug.WriteLine("Starting to map DataTable to Friendships");
                var friendships = dataTable.AsEnumerable().Select(row => new Friendship
                {
                    FriendshipId = Convert.ToInt32(row["friendship_id"]),
                    UserId = Convert.ToInt32(row["user_id"]),
                    FriendId = Convert.ToInt32(row["friend_id"])
                }).ToList();
                Debug.WriteLine($"Successfully mapped {friendships.Count} friendships");
                return friendships;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error mapping DataTable: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        private static Friendship MapDataRowToFriendship(DataRow row)
        {
            return new Friendship
            {
                FriendshipId = Convert.ToInt32(row["friendship_id"]),
                UserId = Convert.ToInt32(row["user_id"]),
                FriendId = Convert.ToInt32(row["friend_id"])
            };
        }
    }

    public class RepositoryException : Exception
    {
        public RepositoryException(string message) : base(message) { }
        public RepositoryException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}