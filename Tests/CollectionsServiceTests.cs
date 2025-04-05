using BusinessLayer.Models;
using BusinessLayer.Repositories.Fakes;
using BusinessLayer.Services;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Services.Fakes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    [TestFixture]
    public class CollectionsServiceTests
    {
        private ICollectionsService _collectionsService;
        private FakeCollectionsRepository _fakeCollectionsRepository;
        private FakeOwnedGamesRepository _fakeOwnedGamesRepository;
        private OwnedGamesService _fakeOwnedGamesService;

        [SetUp]
        public void SetUp()
        {
            // For testing the service, use the fake repository.
            _fakeCollectionsRepository = new FakeCollectionsRepository();
            _fakeOwnedGamesRepository = new FakeOwnedGamesRepository();
            // For OwnedGamesService, if you have a real one, you could create a fake as needed.
            // For simplicity, we assume a minimal implementation:
            _fakeOwnedGamesService = new OwnedGamesService(_fakeOwnedGamesRepository); 
            _collectionsService = new CollectionsService(_fakeCollectionsRepository, _fakeOwnedGamesService);
        }

        [Test]
        public void GetAllCollections_ReturnsCollectionsForUser()
        {
            List<Collection> result = _collectionsService.GetAllCollections(1);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.GreaterThanOrEqualTo(3));
            foreach (Collection c in result)
            {
                Assert.That(c.UserId, Is.EqualTo(1));
            }
        }

        [Test]
        public void GetCollectionById_ExistingCollection_ReturnsCollectionWithGames()
        {
            // Arrange: get an existing collection id from fake repository.
            List<Collection> all = _fakeCollectionsRepository.GetAllCollections(1);
            Collection col = all[0];
            int colId = col.CollectionId;
            // The fake repository for games returns dummy games only for collection id 1.
            if (colId != 1)
            {
                colId = 1;
            }
            // Act
            Collection result = _collectionsService.GetCollectionById(colId, 1);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.CollectionId, Is.EqualTo(colId));
            Assert.That(result.Games, Is.Not.Null);
        }

        [Test]
        public void GetGamesInCollection_ReturnsGames()
        {
            List<OwnedGame> result = _collectionsService.GetGamesInCollection(1);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            foreach (OwnedGame game in result)
            {
                Assert.That(game.UserId, Is.EqualTo(1));
            }
        }

        [Test]
        public void AddGameToCollection_DoesNotThrow()
        {
            Exception caught = null;
            try
            {
                _collectionsService.AddGameToCollection(1, 10, 1);
            }
            catch (Exception ex)
            {
                caught = ex;
            }
            Assert.That(caught, Is.Null);
        }

        [Test]
        public void RemoveGameFromCollection_DoesNotThrow()
        {
            Exception caught = null;
            try
            {
                _collectionsService.RemoveGameFromCollection(1, 10);
            }
            catch (Exception ex)
            {
                caught = ex;
            }
            Assert.That(caught, Is.Null);
        }

        [Test]
        public void DeleteCollection_DeletesExistingCollection()
        {
            // Arrange: Create a collection and then delete it.
            _collectionsService.CreateCollection(1, "Service Delete Test", "test.jpg", true, DateOnly.FromDateTime(DateTime.Now));
            List<Collection> all = _fakeCollectionsRepository.GetAllCollections(1);
            Collection toDelete = null;
            int countBefore = all.Count;
            foreach (Collection c in all)
            {
                if (c.Name == "Service Delete Test")
                {
                    toDelete = c;
                    break;
                }
            }
            Assert.That(toDelete, Is.Not.Null);
            // Act
            _collectionsService.DeleteCollection(toDelete.CollectionId, 1);
            List<Collection> after = _fakeCollectionsRepository.GetAllCollections(1);
            bool exists = false;
            foreach (Collection c in after)
            {
                if (c.CollectionId == toDelete.CollectionId)
                {
                    exists = true;
                    break;
                }
            }
            Assert.That(exists, Is.False);
        }

        [Test]
        public void CreateCollection_AddsNewCollection()
        {
            int initialCount = _fakeCollectionsRepository.GetAllCollections(1).Count;
            _collectionsService.CreateCollection(1, "Service Create Test", "cover.jpg", false, DateOnly.FromDateTime(DateTime.Now));
            int newCount = _fakeCollectionsRepository.GetAllCollections(1).Count;
            Assert.That(newCount, Is.EqualTo(initialCount + 1));
        }

        [Test]
        public void UpdateCollection_UpdatesCollectionProperties()
        {
            List<Collection> all = _fakeCollectionsRepository.GetAllCollections(1);
            Collection col = all[0];
            int id = col.CollectionId;
            string newName = "Updated Service Name";
            string newCover = "newcover.jpg";
            bool newVisibility = !col.IsPublic;
            _collectionsService.UpdateCollection(id, 1, newName, newCover, newVisibility);
            Collection updated = _fakeCollectionsRepository.GetCollectionById(id, 1);
            Assert.That(updated, Is.Not.Null);
            Assert.That(updated.Name, Is.EqualTo(newName));
            Assert.That(updated.CoverPicture, Is.EqualTo(newCover));
            Assert.That(updated.IsPublic, Is.EqualTo(newVisibility));
        }

        [Test]
        public void GetPublicCollectionsForUser_ReturnsOnlyPublicCollections()
        {
            List<Collection> result = _collectionsService.GetPublicCollectionsForUser(1);
            foreach (Collection c in result)
            {
                Assert.That(c.IsPublic, Is.True);
            }
        }

        [Test]
        public void GetGamesNotInCollection_ReturnsGames()
        {
            List<OwnedGame> result = _collectionsService.GetGamesNotInCollection(5, 1);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.GreaterThan(0));
        }
    }
}
