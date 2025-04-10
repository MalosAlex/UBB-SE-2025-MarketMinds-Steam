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
        private const string ParamUserId = "@user_id";
        private const string ParamFriendId = "@friend_id";
        private const string ParamFriendshipIdCamel = "@friendshipId";
        private const string ParamFriendshipIdUnderscore = "@friendship_id";

        // Stored Procedure Names
        private const string SP_GetFriendsForUser = "GetFriendsForUser";
        private const string SP_GetUserById = "GetUserById";
        private const string SP_GetUserProfileByUserId = "GetUserProfileByUserId";
        private const string SP_AddFriend = "AddFriend";
        private const string SP_GetFriendshipById = "GetFriendshipById";
        private const string SP_RemoveFriend = "RemoveFriend";
        private const string SP_GetFriendshipCountForUser = "GetFriendshipCountForUser";
        private const string SP_GetFriendshipId = "GetFriendshipId";

        // Error messages
        private const string Err_GetFriendshipsDb = "Database error while retrieving friendships.";
        private const string Err_GetFriendshipsUnexpected = "An unexpected error occurred while retrieving friendships.";
        private const string Err_AddFriendshipDb = "Database error while adding friendship.";
        private const string Err_AddFriendshipUnexpected = "An unexpected error occurred while adding friendship.";
        private const string Err_UserDoesNotExist = "User with ID {0} does not exist.";
        private const string Err_FriendshipAlreadyExists = "Friendship already exists.";
        private const string Err_GetFriendshipByIdDb = "Database error while retrieving friendship by ID.";
        private const string Err_GetFriendshipByIdUnexpected = "An unexpected error occurred while retrieving friendship by ID.";
        private const string Err_RemoveFriendshipDb = "Database error while removing friendship.";
        private const string Err_RemoveFriendshipUnexpected = "An unexpected error occurred while removing friendship.";
        private const string Err_GetFriendshipCountDb = "Database error while retrieving friendship count.";
        private const string Err_GetFriendshipCountUnexpected = "An unexpected error occurred while retrieving friendship count.";
        private const string Err_GetFriendshipIdDb = "Database error while retrieving friendship ID.";
        private const string Err_GetFriendshipIdUnexpected = "An unexpected error occurred while retrieving friendship ID.";

        // Column Names
        private const string ColFriendshipId = "friendship_id";
        private const string ColUserId = "user_id";
        private const string ColFriendId = "friend_id";
        private const string ColUsername = "username";
        private const string ColProfilePicture = "profile_picture";

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
                    new SqlParameter(ParamUserId, userIdentifier)
                };

                var friendshipDataTable = dataLink.ExecuteReader(SP_GetFriendsForUser, storedProcedureParameters);

                var listOfFriendships = new List<Friendship>();
                foreach (DataRow friendshipDataRow in friendshipDataTable.Rows)
                {
                    var friendship = new Friendship(
                        friendshipId: Convert.ToInt32(friendshipDataRow[ColFriendshipId]),
                        userId: Convert.ToInt32(friendshipDataRow[ColUserId]),
                        friendId: Convert.ToInt32(friendshipDataRow[ColFriendId]));

                    var friendProfileQueryParameters = new SqlParameter[]
                    {
                        new SqlParameter(ParamUserId, friendship.FriendId)
                    };

                    var friendUserProfileDataTable = dataLink.ExecuteReader(SP_GetUserById, friendProfileQueryParameters);
                    if (friendUserProfileDataTable.Rows.Count > 0)
                    {
                        friendship.FriendUsername = friendUserProfileDataTable.Rows[0][ColUsername].ToString();

                        var friendProfilePictureQueryParameters = new SqlParameter[]
                        {
                            new SqlParameter(ParamUserId, friendship.FriendId)
                        };

                        var friendUserProfilePictureDataTable = dataLink.ExecuteReader(SP_GetUserProfileByUserId, friendProfilePictureQueryParameters);
                        if (friendUserProfilePictureDataTable.Rows.Count > 0)
                        {
                            friendship.FriendProfilePicture = friendUserProfilePictureDataTable.Rows[0][ColProfilePicture].ToString();
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
                throw new RepositoryException(Err_GetFriendshipsDb, sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Err_GetFriendshipsUnexpected, generalException);
            }
        }

        public void AddFriendship(int userIdentifier, int friendUserIdentifier)
        {
            try
            {
                var userExistenceCheckParameters = new SqlParameter[]
                {
                    new SqlParameter(ParamUserId, userIdentifier)
                };
                var friendExistenceCheckParameters = new SqlParameter[]
                {
                    new SqlParameter(ParamUserId, friendUserIdentifier)
                };

                var userRecordDataTable = dataLink.ExecuteReader(SP_GetUserById, userExistenceCheckParameters);
                var friendRecordDataTable = dataLink.ExecuteReader(SP_GetUserById, friendExistenceCheckParameters);

                if (userRecordDataTable.Rows.Count == 0)
                {
                    throw new RepositoryException(string.Format(Err_UserDoesNotExist, userIdentifier));
                }

                if (friendRecordDataTable.Rows.Count == 0)
                {
                    throw new RepositoryException(string.Format(Err_UserDoesNotExist, friendUserIdentifier));
                }

                var existingFriendshipsForUser = GetAllFriendships(userIdentifier);
                if (existingFriendshipsForUser.Any(existingFriendship => existingFriendship.FriendId == friendUserIdentifier))
                {
                    throw new RepositoryException(Err_FriendshipAlreadyExists);
                }

                var createFriendshipParameters = new SqlParameter[]
                {
                    new SqlParameter(ParamUserId, userIdentifier),
                    new SqlParameter(ParamFriendId, friendUserIdentifier)
                };
                dataLink.ExecuteNonQuery(SP_AddFriend, createFriendshipParameters);
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException(Err_AddFriendshipDb, sqlException);
            }
            catch (RepositoryException)
            {
                throw;
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Err_AddFriendshipUnexpected, generalException);
            }
        }

        public Friendship GetFriendshipById(int friendshipIdentifier)
        {
            try
            {
                var retrieveFriendshipParameters = new SqlParameter[]
                {
                    new SqlParameter(ParamFriendshipIdCamel, friendshipIdentifier)
                };
                var friendshipDataTable = dataLink.ExecuteReader(SP_GetFriendshipById, retrieveFriendshipParameters);
                return friendshipDataTable.Rows.Count > 0
                    ? MapDataRowToFriendship(friendshipDataTable.Rows[0])
                    : null;
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException(Err_GetFriendshipByIdDb, sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Err_GetFriendshipByIdUnexpected, generalException);
            }
        }

        public void RemoveFriendship(int friendshipIdentifier)
        {
            try
            {
                var deleteFriendshipParameters = new SqlParameter[]
                {
                    new SqlParameter(ParamFriendshipIdUnderscore, friendshipIdentifier)
                };
                dataLink.ExecuteNonQuery(SP_RemoveFriend, deleteFriendshipParameters);
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException(Err_RemoveFriendshipDb, sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Err_RemoveFriendshipUnexpected, generalException);
            }
        }

        public int GetFriendshipCount(int userIdentifier)
        {
            try
            {
                var countQueryParameters = new SqlParameter[]
                {
                    new SqlParameter(ParamUserId, userIdentifier)
                };
                return dataLink.ExecuteScalar<int>(SP_GetFriendshipCountForUser, countQueryParameters);
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException(Err_GetFriendshipCountDb, sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Err_GetFriendshipCountUnexpected, generalException);
            }
        }

        public int? GetFriendshipId(int userIdentifier, int friendIdentifier)
        {
            try
            {
                var retrieveFriendshipIdParameters = new SqlParameter[]
                {
                    new SqlParameter(ParamUserId, userIdentifier),
                    new SqlParameter(ParamFriendId, friendIdentifier)
                };
                var friendshipIdentifierResult = dataLink.ExecuteScalar<int?>(SP_GetFriendshipId, retrieveFriendshipIdParameters);
                return friendshipIdentifierResult;
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException(Err_GetFriendshipIdDb, sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException(Err_GetFriendshipIdUnexpected, generalException);
            }
        }

        private static Friendship MapDataRowToFriendship(DataRow friendshipDataRow)
        {
            return new Friendship(
                friendshipId: Convert.ToInt32(friendshipDataRow[ColFriendshipId]),
                userId: Convert.ToInt32(friendshipDataRow[ColUserId]),
                friendId: Convert.ToInt32(friendshipDataRow[ColFriendId]));
        }
    }
}
