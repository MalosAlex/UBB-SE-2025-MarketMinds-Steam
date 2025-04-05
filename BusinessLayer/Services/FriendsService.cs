using BusinessLayer.Models;
using BusinessLayer.Repositories;
using BusinessLayer.Services.Interfaces;
using System;
using System.Collections.Generic;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Services
{
    public class FriendsService : IFriendsService
    {
        private readonly IFriendshipsRepository _friendshipsRepository;
        private readonly IUserService _userService;

        public FriendsService(IFriendshipsRepository friendshipsRepository, IUserService userService)
        {
            _friendshipsRepository = friendshipsRepository ?? throw new ArgumentNullException(nameof(friendshipsRepository));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public List<Friendship> GetAllFriendships()
        {
            try
            {
                int currentUserId = _userService.GetCurrentUser().UserId;
                return _friendshipsRepository.GetAllFriendships(currentUserId);
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
                List<Friendship> friendships = _friendshipsRepository.GetAllFriendships(userId1);
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
                List<Friendship> friendships = _friendshipsRepository.GetAllFriendships(userId1);
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
            public ServiceException(string message, Exception innerException) : base(message, innerException) { }
        }
    }
}
