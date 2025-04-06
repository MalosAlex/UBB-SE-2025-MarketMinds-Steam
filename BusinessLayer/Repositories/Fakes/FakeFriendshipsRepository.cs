using BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLayer.Repositories;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Repositories.Fakes
{
    public class FakeFriendshipsRepository : IFriendshipsRepository
    {
        private readonly List<Friendship> _friendships = new();

        public FakeFriendshipsRepository()
        {
            // Seed some dummy friendships for testing.
            _friendships.Add(new Friendship(friendshipId: 1, userId: 1, friendId: 2)
            {
                FriendUsername = "Alice",
                FriendProfilePicture = "alice.jpg"
            });
            _friendships.Add(new Friendship(friendshipId: 2, userId: 1, friendId: 3)
            {
                FriendUsername = "Bob",
                FriendProfilePicture = "bob.jpg"
            });
        }

        public List<Friendship> GetAllFriendships(int userId)
        {
            return _friendships.Where(f => f.UserId == userId).OrderBy(f => f.FriendUsername).ToList();
        }

        public void AddFriendship(int userId, int friendId)
        {
            if (_friendships.Any(f => f.UserId == userId && f.FriendId == friendId))
                throw new Exception("Friendship already exists.");
            int newId = _friendships.Any() ? _friendships.Max(f => f.FriendshipId) + 1 : 1;
            _friendships.Add(new Friendship(newId, userId, friendId)
            {
                FriendUsername = $"User{friendId}",
                FriendProfilePicture = $"user{friendId}.jpg"
            });
        }

        public Friendship GetFriendshipById(int friendshipId)
        {
            return _friendships.FirstOrDefault(f => f.FriendshipId == friendshipId);
        }

        public void RemoveFriendship(int friendshipId)
        {
            var friendship = _friendships.FirstOrDefault(f => f.FriendshipId == friendshipId);
            if (friendship != null)
                _friendships.Remove(friendship);
        }

        public int GetFriendshipCount(int userId)
        {
            return _friendships.Count(f => f.UserId == userId);
        }

        public int? GetFriendshipId(int userId, int friendId)
        {
            return _friendships.FirstOrDefault(f => f.UserId == userId && f.FriendId == friendId)?.FriendshipId;
        }
    }
}
