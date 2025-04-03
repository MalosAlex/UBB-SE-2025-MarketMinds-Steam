using SteamProfile.Models;
using SteamProfile.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SteamProfile.Services
{
    public class CollectionsService
    {
        private readonly CollectionsRepository _collectionsRepository;
        private readonly OwnedGamesService _ownedGamesService;

        public CollectionsService(CollectionsRepository collectionsRepository, OwnedGamesService ownedGamesService)
        {
            _collectionsRepository = collectionsRepository ?? throw new ArgumentNullException(nameof(collectionsRepository));
            _ownedGamesService = ownedGamesService ?? throw new ArgumentNullException(nameof(ownedGamesService));
        }

        public CollectionsService(CollectionsRepository collectionsRepository)
        {
            _collectionsRepository = collectionsRepository;
        }

        public List<Collection> GetAllCollections(int userId)
        {
            try
            {
                Debug.WriteLine($"Service: Getting all collections for user {userId}");
                var collections = _collectionsRepository.GetAllCollections(userId);
                Debug.WriteLine($"Service: Retrieved {collections?.Count ?? 0} collections from repository");
                return collections;
            }
            catch (RepositoryException ex)
            {
                Debug.WriteLine($"Service: Repository error getting collections: {ex.Message}");
                Debug.WriteLine($"Service: Stack trace: {ex.StackTrace}");
                throw new ServiceException("Failed to retrieve collections from database", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Service: Unexpected error getting collections: {ex.Message}");
                Debug.WriteLine($"Service: Stack trace: {ex.StackTrace}");
                throw new ServiceException("An unexpected error occurred while retrieving collections", ex);
            }
        }

        public Collection GetCollectionById(int collectionId, int userId)
        {
            try
            {
                Debug.WriteLine($"Getting collection {collectionId}");
                var collection = _collectionsRepository.GetCollectionById(collectionId, userId);
                if (collection == null)
                {
                    Debug.WriteLine($"No collection found with ID {collectionId}");
                    return null;
                }

                // Load games for the collection
                collection.Games = _collectionsRepository.GetGamesInCollection(collectionId, userId);
                Debug.WriteLine($"Successfully retrieved collection {collectionId} with {collection.Games.Count} games");
                return collection;
            }
            catch (RepositoryException ex)
            {
                Debug.WriteLine($"Repository error: {ex.Message}");
                throw new ServiceException("Failed to retrieve collection.", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error: {ex.Message}");
                throw new ServiceException("An unexpected error occurred while retrieving collection.", ex);
            }
        }

        public List<OwnedGame> GetGamesInCollection(int collectionId)
        {
            try
            {
                Debug.WriteLine($"Service: Getting games for collection {collectionId}");
                var games = _collectionsRepository.GetGamesInCollection(collectionId);
                Debug.WriteLine($"Service: Retrieved {games?.Count ?? 0} games from repository");
                return games;
            }
            catch (RepositoryException ex)
            {
                Debug.WriteLine($"Service: Repository error getting games: {ex.Message}");
                Debug.WriteLine($"Service: Stack trace: {ex.StackTrace}");
                throw new ServiceException("Failed to retrieve games from database", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Service: Unexpected error getting games: {ex.Message}");
                Debug.WriteLine($"Service: Stack trace: {ex.StackTrace}");
                throw new ServiceException("An unexpected error occurred while retrieving games", ex);
            }
        }

        public void AddGameToCollection(int collectionId, int gameId, int userId)
        {
            try
            {
                Debug.WriteLine($"Service: Adding game {gameId} to collection {collectionId} for user {userId}");
                _collectionsRepository.AddGameToCollection(collectionId, gameId, userId);
                Debug.WriteLine($"Service: Successfully added game {gameId} to collection {collectionId}");
            }
            catch (CollectionsRepository.RepositoryException ex)
            {
                Debug.WriteLine($"Service: Repository error: {ex.Message}");
                throw new ServiceException("Failed to add game to collection", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Service: Unexpected error: {ex.Message}");
                throw new ServiceException("An unexpected error occurred", ex);
            }
        }

        public void RemoveGameFromCollection(int collectionId, int gameId)
        {
            try
            {
                Debug.WriteLine($"Removing game {gameId} from collection {collectionId}");
                _collectionsRepository.RemoveGameFromCollection(collectionId, gameId);
                Debug.WriteLine($"Successfully removed game {gameId} from collection {collectionId}");
            }
            catch (RepositoryException ex)
            {
                Debug.WriteLine($"Repository error: {ex.Message}");
                throw new ServiceException("Failed to remove game from collection.", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected error: {ex.Message}");
                throw new ServiceException("An unexpected error occurred while removing game from collection.", ex);
            }
        }

        public void DeleteCollection(int collectionId, int userId)
        {
            try
            {
                Debug.WriteLine($"Service: Deleting collection {collectionId} for user {userId}");
                _collectionsRepository.DeleteCollection(collectionId, userId);
                Debug.WriteLine("Service: Collection deleted successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Service: Error deleting collection: {ex.Message}");
                throw new Exception("Failed to delete collection", ex);
            }
        }

        public void CreateCollection(int userId, string name, string coverPicture, bool isPublic, DateOnly createdAt)
        {
            try
            {
                Debug.WriteLine($"Service: Creating collection for user {userId}");
                _collectionsRepository.CreateCollection(userId, name, coverPicture, isPublic, createdAt);
                Debug.WriteLine("Service: Collection created successfully");
            }
            catch (RepositoryException ex)
            {
                Debug.WriteLine($"Service: Repository error creating collection: {ex.Message}");
                Debug.WriteLine($"Service: Stack trace: {ex.StackTrace}");
                throw new ServiceException("Failed to create collection in database", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Service: Unexpected error creating collection: {ex.Message}");
                Debug.WriteLine($"Service: Stack trace: {ex.StackTrace}");
                throw new ServiceException("An unexpected error occurred while creating collection", ex);
            }
        }

        public void UpdateCollection(int collectionId, int userId, string name, string coverPicture, bool isPublic)
        {
            try
            {
                Debug.WriteLine($"Service: Updating collection {collectionId} for user {userId}");
                _collectionsRepository.UpdateCollection(collectionId, userId, name, coverPicture, isPublic);
                Debug.WriteLine("Service: Collection updated successfully");
            }
            catch (RepositoryException ex)
            {
                Debug.WriteLine($"Service: Repository error updating collection: {ex.Message}");
                Debug.WriteLine($"Service: Stack trace: {ex.StackTrace}");
                throw new ServiceException("Failed to update collection in database", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Service: Unexpected error updating collection: {ex.Message}");
                Debug.WriteLine($"Service: Stack trace: {ex.StackTrace}");
                throw new ServiceException("An unexpected error occurred while updating collection", ex);
            }
        }

        public List<Collection> GetPublicCollectionsForUser(int userId)
        {
            try
            {
                Debug.WriteLine($"Service: Getting public collections for user {userId}");
                var collections = _collectionsRepository.GetPublicCollectionsForUser(userId);
                Debug.WriteLine($"Service: Retrieved {collections?.Count ?? 0} public collections from repository");
                return collections;
            }
            catch (RepositoryException ex)
            {
                Debug.WriteLine($"Service: Repository error getting public collections: {ex.Message}");
                Debug.WriteLine($"Service: Stack trace: {ex.StackTrace}");
                throw new ServiceException("Failed to retrieve public collections from database", ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Service: Unexpected error getting public collections: {ex.Message}");
                Debug.WriteLine($"Service: Stack trace: {ex.StackTrace}");
                throw new ServiceException("An unexpected error occurred while retrieving public collections", ex);
            }
        }

        public List<OwnedGame> GetGamesNotInCollection(int collectionId, int userId)
        {
            return _collectionsRepository.GetGamesNotInCollection(collectionId, userId);
        }

        public class ServiceException : Exception
        {
            public ServiceException(string message) : base(message) { }
            public ServiceException(string message, Exception innerException)
                : base(message, innerException) { }
        }
    }
}
