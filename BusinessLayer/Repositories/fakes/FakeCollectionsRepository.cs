using BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLayer.Repositories;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Repositories.Fakes
{
    public class FakeCollectionsRepository : ICollectionsRepository
    {
        private readonly List<Collection> _collections = new();

        public FakeCollectionsRepository()
        {
            // Seed with some dummy collections for testing.
            _collections.Add(new Collection(userId: 1, name: "Test Collection 1", createdAt: DateOnly.FromDateTime(DateTime.Now.AddDays(-5)), coverPicture: "pic1.jpg", isPublic: true) { CollectionId = 1 });
            _collections.Add(new Collection(userId: 1, name: "Test Collection 2", createdAt: DateOnly.FromDateTime(DateTime.Now.AddDays(-3)), coverPicture: "pic2.jpg", isPublic: false) { CollectionId = 2 });
            _collections.Add(new Collection(userId: 1, name: "Test Collection 3", createdAt: DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), coverPicture: "pic3.jpg", isPublic: true) { CollectionId = 3 });
            _collections.Add(new Collection(userId: 2, name: "Other User Collection", createdAt: DateOnly.FromDateTime(DateTime.Now.AddDays(-2)), coverPicture: "pic4.jpg", isPublic: true) { CollectionId = 4 });
        }

        public List<Collection> GetAllCollections(int userId)
        {
            return _collections.Where(c => c.UserId == userId).ToList();
        }

        public List<Collection> GetLastThreeCollectionsForUser(int userId)
        {
            return GetAllCollections(userId)
                .OrderByDescending(c => c.CreatedAt)
                .Take(3)
                .ToList();
        }

        public Collection GetCollectionById(int collectionId, int userId)
        {
            return _collections.FirstOrDefault(c => c.CollectionId == collectionId && c.UserId == userId);
        }

        public List<OwnedGame> GetGamesInCollection(int collectionId)
        {
            // For testing, return an empty list.
            return new List<OwnedGame>();
        }

        public List<OwnedGame> GetGamesInCollection(int collectionId, int userId)
        {
            // If collectionId==1, we mimic returning all games for the user.
            if (collectionId == 1)
            {
                return new List<OwnedGame>
                {
                    new OwnedGame { GameId = 1, UserId = userId, Title = "Game A", Description = "Desc A", CoverPicture = "gameA.jpg" },
                    new OwnedGame { GameId = 2, UserId = userId, Title = "Game B", Description = "Desc B", CoverPicture = "gameB.jpg" }
                };
            }
            else
            {
                return GetGamesInCollection(collectionId);
            }
        }

        public void AddGameToCollection(int collectionId, int gameId, int userId)
        {
            // No-op for fake.
        }

        public void RemoveGameFromCollection(int collectionId, int gameId)
        {
            // No-op for fake.
        }

        public void MakeCollectionPrivateForUser(string userId, string collectionId)
        {
            // For fake, simply do nothing.
        }

        public void MakeCollectionPublicForUser(string userId, string collectionId)
        {
            // For fake, simply do nothing.
        }

        public void RemoveCollectionForUser(string userId, string collectionId)
        {
            int uid = int.Parse(userId);
            int cid = int.Parse(collectionId);
            var col = _collections.FirstOrDefault(c => c.CollectionId == cid && c.UserId == uid);
            if (col != null)
                _collections.Remove(col);
        }

        public void SaveCollection(string userId, Collection collection)
        {
            int uid = int.Parse(userId);
            if (collection.CollectionId == 0)
            {
                // Mimic creating new collection
                collection.CollectionId = _collections.Any() ? _collections.Max(c => c.CollectionId) + 1 : 1;
                collection.UserId = uid;
                _collections.Add(collection);
            }
            else
            {
                // Mimic update: remove the old version and add new one.
                var existing = _collections.FirstOrDefault(c => c.CollectionId == collection.CollectionId && c.UserId == uid);
                if (existing != null)
                {
                    _collections.Remove(existing);
                    _collections.Add(collection);
                }
            }
        }

        public void DeleteCollection(int collectionId, int userId)
        {
            var col = _collections.FirstOrDefault(c => c.CollectionId == collectionId && c.UserId == userId);
            if (col != null)
            {
                _collections.Remove(col);
            }
        }

        public void CreateCollection(int userId, string name, string coverPicture, bool isPublic, DateOnly createdAt)
        {
            var collection = new Collection(userId, name, createdAt, coverPicture, isPublic)
            {
                CollectionId = _collections.Any() ? _collections.Max(c => c.CollectionId) + 1 : 1
            };
            _collections.Add(collection);
        }

        public void UpdateCollection(int collectionId, int userId, string name, string coverPicture, bool isPublic)
        {
            var col = _collections.FirstOrDefault(c => c.CollectionId == collectionId && c.UserId == userId);
            if (col != null)
            {
                // For simplicity, we replace properties.
                col.Name = name;
                col.CoverPicture = coverPicture;
                col.IsPublic = isPublic;
                // Update createdAt to now (as in original implementation)
                col.CreatedAt = DateOnly.FromDateTime(DateTime.Now);
            }
        }

        public List<Collection> GetPublicCollectionsForUser(int userId)
        {
            return _collections.Where(c => c.UserId == userId && c.IsPublic).ToList();
        }

        public List<OwnedGame> GetGamesNotInCollection(int collectionId, int userId)
        {
            // Return dummy games not in the collection.
            return new List<OwnedGame>
            {
                new OwnedGame { GameId = 3, UserId = userId, Title = "Game C", Description = "Desc C", CoverPicture = "gameC.jpg" }
            };
        }
    }
}
