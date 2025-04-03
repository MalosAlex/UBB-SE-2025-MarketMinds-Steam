using SteamProfile.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SteamProfile.Services
{
    public interface IFriendsService
    {
        Task<List<Friendship>> GetFriendsList(string userId);
        Task AddFriend(string userId, string friendId);
        Task RemoveFriend(string userId, string friendId);
        Task<int> GetFriendshipCount(string userId);
    }
}