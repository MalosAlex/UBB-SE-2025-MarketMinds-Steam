using BusinessLayer.Models;

namespace BusinessLayer.Services.Interfaces
{
    public interface IFriendsService
    {
        List<Friendship> GetAllFriendships();
        void RemoveFriend(int friendshipId);
        int GetFriendshipCount(int userId);
        bool AreUsersFriends(int userId1, int userId2);
        int? GetFriendshipId(int userId1, int userId2);
        void AddFriend(int userId, int friendId);
    }
}