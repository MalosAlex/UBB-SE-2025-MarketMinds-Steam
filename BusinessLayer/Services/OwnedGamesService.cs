using BusinessLayer.Models;
using BusinessLayer.Exceptions;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Services.Interfaces;

namespace BusinessLayer.Services
{
    public class OwnedGamesService : IOwnedGamesService
    {
        // Error message constants
        private const string Err_RetrieveOwnedGames = "Failed to retrieve owned games.";
        private const string Err_RetrieveOwnedGamesUnexpected = "An unexpected error occurred while retrieving owned games.";
        private const string Err_RetrieveOwnedGame = "Failed to retrieve owned game.";
        private const string Err_RetrieveOwnedGameUnexpected = "An unexpected error occurred while retrieving owned game.";
        private const string Err_RemoveOwnedGame = "Failed to remove owned game.";
        private const string Err_RemoveOwnedGameUnexpected = "An unexpected error occurred while removing owned game.";

        private readonly IOwnedGamesRepository ownedGamesRepository;

        public OwnedGamesService(IOwnedGamesRepository ownedGamesRepository)
        {
            this.ownedGamesRepository = ownedGamesRepository ?? throw new ArgumentNullException(nameof(ownedGamesRepository));
        }

        public List<OwnedGame> GetAllOwnedGames(int userId)
        {
            try
            {
                var ownedGamesList = ownedGamesRepository.GetAllOwnedGames(userId);
                return ownedGamesList;
            }
            catch (RepositoryException repositoryException)
            {
                throw new ServiceException(Err_RetrieveOwnedGames, repositoryException);
            }
            catch (Exception unexpectedException)
            {
                throw new ServiceException(Err_RetrieveOwnedGamesUnexpected, unexpectedException);
            }
        }

        public OwnedGame GetOwnedGameById(int gameId, int userId)
        {
            try
            {
                var ownedGame = ownedGamesRepository.GetOwnedGameById(gameId, userId);

                if (ownedGame == null)
                {
                    return null;
                }

                return ownedGame;
            }
            catch (RepositoryException repositoryException)
            {
                throw new ServiceException(Err_RetrieveOwnedGame, repositoryException);
            }
            catch (Exception unexpectedException)
            {
                throw new ServiceException(Err_RetrieveOwnedGameUnexpected, unexpectedException);
            }
        }

        public void RemoveOwnedGame(int gameId, int userId)
        {
            try
            {
                ownedGamesRepository.RemoveOwnedGame(gameId, userId);
            }
            catch (RepositoryException repositoryException)
            {
                throw new ServiceException(Err_RemoveOwnedGame, repositoryException);
            }
            catch (Exception unexpectedException)
            {
                throw new ServiceException(Err_RemoveOwnedGameUnexpected, unexpectedException);
            }
        }
    }
}
