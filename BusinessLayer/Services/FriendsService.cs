using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Exceptions;

namespace BusinessLayer.Services
{
    public class FriendsService : IFriendsService
    {
        private readonly IFriendshipsRepository friendshipsRepository;
        private readonly IUserService userService;

        public FriendsService(IFriendshipsRepository friendshipsRepository, IUserService userService)
        {
            friendshipsRepository = friendshipsRepository ?? throw new ArgumentNullException(nameof(friendshipsRepository));
            userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public List<Friendship> GetAllFriendships()
        {
            try
            {
                int currentUserId = userService.GetCurrentUser().UserId;
                return friendshipsRepository.GetAllFriendships(currentUserId);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Error retrieving friendships for user.", ex);
            }
        }

        public void RemoveFriend(int friendshipId)
        {
            try
            {
                friendshipsRepository.RemoveFriendship(friendshipId);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Error removing friend.", ex);
            }
        }

        public int GetFriendshipCount(int userId)
        {
            try
            {
                return friendshipsRepository.GetFriendshipCount(userId);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Error retrieving friendship count.", ex);
            }
        }

        public bool AreUsersFriends(int userId1, int userId2)
        {
            try
            {
                List<Friendship> friendships = friendshipsRepository.GetAllFriendships(userId1);
                bool areFriends = false;
                foreach (Friendship f in friendships)
                {
                    if (f.FriendId == userId2)
                    {
                        areFriends = true;
                        break;
                    }
                }
                return areFriends;
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Error checking friendship status.", ex);
            }
        }

        public int? GetFriendshipId(int userId1, int userId2)
        {
            try
            {
                List<Friendship> friendships = friendshipsRepository.GetAllFriendships(userId1);
                Friendship found = null;
                foreach (Friendship f in friendships)
                {
                    if (f.FriendId == userId2)
                    {
                        found = f;
                        break;
                    }
                }
                return found != null ? new int?(found.FriendshipId) : null;
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Error retrieving friendship ID.", ex);
            }
        }

        public void AddFriend(int userId, int friendId)
        {
            try
            {
                friendshipsRepository.AddFriendship(userId, friendId);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Error adding friend.", ex);
            }
        }
    }
}
