using System.Data;
using BusinessLayer.Data;
using BusinessLayer.Models;
using Microsoft.Data.SqlClient;
using BusinessLayer.Exceptions;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Repositories
{
    public class FriendshipsRepository : IFriendshipsRepository
    {
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
                    new SqlParameter("@user_id", userIdentifier)
                };

                var friendshipDataTable = dataLink.ExecuteReader("GetFriendsForUser", storedProcedureParameters);

                var listOfFriendships = new List<Friendship>();
                foreach (DataRow friendshipDataRow in friendshipDataTable.Rows)
                {
                    var friendship = new Friendship(
                        friendshipId: Convert.ToInt32(friendshipDataRow["friendship_id"]),
                        userId: Convert.ToInt32(friendshipDataRow["user_id"]),
                        friendId: Convert.ToInt32(friendshipDataRow["friend_id"]));

                    var friendProfileQueryParameters = new SqlParameter[]
                    {
                        new SqlParameter("@user_id", friendship.FriendId)
                    };
                    var friendUserProfileDataTable = dataLink.ExecuteReader("GetUserById", friendProfileQueryParameters);
                    if (friendUserProfileDataTable.Rows.Count > 0)
                    {
                        friendship.FriendUsername = friendUserProfileDataTable.Rows[0]["username"].ToString();

                        var friendProfilePictureQueryParameters = new SqlParameter[]
                        {
                            new SqlParameter("@user_id", friendship.FriendId)
                        };
                        var friendUserProfilePictureDataTable = dataLink.ExecuteReader("GetUserProfileByUserId", friendProfilePictureQueryParameters);
                        if (friendUserProfilePictureDataTable.Rows.Count > 0)
                        {
                            friendship.FriendProfilePicture = friendUserProfilePictureDataTable.Rows[0]["profile_picture"].ToString();
                        }
                    }

                    listOfFriendships.Add(friendship);
                }

                listOfFriendships = listOfFriendships.OrderBy(friendship => friendship.FriendUsername).ToList();
                return listOfFriendships;
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException("Database error while retrieving friendships.", sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving friendships.", generalException);
            }
        }

        public void AddFriendship(int userIdentifier, int friendUserIdentifier)
        {
            try
            {
                var userExistenceCheckParameters = new SqlParameter[] { new SqlParameter("@user_id", userIdentifier) };
                var friendExistenceCheckParameters = new SqlParameter[] { new SqlParameter("@user_id", friendUserIdentifier) };

                var userRecordDataTable = dataLink.ExecuteReader("GetUserById", userExistenceCheckParameters);
                var friendRecordDataTable = dataLink.ExecuteReader("GetUserById", friendExistenceCheckParameters);

                if (userRecordDataTable.Rows.Count == 0)
                {
                    throw new RepositoryException($"User with ID {userIdentifier} does not exist.");
                }

                if (friendRecordDataTable.Rows.Count == 0)
                {
                    throw new RepositoryException($"User with ID {friendUserIdentifier} does not exist.");
                }

                var existingFriendshipsForUser = GetAllFriendships(userIdentifier);
                if (existingFriendshipsForUser.Any(existingFriendship => existingFriendship.FriendId == friendUserIdentifier))
                {
                    throw new RepositoryException("Friendship already exists.");
                }

                var createFriendshipParameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userIdentifier),
                    new SqlParameter("@friend_id", friendUserIdentifier)
                };
                dataLink.ExecuteNonQuery("AddFriend", createFriendshipParameters);
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException("Database error while adding friendship.", sqlException);
            }
            catch (RepositoryException)
            {
                throw;
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("An unexpected error occurred while adding friendship.", generalException);
            }
        }

        public Friendship GetFriendshipById(int friendshipIdentifier)
        {
            try
            {
                var retrieveFriendshipParameters = new SqlParameter[]
                {
                    new SqlParameter("@friendshipId", friendshipIdentifier)
                };
                var friendshipDataTable = dataLink.ExecuteReader("GetFriendshipById", retrieveFriendshipParameters);
                return friendshipDataTable.Rows.Count > 0
                    ? MapDataRowToFriendship(friendshipDataTable.Rows[0])
                    : null;
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException("Database error while retrieving friendship by ID.", sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving friendship by ID.", generalException);
            }
        }

        public void RemoveFriendship(int friendshipIdentifier)
        {
            try
            {
                var deleteFriendshipParameters = new SqlParameter[]
                {
                    new SqlParameter("@friendship_id", friendshipIdentifier)
                };
                dataLink.ExecuteNonQuery("RemoveFriend", deleteFriendshipParameters);
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException("Database error while removing friendship.", sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("An unexpected error occurred while removing friendship.", generalException);
            }
        }

        public int GetFriendshipCount(int userIdentifier)
        {
            try
            {
                var countQueryParameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userIdentifier)
                };
                return dataLink.ExecuteScalar<int>("GetFriendshipCountForUser", countQueryParameters);
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException("Database error while retrieving friendship count.", sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving friendship count.", generalException);
            }
        }

        public int? GetFriendshipId(int userIdentifier, int friendIdentifier)
        {
            try
            {
                var retrieveFriendshipIdParameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userIdentifier),
                    new SqlParameter("@friend_id", friendIdentifier)
                };
                var friendshipIdentifierResult = dataLink.ExecuteScalar<int?>("GetFriendshipId", retrieveFriendshipIdParameters);
                return friendshipIdentifierResult;
            }
            catch (SqlException sqlException)
            {
                throw new RepositoryException("Database error while retrieving friendship ID.", sqlException);
            }
            catch (Exception generalException)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving friendship ID.", generalException);
            }
        }

        private static Friendship MapDataRowToFriendship(DataRow friendshipDataRow)
        {
            return new Friendship(
                friendshipId: Convert.ToInt32(friendshipDataRow["friendship_id"]),
                userId: Convert.ToInt32(friendshipDataRow["user_id"]),
                friendId: Convert.ToInt32(friendshipDataRow["friend_id"]));
        }
    }
}
