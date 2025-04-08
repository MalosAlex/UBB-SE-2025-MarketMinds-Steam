using System.Diagnostics;
using BusinessLayer.Models;
using BusinessLayer.Exceptions;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Services.Interfaces;

namespace BusinessLayer.Services
{
    public class OwnedGamesService : IOwnedGamesService
    {
        private readonly IOwnedGamesRepository ownedGamesRepository;

        public OwnedGamesService(IOwnedGamesRepository ownedGamesRepository)
        {
            this.ownedGamesRepository = ownedGamesRepository ?? throw new ArgumentNullException(nameof(ownedGamesRepository));
        }

        public List<OwnedGame> GetAllOwnedGames(int userId)
        {
            try
            {
                Debug.WriteLine($"Getting all owned games for user {userId}");
                var ownedGamesList = ownedGamesRepository.GetAllOwnedGames(userId);
                Debug.WriteLine($"Successfully retrieved {ownedGamesList.Count} owned games");
                return ownedGamesList;
            }
            catch (RepositoryException repositoryException)
            {
                Debug.WriteLine($"Repository error: {repositoryException.Message}");
                throw new ServiceException("Failed to retrieve owned games.", repositoryException);
            }
            catch (Exception unexpectedException)
            {
                Debug.WriteLine($"Unexpected error: {unexpectedException.Message}");
                throw new ServiceException("An unexpected error occurred while retrieving owned games.", unexpectedException);
            }
        }

        public OwnedGame GetOwnedGameById(int gameId, int userId)
        {
            try
            {
                Debug.WriteLine($"Getting owned game {gameId} for user {userId}");
                var ownedGame = ownedGamesRepository.GetOwnedGameById(gameId, userId);

                if (ownedGame == null)
                {
                    Debug.WriteLine($"No owned game found with ID {gameId} for user {userId}");
                    return null;
                }

                Debug.WriteLine($"Successfully retrieved owned game {gameId}");
                return ownedGame;
            }
            catch (RepositoryException repositoryException)
            {
                Debug.WriteLine($"Repository error: {repositoryException.Message}");
                throw new ServiceException("Failed to retrieve owned game.", repositoryException);
            }
            catch (Exception unexpectedException)
            {
                Debug.WriteLine($"Unexpected error: {unexpectedException.Message}");
                throw new ServiceException("An unexpected error occurred while retrieving owned game.", unexpectedException);
            }
        }

        public void RemoveOwnedGame(int gameId, int userId)
        {
            try
            {
                Debug.WriteLine($"Removing owned game {gameId} for user {userId}");
                ownedGamesRepository.RemoveOwnedGame(gameId, userId);
                Debug.WriteLine($"Successfully removed owned game {gameId}");
            }
            catch (RepositoryException repositoryException)
            {
                Debug.WriteLine($"Repository error: {repositoryException.Message}");
                throw new ServiceException("Failed to remove owned game.", repositoryException);
            }
            catch (Exception unexpectedException)
            {
                Debug.WriteLine($"Unexpected error: {unexpectedException.Message}");
                throw new ServiceException("An unexpected error occurred while removing owned game.", unexpectedException);
            }
        }
    }
}
