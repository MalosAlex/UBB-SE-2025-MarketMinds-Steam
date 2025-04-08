using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Exceptions;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Services
{
    public class CollectionsService : ICollectionsService
    {
        private readonly ICollectionsRepository collectionsRepository;

        public CollectionsService(ICollectionsRepository collectionsRepository)
        {
            this.collectionsRepository = collectionsRepository ?? throw new ArgumentNullException(nameof(collectionsRepository));
        }

        public List<Collection> GetAllCollections(int userId)
        {
            try
            {
                var userCollections = collectionsRepository.GetAllCollections(userId);
                return userCollections;
            }
            catch (RepositoryException repositoryException)
            {
                throw new ServiceException("Failed to retrieve collections from database", repositoryException);
            }
            catch (Exception generalException)
            {
                throw new ServiceException("An unexpected error occurred while retrieving collections", generalException);
            }
        }

        public Collection GetCollectionById(int collectionId, int userId)
        {
            try
            {
                var collection = collectionsRepository.GetCollectionById(collectionId, userId);

                // Attach the games to the collection
                collection.Games = collectionsRepository.GetGamesInCollection(collectionId, userId);
                return collection;
            }
            catch (RepositoryException repositoryException)
            {
                throw new ServiceException("Failed to retrieve collection.", repositoryException);
            }
            catch (Exception generalException)
            {
                throw new ServiceException("An unexpected error occurred while retrieving collection.", generalException);
            }
        }

        public List<OwnedGame> GetGamesInCollection(int collectionId)
        {
            try
            {
                var ownedGamesInCollection = collectionsRepository.GetGamesInCollection(collectionId);
                return ownedGamesInCollection;
            }
            catch (RepositoryException repositoryException)
            {
                throw new ServiceException("Failed to retrieve games from database", repositoryException);
            }
            catch (Exception generalException)
            {
                throw new ServiceException("An unexpected error occurred while retrieving games", generalException);
            }
        }

        public void AddGameToCollection(int collectionId, int gameId, int userId)
        {
            try
            {
                collectionsRepository.AddGameToCollection(collectionId, gameId, userId);
            }
            catch (RepositoryException repositoryException)
            {
                throw new ServiceException("Failed to add game to collection", repositoryException);
            }
            catch (Exception generalException)
            {
                throw new ServiceException("An unexpected error occurred", generalException);
            }
        }

        public void RemoveGameFromCollection(int collectionId, int gameId)
        {
            try
            {
                collectionsRepository.RemoveGameFromCollection(collectionId, gameId);
            }
            catch (RepositoryException repositoryException)
            {
                throw new ServiceException("Failed to remove game from collection.", repositoryException);
            }
            catch (Exception generalException)
            {
                throw new ServiceException("An unexpected error occurred while removing game from collection.", generalException);
            }
        }

        public void DeleteCollection(int collectionId, int userId)
        {
            try
            {
                collectionsRepository.DeleteCollection(collectionId, userId);
            }
            catch (Exception generalException)
            {
                throw new Exception("Failed to delete collection", generalException);
            }
        }

        public void CreateCollection(int userId, string name, string coverPicture, bool isPublic, DateOnly createdAt)
        {
            try
            {
                collectionsRepository.CreateCollection(userId, name, coverPicture, isPublic, createdAt);
            }
            catch (RepositoryException repositoryException)
            {
                throw new ServiceException("Failed to create collection in database", repositoryException);
            }
            catch (Exception generalException)
            {
                throw new ServiceException("An unexpected error occurred while creating collection", generalException);
            }
        }

        public void UpdateCollection(int collectionId, int userId, string name, string coverPicture, bool isPublic)
        {
            try
            {
                collectionsRepository.UpdateCollection(collectionId, userId, name, coverPicture, isPublic);
            }
            catch (RepositoryException repositoryException)
            {
                throw new ServiceException("Failed to update collection in database", repositoryException);
            }
            catch (Exception generalException)
            {
                throw new ServiceException("An unexpected error occurred while updating collection", generalException);
            }
        }

        public List<Collection> GetPublicCollectionsForUser(int userId)
        {
            try
            {
                var publicCollections = collectionsRepository.GetPublicCollectionsForUser(userId);
                return publicCollections;
            }
            catch (RepositoryException repositoryException)
            {
                throw new ServiceException("Failed to retrieve public collections from database", repositoryException);
            }
            catch (Exception generalException)
            {
                throw new ServiceException("An unexpected error occurred while retrieving public collections", generalException);
            }
        }

        public List<OwnedGame> GetGamesNotInCollection(int collectionId, int userId)
        {
            var gamesNotInCollection = collectionsRepository.GetGamesNotInCollection(collectionId, userId);
            return gamesNotInCollection;
        }
    }
}
