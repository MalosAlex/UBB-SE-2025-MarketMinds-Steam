using BusinessLayer.Models;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Repositories.Fakes
{
    public class FakeOwnedGamesRepository : IOwnedGamesRepository
    {
        private readonly List<OwnedGame> _ownedGames;

        public FakeOwnedGamesRepository()
        {
            // Seed with some dummy owned games for testing.
            _ownedGames = new List<OwnedGame>
            {
                // Creating OwnedGame using the new constructor, then assigning GameId.
                CreateOwnedGame(1, 1, "Game A", "Description A", "gameA.jpg"),
                CreateOwnedGame(1, 2, "Game B", "Description B", "gameB.jpg"),
                CreateOwnedGame(2, 3, "Game C", "Description C", "gameC.jpg")
            };
        }

        public List<OwnedGame> GetAllOwnedGames(int userId)
        {
            return _ownedGames.Where(g => g.UserId == userId).ToList();
        }

        public OwnedGame GetOwnedGameById(int gameId, int userId)
        {
            return _ownedGames.FirstOrDefault(g => g.GameId == gameId && g.UserId == userId);
        }

        public void RemoveOwnedGame(int gameId, int userId)
        {
            var game = _ownedGames.FirstOrDefault(g => g.GameId == gameId && g.UserId == userId);
            if (game != null)
            {
                _ownedGames.Remove(game);
            }
        }

        // Helper method to create an OwnedGame using the new constructor.
        private OwnedGame CreateOwnedGame(int userId, int gameId, string title, string description, string coverPicture)
        {
            var game = new OwnedGame(userId, title, description, coverPicture);
            game.GameId = gameId;
            return game;
        }
    }
}