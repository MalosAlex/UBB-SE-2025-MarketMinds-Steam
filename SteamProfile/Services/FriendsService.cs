using SteamProfile.Models;
using SteamProfile.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SteamProfile.Services
{
    public class FriendsService
    {
        private readonly FriendshipsRepository _friendshipsRepository;
        private readonly UserService _userService;

        public FriendsService(FriendshipsRepository friendshipsRepository, UserService userService)
        {
            _friendshipsRepository = friendshipsRepository ?? throw new ArgumentNullException(nameof(friendshipsRepository));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public List<Friendship> GetAllFriendships()
        {
            try
            {
                return _friendshipsRepository.GetAllFriendships(_userService.GetCurrentUser().UserId);
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
                _friendshipsRepository.RemoveFriendship(friendshipId);
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
                return _friendshipsRepository.GetFriendshipCount(userId);
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
                var friendships = _friendshipsRepository.GetAllFriendships(userId1);
                return friendships.Any(f => f.FriendId == userId2);
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
                var friendships = _friendshipsRepository.GetAllFriendships(userId1);
                var friendship = friendships.FirstOrDefault(f => f.FriendId == userId2);
                return friendship?.FriendshipId;
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
                _friendshipsRepository.AddFriendship(userId, friendId);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Error adding friend.", ex);
            }
        }

        public class ServiceException : Exception
        {
            public ServiceException(string message) : base(message) { }
            public ServiceException(string message, Exception innerException)
                : base(message, innerException) { }
        }
    }
}