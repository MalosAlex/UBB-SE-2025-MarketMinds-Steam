using BusinessLayer.Models;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Repositories.Fakes
{
    public class FakeFriendshipsRepository : IFriendshipsRepository
    {
        private readonly List<Friendship> friendships = new();

        public FakeFriendshipsRepository()
        {
            // Seed some dummy friendships for testing.
            friendships.Add(new Friendship(friendshipId: 1, userId: 1, friendId: 2)
            {
                FriendUsername = "Alice",
                FriendProfilePicture = "alice.jpg"
            });
            friendships.Add(new Friendship(friendshipId: 2, userId: 1, friendId: 3)
            {
                FriendUsername = "Bob",
                FriendProfilePicture = "bob.jpg"
            });
        }

        public List<Friendship> GetAllFriendships(int userId)
        {
            return friendships.Where(f => f.UserId == userId).OrderBy(f => f.FriendUsername).ToList();
        }

        public void AddFriendship(int userId, int friendId)
        {
            if (friendships.Any(f => f.UserId == userId && f.FriendId == friendId))
            {
                throw new Exception("Friendship already exists.");
            }

            int newId = friendships.Any() ? friendships.Max(f => f.FriendshipId) + 1 : 1;
            friendships.Add(new Friendship(newId, userId, friendId)
            {
                FriendUsername = $"User{friendId}",
                FriendProfilePicture = $"user{friendId}.jpg"
            });
        }

        public Friendship GetFriendshipById(int friendshipId)
        {
            return friendships.FirstOrDefault(f => f.FriendshipId == friendshipId);
        }

        public void RemoveFriendship(int friendshipId)
        {
            var friendship = friendships.FirstOrDefault(f => f.FriendshipId == friendshipId);
            if (friendship != null)
            {
                friendships.Remove(friendship);
            }
        }

        public int GetFriendshipCount(int userId)
        {
            return friendships.Count(f => f.UserId == userId);
        }

        public int? GetFriendshipId(int userId, int friendId)
        {
            return friendships.FirstOrDefault(f => f.UserId == userId && f.FriendId == friendId)?.FriendshipId;
        }
    }
}
