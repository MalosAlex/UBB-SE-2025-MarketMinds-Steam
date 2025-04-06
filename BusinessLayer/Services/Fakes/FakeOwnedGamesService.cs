using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;

namespace BusinessLayer.Services.Fakes
{
    public class FakeOwnedGamesService : IOwnedGamesService
    {
        private readonly List<OwnedGame> ownedGames;

        public FakeOwnedGamesService()
        {
            // Seed with some dummy owned games using the proper OwnedGame constructor.
            ownedGames = new List<OwnedGame>
            {
                new OwnedGame(1, "Game A", "Description for Game A", "gamea.jpg") { GameId = 100 },
                new OwnedGame(1, "Game B", "Description for Game B", "gameb.jpg") { GameId = 101 },
                new OwnedGame(2, "Game C", "Description for Game C", "gamec.jpg") { GameId = 102 }
            };
        }

        public List<OwnedGame> GetAllOwnedGames(int userId)
        {
            return ownedGames.Where(game => game.UserId == userId).ToList();
        }

        public OwnedGame GetOwnedGameById(int gameId, int userId)
        {
            return ownedGames.FirstOrDefault(game => game.GameId == gameId && game.UserId == userId);
        }

        public void RemoveOwnedGame(int gameId, int userId)
        {
            var game = ownedGames.FirstOrDefault(g => g.GameId == gameId && g.UserId == userId);
            if (game != null)
            {
                ownedGames.Remove(game);
            }
        }
    }
}