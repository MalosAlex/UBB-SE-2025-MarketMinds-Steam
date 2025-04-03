using SteamProfile.Models;
using SteamProfile.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SteamProfile.Services
{
    public class OwnedGamesService
    {
        private readonly OwnedGamesRepository _ownedGamesRepository;

        public OwnedGamesService(OwnedGamesRepository ownedGamesRepository)
        {
            _ownedGamesRepository = ownedGamesRepository ?? throw new ArgumentNullException(nameof(ownedGamesRepository));
        }

        public List<OwnedGame> GetAllOwnedGames(int userId)
        {
            try
            {
                Debug.WriteLine($"Getting all owned games for user {userId}");
                var games = _ownedGamesRepository.GetAllOwnedGames(userId);
                Debug.WriteLine($"Successfully retrieved {games.Count} owned games");
                return games;
            }
            catch (RepositoryException ex)
            {
                Debug.WriteLine($"Repository error: {ex.Message}");
                throw new ServiceException("Failed to retrieve owned games.", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error: {ex.Message}");
                throw new ServiceException("An unexpected error occurred while retrieving owned games.", ex);
            }
        }

        public OwnedGame GetOwnedGameById(int gameId, int userId)
        {
            try
            {
                Debug.WriteLine($"Getting owned game {gameId} for user {userId}");
                var game = _ownedGamesRepository.GetOwnedGameById(gameId, userId);
                if (game == null)
                {
                    Debug.WriteLine($"No owned game found with ID {gameId} for user {userId}");
                    return null;
                }
                Debug.WriteLine($"Successfully retrieved owned game {gameId}");
                return game;
            }
            catch (RepositoryException ex)
            {
                Debug.WriteLine($"Repository error: {ex.Message}");
                throw new ServiceException("Failed to retrieve owned game.", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error: {ex.Message}");
                throw new ServiceException("An unexpected error occurred while retrieving owned game.", ex);
            }
        }

        public void RemoveOwnedGame(int gameId, int userId)
        {
            try
            {
                Debug.WriteLine($"Removing owned game {gameId} for user {userId}");
                _ownedGamesRepository.RemoveOwnedGame(gameId, userId);
                Debug.WriteLine($"Successfully removed owned game {gameId}");
            }
            catch (RepositoryException ex)
            {
                Debug.WriteLine($"Repository error: {ex.Message}");
                throw new ServiceException("Failed to remove owned game.", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error: {ex.Message}");
                throw new ServiceException("An unexpected error occurred while removing owned game.", ex);
            }
        }

        public class ServiceException : Exception
        {
            public ServiceException(string message) : base(message) { }
            public ServiceException(string message, Exception innerException)
                : base(message, innerException) { }
        }
    }
}
