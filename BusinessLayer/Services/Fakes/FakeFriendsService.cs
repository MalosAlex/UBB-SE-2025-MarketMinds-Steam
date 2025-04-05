using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace BusinessLayer.Services.Fakes
{
    public class FakeFriendsService : IFriendsService
    {
        private List<Friendship> _friendships;

        public FakeFriendsService()
        {
            _friendships = new List<Friendship>();
            // Seed with some dummy friendships.
            _friendships.Add(new Friendship(1, 1, 2)
            {
                FriendUsername = "Alice",
                FriendProfilePicture = "alice.jpg"
            });
            _friendships.Add(new Friendship(2, 1, 3)
            {
                FriendUsername = "Bob",
                FriendProfilePicture = "bob.jpg"
            });
        }

        public List<Friendship> GetAllFriendships()
        {
            List<Friendship> result = new List<Friendship>();
            foreach (Friendship f in _friendships)
            {
                if (f.UserId == 1) // Assume current user is 1
                {
                    result.Add(f);
                }
            }
            // Explicit sort using delegate
            result.Sort(delegate(Friendship f1, Friendship f2)
            {
                return string.Compare(f1.FriendUsername, f2.FriendUsername, StringComparison.Ordinal);
            });
            return result;
        }

        public void RemoveFriend(int friendshipId)
        {
            Friendship toRemove = null;
            foreach (Friendship f in _friendships)
            {
                if (f.FriendshipId == friendshipId)
                {
                    toRemove = f;
                    break;
                }
            }
            if (toRemove != null)
            {
                _friendships.Remove(toRemove);
            }
        }

        public int GetFriendshipCount(int userId)
        {
            int count = 0;
            foreach (Friendship f in _friendships)
            {
                if (f.UserId == userId)
                {
                    count++;
                }
            }
            return count;
        }

        public bool AreUsersFriends(int userId1, int userId2)
        {
            bool result = false;
            foreach (Friendship f in _friendships)
            {
                if (f.UserId == userId1 && f.FriendId == userId2)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public int? GetFriendshipId(int userId1, int userId2)
        {
            foreach (Friendship f in _friendships)
            {
                if (f.UserId == userId1 && f.FriendId == userId2)
                {
                    return f.FriendshipId;
                }
            }
            return null;
        }

        public void AddFriend(int userId, int friendId)
        {
            foreach (Friendship f in _friendships)
            {
                if (f.UserId == userId && f.FriendId == friendId)
                {
                    throw new Exception("Friendship already exists.");
                }
            }
            int newId = _friendships.Count > 0 ? _friendships[_friendships.Count - 1].FriendshipId + 1 : 1;
            Friendship newFriendship = new Friendship(newId, userId, friendId)
            {
                FriendUsername = "User" + friendId.ToString(),
                FriendProfilePicture = "user" + friendId.ToString() + ".jpg"
            };
            _friendships.Add(newFriendship);
        }
    }
}
