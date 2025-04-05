using NUnit.Framework;
using BusinessLayer.Models;
using BusinessLayer.Repositories.Fakes;
using System;
using System.Linq;

namespace Tests
{
    [TestFixture]
    public class FakeCollectionsRepositoryTests
    {
        private FakeCollectionsRepository _repo;

        [SetUp]
        public void SetUp()
        {
            // Arrange: Instantiate the fake repository (which seeds dummy data).
            _repo = new FakeCollectionsRepository();
        }

        [Test]
        public void GetAllCollections_ReturnsThreeCollections_ForUser1()
        {
            // Act: Get all collections for user 1.
            var collections = _repo.GetAllCollections(1);
            // Assert: Expect exactly three collections.
            Assert.That(collections.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetAllCollections_ReturnsEmpty_ForNonExistingUser()
        {
            // Act: Get all collections for a user that has none.
            var collections = _repo.GetAllCollections(999);
            // Assert: Expect an empty list.
            Assert.That(collections, Is.Empty);
        }

        [Test]
        public void GetLastThreeCollectionsForUser_ReturnsAtMostThreeCollections()
        {
            // Act: Get the last three collections for user 1.
            var lastThree = _repo.GetLastThreeCollectionsForUser(1);
            // Assert: Expect exactly three collections.
            Assert.That(lastThree.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetLastThreeCollectionsForUser_ReturnsCollectionsInDescendingOrder()
        {
            // Act: Get the last three collections for user 1.
            var lastThree = _repo.GetLastThreeCollectionsForUser(1);
            // Assert: The first collection should have a CreatedAt not earlier than the second.
            Assert.That(lastThree.First().CreatedAt, Is.GreaterThanOrEqualTo(lastThree.ElementAt(1).CreatedAt));
        }

        [Test]
        public void GetCollectionById_ReturnsCorrectCollection_WhenItExists()
        {
            // Act: Retrieve collection with CollectionId 2 for user 1.
            var collection = _repo.GetCollectionById(2, 1);
            // Assert: The collection's name should be "Test Collection 2".
            Assert.That(collection.Name, Is.EqualTo("Test Collection 2"));
        }

        [Test]
        public void GetCollectionById_ReturnsNull_WhenItDoesNotExist()
        {
            // Act: Try to retrieve a non-existing collection.
            var collection = _repo.GetCollectionById(999, 1);
            // Assert: The result should be null.
            Assert.That(collection, Is.Null);
        }

        [Test]
        public void GetGamesInCollection_ReturnsEmptyList()
        {
            // Act: Call the method that returns games in a collection (for a single parameter).
            var games = _repo.GetGamesInCollection(1);
            // Assert: Should always return an empty list.
            Assert.That(games, Is.Empty);
        }

        [Test]
        public void GetGamesInCollection_WithUserId_ReturnsTwoGames_WhenCollectionIdIsOne()
        {
            // Act: Call the overload with a userId when collectionId is 1.
            var games = _repo.GetGamesInCollection(1, 1);
            // Assert: Should return two games.
            Assert.That(games.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetGamesInCollection_WithUserId_ReturnsEmpty_WhenCollectionIdIsNotOne()
        {
            // Act: Call the overload with a userId when collectionId is not 1.
            var games = _repo.GetGamesInCollection(2, 1);
            // Assert: Should return an empty list.
            Assert.That(games, Is.Empty);
        }

        [Test]
        public void AddGameToCollection_DoesNotThrowException()
        {
            // Act & Assert: Calling AddGameToCollection should not throw any exception.
            Assert.DoesNotThrow(() => _repo.AddGameToCollection(1, 1, 1));
        }

        [Test]
        public void RemoveGameFromCollection_DoesNotThrowException()
        {
            // Act & Assert: Calling RemoveGameFromCollection should not throw any exception.
            Assert.DoesNotThrow(() => _repo.RemoveGameFromCollection(1, 1));
        }

        [Test]
        public void MakeCollectionPrivateForUser_DoesNotThrowException()
        {
            // Act & Assert: Calling MakeCollectionPrivateForUser should not throw any exception.
            Assert.DoesNotThrow(() => _repo.MakeCollectionPrivateForUser("1", "1"));
        }

        [Test]
        public void MakeCollectionPublicForUser_DoesNotThrowException()
        {
            // Act & Assert: Calling MakeCollectionPublicForUser should not throw any exception.
            Assert.DoesNotThrow(() => _repo.MakeCollectionPublicForUser("1", "1"));
        }

        [Test]
        public void RemoveCollectionForUser_RemovesCollection_WhenItExists()
        {
            // Arrange: Get the count before removal.
            int beforeCount = _repo.GetAllCollections(1).Count;
            // Act: Remove collection with CollectionId "1" for user "1".
            _repo.RemoveCollectionForUser("1", "1");
            // Assert: Count should decrease by one.
            int afterCount = _repo.GetAllCollections(1).Count;
            Assert.That(afterCount, Is.EqualTo(beforeCount - 1));
        }

        [Test]
        public void SaveCollection_AddsNewCollection_WhenCollectionIdIsZero()
        {
            // Arrange: Create a new collection with CollectionId = 0.
            var newCollection = new Collection(1, "New Collection", DateOnly.FromDateTime(DateTime.Now), "new.jpg", true);
            newCollection.CollectionId = 0;
            int beforeCount = _repo.GetAllCollections(1).Count;
            // Act: Save the new collection.
            _repo.SaveCollection("1", newCollection);
            // Assert: Count should increase by one.
            int afterCount = _repo.GetAllCollections(1).Count;
            Assert.That(afterCount, Is.EqualTo(beforeCount + 1));
        }

        [Test]
        public void SaveCollection_UpdatesExistingCollection_WhenCollectionIdIsNotZero()
        {
            // Arrange: Retrieve an existing collection.
            var collection = _repo.GetCollectionById(2, 1);
            // Act: Change its name and save.
            collection.Name = "Updated Name";
            _repo.SaveCollection("1", collection);
            // Assert: The updated collection should have the new name.
            var updated = _repo.GetCollectionById(2, 1);
            Assert.That(updated.Name, Is.EqualTo("Updated Name"));
        }

        [Test]
        public void DeleteCollection_RemovesCollection_WhenItExists()
        {
            // Arrange: Get current count for user 1.
            int beforeCount = _repo.GetAllCollections(1).Count;
            // Act: Delete collection with CollectionId 2 for user 1.
            _repo.DeleteCollection(2, 1);
            // Assert: Count should decrease by one.
            int afterCount = _repo.GetAllCollections(1).Count;
            Assert.That(afterCount, Is.EqualTo(beforeCount - 1));
        }

        [Test]
        public void CreateCollection_AddsNewCollection()
        {
            // Arrange: Get the current count.
            int beforeCount = _repo.GetAllCollections(1).Count;
            // Act: Create a new collection for user 1.
            _repo.CreateCollection(1, "Created Collection", "created.jpg", true, DateOnly.FromDateTime(DateTime.Now));
            // Assert: Count should increase by one.
            int afterCount = _repo.GetAllCollections(1).Count;
            Assert.That(afterCount, Is.EqualTo(beforeCount + 1));
        }

        [Test]
        public void UpdateCollection_UpdatesExistingCollection()
        {
            // Arrange: Retrieve a collection for user 1.
            var collection = _repo.GetCollectionById(2, 1);
            // Act: Update its details.
            _repo.UpdateCollection(2, 1, "Updated Collection", "updated.jpg", false);
            // Assert: The collection's name should be updated.
            var updated = _repo.GetCollectionById(2, 1);
            Assert.That(updated.Name, Is.EqualTo("Updated Collection"));
        }

        [Test]
        public void GetPublicCollectionsForUser_ReturnsOnlyPublicCollections()
        {
            // Act: Retrieve public collections for user 1.
            var publicCollections = _repo.GetPublicCollectionsForUser(1);
            // Assert: For user 1, seeded data has two public collections.
            Assert.That(publicCollections.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetGamesNotInCollection_ReturnsDummyGame()
        {
            // Act: Retrieve games not in a collection for user 1.
            var games = _repo.GetGamesNotInCollection(1, 1);
            // Assert: Should return a list with one dummy game.
            Assert.That(games.Count, Is.EqualTo(1));
        }
    }
}
