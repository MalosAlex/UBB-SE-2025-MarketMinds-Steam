﻿using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Exceptions;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Services
{
    public class CollectionsService : ICollectionsService
    {
        // Error message constants
        private const string Err_RetrieveCollectionsDb = "Failed to retrieve collections from database";
        private const string Err_RetrieveCollectionsUnexpected = "An unexpected error occurred while retrieving collections";
        private const string Err_RetrieveCollection = "Failed to retrieve collection.";
        private const string Err_RetrieveCollectionUnexpected = "An unexpected error occurred while retrieving collection.";
        private const string Err_RetrieveGamesDb = "Failed to retrieve games from database";
        private const string Err_RetrieveGamesUnexpected = "An unexpected error occurred while retrieving games";
        private const string Err_AddGameToCollection = "Failed to add game to collection";
        private const string Err_Unexpected = "An unexpected error occurred";
        private const string Err_RemoveGameFromCollection = "Failed to remove game from collection.";
        private const string Err_RemoveGameFromCollectionUnexpected = "An unexpected error occurred while removing game from collection.";
        private const string Err_DeleteCollection = "Failed to delete collection";
        private const string Err_CreateCollectionDb = "Failed to create collection in database";
        private const string Err_CreateCollectionUnexpected = "An unexpected error occurred while creating collection";
        private const string Err_UpdateCollectionDb = "Failed to update collection in database";
        private const string Err_UpdateCollectionUnexpected = "An unexpected error occurred while updating collection";
        private const string Err_RetrievePublicCollectionsDb = "Failed to retrieve public collections from database";
        private const string Err_RetrievePublicCollectionsUnexpected = "An unexpected error occurred while retrieving public collections";

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
                throw new ServiceException(Err_RetrieveCollectionsDb, repositoryException);
            }
            catch (Exception generalException)
            {
                throw new ServiceException(Err_RetrieveCollectionsUnexpected, generalException);
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
                throw new ServiceException(Err_RetrieveCollection, repositoryException);
            }
            catch (Exception generalException)
            {
                throw new ServiceException(Err_RetrieveCollectionUnexpected, generalException);
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
                throw new ServiceException(Err_RetrieveGamesDb, repositoryException);
            }
            catch (Exception generalException)
            {
                throw new ServiceException(Err_RetrieveGamesUnexpected, generalException);
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
                throw new ServiceException(Err_AddGameToCollection, repositoryException);
            }
            catch (Exception generalException)
            {
                throw new ServiceException(Err_Unexpected, generalException);
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
                throw new ServiceException(Err_RemoveGameFromCollection, repositoryException);
            }
            catch (Exception generalException)
            {
                throw new ServiceException(Err_RemoveGameFromCollectionUnexpected, generalException);
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
                throw new Exception(Err_DeleteCollection, generalException);
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
                throw new ServiceException(Err_CreateCollectionDb, repositoryException);
            }
            catch (Exception generalException)
            {
                throw new ServiceException(Err_CreateCollectionUnexpected, generalException);
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
                throw new ServiceException(Err_UpdateCollectionDb, repositoryException);
            }
            catch (Exception generalException)
            {
                throw new ServiceException(Err_UpdateCollectionUnexpected, generalException);
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
                throw new ServiceException(Err_RetrievePublicCollectionsDb, repositoryException);
            }
            catch (Exception generalException)
            {
                throw new ServiceException(Err_RetrievePublicCollectionsUnexpected, generalException);
            }
        }

        public List<OwnedGame> GetGamesNotInCollection(int collectionId, int userId)
        {
            var gamesNotInCollection = collectionsRepository.GetGamesNotInCollection(collectionId, userId);
            return gamesNotInCollection;
        }
    }
}
