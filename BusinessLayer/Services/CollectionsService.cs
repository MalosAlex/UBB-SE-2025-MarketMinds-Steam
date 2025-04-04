﻿using BusinessLayer.Models;
using BusinessLayer.Repositories;
using BusinessLayer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using BusinessLayer.Exceptions;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Repositories;
using BusinessLayer.Exceptions;

namespace BusinessLayer.Services
{
    public class CollectionsService : ICollectionsService
    {
        private readonly ICollectionsRepository _collectionsRepository;
        
        public CollectionsService(ICollectionsRepository collectionsRepository)
        {
            _collectionsRepository = collectionsRepository ?? throw new ArgumentNullException(nameof(collectionsRepository));
        }

        public List<Collection> GetAllCollections(int userId)
        {
            try
            {
                var collections = _collectionsRepository.GetAllCollections(userId);
                return collections;
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Failed to retrieve collections from database", ex);
            }
            catch (Exception ex)
            {
                throw new ServiceException("An unexpected error occurred while retrieving collections", ex);
            }
        }

        public Collection GetCollectionById(int collectionId, int userId)
        {
            try
            {
                var collection = _collectionsRepository.GetCollectionById(collectionId, userId);
                if (collection == null)
                {
                    return null;
                }

                // Load games for the collection.
                collection.Games = _collectionsRepository.GetGamesInCollection(collectionId, userId);
                return collection;
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Failed to retrieve collection.", ex);
            }
            catch (Exception ex)
            {
                throw new ServiceException("An unexpected error occurred while retrieving collection.", ex);
            }
        }

        public List<OwnedGame> GetGamesInCollection(int collectionId)
        {
            try
            {
                var games = _collectionsRepository.GetGamesInCollection(collectionId);
                return games;
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Failed to retrieve games from database", ex);
            }
            catch (Exception ex)
            {
                throw new ServiceException("An unexpected error occurred while retrieving games", ex);
            }
        }

        public void AddGameToCollection(int collectionId, int gameId, int userId)
        {
            try
            {
                _collectionsRepository.AddGameToCollection(collectionId, gameId, userId);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Failed to add game to collection", ex);
            }
            catch (Exception ex)
            {
                throw new ServiceException("An unexpected error occurred", ex);
            }
        }

        public void RemoveGameFromCollection(int collectionId, int gameId)
        {
            try
            {
                _collectionsRepository.RemoveGameFromCollection(collectionId, gameId);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Failed to remove game from collection.", ex);
            }
            catch (Exception ex)
            {
                throw new ServiceException("An unexpected error occurred while removing game from collection.", ex);
            }
        }

        public void DeleteCollection(int collectionId, int userId)
        {
            try
            {
                _collectionsRepository.DeleteCollection(collectionId, userId);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete collection", ex);
            }
        }

        public void CreateCollection(int userId, string name, string coverPicture, bool isPublic, DateOnly createdAt)
        {
            try
            {
                _collectionsRepository.CreateCollection(userId, name, coverPicture, isPublic, createdAt);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Failed to create collection in database", ex);
            }
            catch (Exception ex)
            {
                throw new ServiceException("An unexpected error occurred while creating collection", ex);
            }
        }

        public void UpdateCollection(int collectionId, int userId, string name, string coverPicture, bool isPublic)
        {
            try
            {
                _collectionsRepository.UpdateCollection(collectionId, userId, name, coverPicture, isPublic);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Failed to update collection in database", ex);
            }
            catch (Exception ex)
            {
                throw new ServiceException("An unexpected error occurred while updating collection", ex);
            }
        }

        public List<Collection> GetPublicCollectionsForUser(int userId)
        {
            try
            {
                var collections = _collectionsRepository.GetPublicCollectionsForUser(userId);
                return collections;
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Failed to retrieve public collections from database", ex);
            }
            catch (Exception ex)
            {
                throw new ServiceException("An unexpected error occurred while retrieving public collections", ex);
            }
        }

        public List<OwnedGame> GetGamesNotInCollection(int collectionId, int userId)
        {
            return _collectionsRepository.GetGamesNotInCollection(collectionId, userId);
        }
    }
}
