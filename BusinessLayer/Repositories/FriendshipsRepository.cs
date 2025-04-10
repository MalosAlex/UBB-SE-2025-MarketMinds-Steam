using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using BusinessLayer.Data;
using BusinessLayer.Models;
using Microsoft.Data.SqlClient;
using BusinessLayer.Exceptions;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Repositories
{
    public class FriendshipsRepository : IFriendshipsRepository
    {
        // SQL Parameter Names
        private const string ParameterUserId = "@user_id";
        private const string ParameterFriendId = "@friend_id";
        private const string ParameterFriendshipIdCamel = "@friendshipId";
        private const string ParameterFriendshipIdUnderscore = "@friendship_id";

        // Stored Procedure Names
        private const string StoredProcedure_GetFriendsForUser = "GetFriendsForUser";
        private const string StoredProcedure_GetUserById = "GetUserById";
        private const string StoredProcedure_GetUserProfileByUserId = "GetUserProfileByUserId";
        private const string StoredProcedure_AddFriend = "AddFriend";
        private const string StoredProcedure_GetFriendshipById = "GetFriendshipById";
        private const string StoredProcedure_RemoveFriend = "RemoveFriend";
        private const string StoredProcedure_GetFriendshipCountForUser = "GetFriendshipCountForUser";
        private const string StoredProcedure_GetFriendshipId = "GetFriendshipId";

        // Error messages
        private const string Error_GetFriendshipsDataBase = "Database error while retrieving friendships.";
        private const string Error_GetFriendshipsUnexpected = "An unexpected error occurred while retrieving friendships.";
        private const string Error_AddFriendshipDataBase = "Database error while adding friendship.";
        private const string Error_AddFriendshipUnexpected = "An unexpected error occurred while adding friendship.";
        private const string Error_UserDoesNotExist = "User with ID {0} does not exist.";
        private const string Error_FriendshipAlreadyExists = "Friendship already exists.";
        private const string Error_GetFriendshipByIdDataBase = "Database error while retrieving friendship by ID.";
        private const string Error_GetFriendshipByIdUnexpected = "An unexpected error occurred while retrieving friendship by ID.";
        private const string Error_RemoveFriendshipDataBase = "Database error while removing friendship.";
        private const string Error_RemoveFriendshipUnexpected = "An unexpected error occurred while removing friendship.";
        private const string Error_GetFriendshipCountDataBase = "Database error while retrieving friendship count.";
        private const string Error_GetFriendshipCountUnexpected = "An unexpected error occurred while retrieving friendship count.";
        private const string Error_GetFriendshipIdDataBase = "Database error while retrieving friendship ID.";
        private const string Error_GetFriendshipIdUnexpected = "An unexpected error occurred while retrieving friendship ID.";

        // Column Names
        private const string ColumnFriendshipId = "friendship_id";
        private const string ColumnUserId = "user_id";
        private const string ColumnFriendId = "friend_id";
        private const string ColumnUsername = "username";
        private const string ColumnProfilePicture = "profile_picture";

        private readonly IDataLink dataLink;

        public FriendshipsRepository(IDataLink dataLink)
        {
            this.dataLink = dataLink ?? throw new ArgumentNullException(nameof(dataLink));
        }

        public List<Friendship> GetAllFriendships(int userIdentifier)
        {
            try
            {
                var storedProcedureParameters = new SqlParameter[]
                {
                    new SqlParameter(ParameterUserId, userIdentifier)
                };

                var friendshipDataTable = dataLink.ExecuteReader(StoredProcedure_GetFriendsForUser, storedProcedureParameters);

                var listOfFriendships = new List<Friendship>();
                foreach (DataRow friendshipDataRow in friendshipDataTable.Rows)
                {
                    var friendship = new Friendship(
                        friendshipId: Convert.ToInt32(friendshipDataRow[ColumnFriendshipId]),
                        userId: Convert.ToInt32(friendshipDataRow[ColumnUserId]),
                        friendId: Convert.ToInt32(friendshipDataRow[ColumnFriendId]));

                    var friendProfileQueryParameters = new SqlParameter[]
                    {
                        new SqlParameter(ParameterUserId, friendship.FriendId)
                    };

                    var friendUserProfileDataTable = dataLink.ExecuteReader(StoredProcedure_GetUserById, friendProfileQueryParameters);
                    if (friendUserProfileDataTable.Rows.Count > 0)
                    {
                        friendship.FriendUsername = friendUserProfileDataTable.Rows[0][ColumnUsername].ToString();

                        var friendProfilePictureQueryParameters = new SqlParameter[]
                        {
                            new SqlParameter(ParameterUserId, friendship.FriendId)
                        };

                        var friendUserProfilePictureDataTable = dataLink.ExecuteReader(StoredProcedure_GetUserProfileByUserId, friendProfilePictureQueryParameters);
                        if (friendUserProfilePictureDataTable.Rows.Count > 0)
                        {
                            friendship.FriendProfilePicture = friendUserProfilePictureDataTable.Rows[0][ColumnProfilePicture].ToString();
                        }
                    }

                    listOfFriendships.Add(friendship);
                }

                listOfFriendships = listOfFriendships
                    .OrderBy(friendship => friendship.FriendUsername)
                    .ToList();

                return listOfFriendships;
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException(Error_GetFriendshipsDataBase, sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Error_GetFriendshipsUnexpected, generalException);
            }
        }

        public void AddFriendship(int userIdentifier, int friendUserIdentifier)
        {
            try
            {
                var userExistenceCheckParameters = new SqlParameter[]
                {
                    new SqlParameter(ParameterUserId, userIdentifier)
                };
                var friendExistenceCheckParameters = new SqlParameter[]
                {
                    new SqlParameter(ParameterUserId, friendUserIdentifier)
                };

                var userRecordDataTable = dataLink.ExecuteReader(StoredProcedure_GetUserById, userExistenceCheckParameters);
                var friendRecordDataTable = dataLink.ExecuteReader(StoredProcedure_GetUserById, friendExistenceCheckParameters);

                if (userRecordDataTable.Rows.Count == 0)
                {
                    throw new RepositoryException(string.Format(Error_UserDoesNotExist, userIdentifier));
                }

                if (friendRecordDataTable.Rows.Count == 0)
                {
                    throw new RepositoryException(string.Format(Error_UserDoesNotExist, friendUserIdentifier));
                }

                var existingFriendshipsForUser = GetAllFriendships(userIdentifier);
                if (existingFriendshipsForUser.Any(existingFriendship => existingFriendship.FriendId == friendUserIdentifier))
                {
                    throw new RepositoryException(Error_FriendshipAlreadyExists);
                }

                var createFriendshipParameters = new SqlParameter[]
                {
                    new SqlParameter(ParameterUserId, userIdentifier),
                    new SqlParameter(ParameterFriendId, friendUserIdentifier)
                };
                dataLink.ExecuteNonQuery(StoredProcedure_AddFriend, createFriendshipParameters);
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException(Error_AddFriendshipDataBase, sqlException);
            }
            catch (RepositoryException)
            {
                throw;
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Error_AddFriendshipUnexpected, generalException);
            }
        }

        public Friendship GetFriendshipById(int friendshipIdentifier)
        {
            try
            {
                var retrieveFriendshipParameters = new SqlParameter[]
                {
                    new SqlParameter(ParameterFriendshipIdCamel, friendshipIdentifier)
                };
                var friendshipDataTable = dataLink.ExecuteReader(StoredProcedure_GetFriendshipById, retrieveFriendshipParameters);
                return friendshipDataTable.Rows.Count > 0
                    ? MapDataRowToFriendship(friendshipDataTable.Rows[0])
                    : null;
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException(Error_GetFriendshipByIdDataBase, sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Error_GetFriendshipByIdUnexpected, generalException);
            }
        }

        public void RemoveFriendship(int friendshipIdentifier)
        {
            try
            {
                var deleteFriendshipParameters = new SqlParameter[]
                {
                    new SqlParameter(ParameterFriendshipIdUnderscore, friendshipIdentifier)
                };
                dataLink.ExecuteNonQuery(StoredProcedure_RemoveFriend, deleteFriendshipParameters);
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException(Error_RemoveFriendshipDataBase, sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Error_RemoveFriendshipUnexpected, generalException);
            }
        }

        public int GetFriendshipCount(int userIdentifier)
        {
            try
            {
                var countQueryParameters = new SqlParameter[]
                {
                    new SqlParameter(ParameterUserId, userIdentifier)
                };
                return dataLink.ExecuteScalar<int>(StoredProcedure_GetFriendshipCountForUser, countQueryParameters);
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException(Error_GetFriendshipCountDataBase, sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Error_GetFriendshipCountUnexpected, generalException);
            }
        }

        public int? GetFriendshipId(int userIdentifier, int friendIdentifier)
        {
            try
            {
                var retrieveFriendshipIdParameters = new SqlParameter[]
                {
                    new SqlParameter(ParameterUserId, userIdentifier),
                    new SqlParameter(ParameterFriendId, friendIdentifier)
                };
                var friendshipIdentifierResult = dataLink.ExecuteScalar<int?>(StoredProcedure_GetFriendshipId, retrieveFriendshipIdParameters);
                return friendshipIdentifierResult;
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException(Error_GetFriendshipIdDataBase, sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Error_GetFriendshipIdUnexpected, generalException);
            }
        }

        private static Friendship MapDataRowToFriendship(DataRow friendshipDataRow)
        {
            return new Friendship(
                friendshipId: Convert.ToInt32(friendshipDataRow[ColumnFriendshipId]),
                userId: Convert.ToInt32(friendshipDataRow[ColumnUserId]),
                friendId: Convert.ToInt32(friendshipDataRow[ColumnFriendId]));
        }
    }
}
