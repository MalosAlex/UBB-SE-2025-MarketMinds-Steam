using BusinessLayer.Models;

namespace BusinessLayer.Services.Interfaces
{
    public interface IOwnedGamesService
    {
        List<OwnedGame> GetAllOwnedGames(int userId);
        OwnedGame GetOwnedGameById(int gameId, int userId);
        void RemoveOwnedGame(int gameId, int userId);
    }
}