using BusinessLayer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Services.Interfaces
{
    public interface IFriendsService
    {
        Task<List<Friendship>> GetFriendsList(string userId);
        Task AddFriend(string userId, string friendId);
        Task RemoveFriend(string userId, string friendId);
        Task<int> GetFriendshipCount(string userId);
    }
}