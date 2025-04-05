using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using BusinessLayer.Models;
using BusinessLayer.Services.Fakes;

namespace Tests
{
    [TestFixture]
    public class FakeCollectionsServiceTests
    {
        #region GetAllCollections

        [Test]
        public void GetAllCollections_ReturnsOnlyUserCollections()
        {
            // Arrange
            var fakeService = new FakeCollectionsService();

            // Act
            var result = fakeService.GetAllCollections(1);

            // Assert: Expect exactly 3 collections for user 1 (seeded data).
            Assert.That(result.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetAllCollections_NoCollectionsForUser_ReturnsEmptyList()
        {
            // Arrange
            var fakeService = new FakeCollectionsService();

            // Act
            var result = fakeService.GetAllCollections(999);

            // Assert: Expect an empty list for a user with no collections.
            Assert.That(result, Is.Empty);
        }

        #endregion

        #region GetCollectionById

        [Test]
        public void GetCollectionById_Collection1_ReturnsGamesAttached()
        {
            // Arrange
            var fakeService = new FakeCollectionsService();

            // Act: For collectionId 1 and user 1, dummy games are attached.
            var result = fakeService.GetCollectionById(1, 1);

            // Assert: Expect the dummy games list to have exactly 2 games.
            Assert.That(result.Games.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetCollectionById_InvalidUser_ReturnsNull()
        {
            // Arrange
            var fakeService = new FakeCollectionsService();

            // Act: Request a collection for a non-matching user.
            var result = fakeService.GetCollectionById(1, 999);

            // Assert: Expect null since the collection does not belong to user 999.
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetCollectionById_CollectionNot1_DoesNotAttachGames()
        {
            // Arrange
            var fakeService = new FakeCollectionsService();

            // Act: For a collection other than 1, no dummy games are attached.
            var result = fakeService.GetCollectionById(2, 1);

            // Assert: Expect the games list to remain as-is (seeded fakes do not add games).
            Assert.That(result.Games.Count, Is.EqualTo(0));
        }

        #endregion

        #region GetGamesInCollection Overloads

        [Test]
        public void GetGamesInCollection_WithoutUser_ReturnsEmptyList()
        {
            // Arrange
            var fakeService = new FakeCollectionsService();

            // Act: Call the overload that does not use userId.
            var result = fakeService.GetGamesInCollection(2);

            // Assert: Expect an empty list.
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetGamesInCollection_WithUser_Collection1_ReturnsGames()
        {
            // Arrange
            var fakeService = new FakeCollectionsService();

            // Act: For collectionId 1 and user 1, dummy games should be returned.
            var result = fakeService.GetGamesInCollection(1, 1);

            // Assert: Expect exactly 2 games.
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetGamesInCollection_WithUser_NonOneCollection_ReturnsEmptyList()
        {
            // Arrange
            var fakeService = new FakeCollectionsService();

            // Act: For a collection other than 1, it falls back to GetGamesInCollection(int) overload.
            var result = fakeService.GetGamesInCollection(2, 1);

            // Assert: Expect an empty list.
            Assert.That(result, Is.Empty);
        }

        #endregion

        #region AddGameToCollection, RemoveGameFromCollection,
        // MakeCollectionPrivateForUser, MakeCollectionPublicForUser

        [Test]
        public void AddGameToCollection_DoesNotThrow()
        {
            // Arrange
            var fakeService = new FakeCollectionsService();

            // Act & Assert: Verify that adding a game does not throw an exception.
            Assert.DoesNotThrow(() => fakeService.AddGameToCollection(1, 10, 1));
        }

        [Test]
        public void RemoveGameFromCollection_DoesNotThrow()
        {
            // Arrange
            var fakeService = new FakeCollectionsService();

            // Act & Assert: Verify that removing a game does not throw an exception.
            Assert.DoesNotThrow(() => fakeService.RemoveGameFromCollection(1, 10));
        }

        [Test]
        public void MakeCollectionPrivateForUser_DoesNotThrow()
        {
            // Arrange
            var fakeService = new FakeCollectionsService();

            // Act & Assert: Verify that making a collection private does not throw.
            Assert.DoesNotThrow(() => fakeService.MakeCollectionPrivateForUser("1", "1"));
        }

        [Test]
        public void MakeCollectionPublicForUser_DoesNotThrow()
        {
            // Arrange
            var fakeService = new FakeCollectionsService();

            // Act & Assert: Verify that making a collection public does not throw.
            Assert.DoesNotThrow(() => fakeService.MakeCollectionPublicForUser("1", "1"));
        }

        #endregion

        #region RemoveCollectionForUser

        [Test]
        public void RemoveCollectionForUser_RemovesCollection()
        {
            // Arrange
            var fakeService = new FakeCollectionsService();

            // Act: Remove collection with id 2 for user 1.
            fakeService.RemoveCollectionForUser("1", "2");
            var result = fakeService.GetAllCollections(1);

            // Assert: Expect no collection with CollectionId == 2.
            Assert.That(result.Any(c => c.CollectionId == 2), Is.False);
        }

        [Test]
        public void RemoveCollectionForUser_NoMatch_DoesNothing()
        {
            // Arrange
            var fakeService = new FakeCollectionsService();
            var before = fakeService.GetAllCollections(1).Count;

            // Act: Attempt to remove a non-existing collection.
            fakeService.RemoveCollectionForUser("1", "999");
            var after = fakeService.GetAllCollections(1).Count;

            // Assert: Expect the count to remain unchanged.
            Assert.That(after, Is.EqualTo(before));
        }

        #endregion

        #region SaveCollection

        [Test]
        public void SaveCollection_NewCollection_AddsCollection()
        {
            // Arrange
            var fakeService = new FakeCollectionsService();
            var newCollection = new Collection(1, "New Collection", DateOnly.FromDateTime(DateTime.Now)) { CollectionId = 0 };

            // Act: Save the new collection.
            fakeService.SaveCollection("1", newCollection);
            var result = fakeService.GetAllCollections(1);

            // Assert: Expect a collection with the name "New Collection" to exist.
            Assert.That(result.Any(c => c.Name == "New Collection"), Is.True);
        }

        [Test]
        public void SaveCollection_UpdateCollection_UpdatesCollection()
        {
            // Arrange
            var fakeService = new FakeCollectionsService();
            var original = fakeService.GetAllCollections(1).First();
            original.Name = "Updated Name";

            // Act: Save (update) the existing collection.
            fakeService.SaveCollection("1", original);
            var updated = fakeService.GetCollectionById(original.CollectionId, 1);

            // Assert: Expect the updated collection's name to be "Updated Name".
            Assert.That(updated.Name, Is.EqualTo("Updated Name"));
        }

        [Test]
        public void SaveCollection_UpdateNonExistingCollection_DoesNotAddNew()
        {
            // Arrange
            var fakeService = new FakeCollectionsService();
            // Create a collection with a non-existing CollectionId.
            var collection = new Collection(1, "Non Existing", DateOnly.FromDateTime(DateTime.Now)) { CollectionId = 999 };
            var before = fakeService.GetAllCollections(1).Count;

            // Act: Attempt to update (which will not match any record).
            fakeService.SaveCollection("1", collection);
            var after = fakeService.GetAllCollections(1).Count;

            // Assert: Count remains the same.
            Assert.That(after, Is.EqualTo(before));
        }

        #endregion

        #region DeleteCollection

        [Test]
        public void DeleteCollection_DeletesCollection()
        {
            // Arrange
            var fakeService = new FakeCollectionsService();
            var initialCount = fakeService.GetAllCollections(1).Count;

            // Act: Delete collection with id 1 for user 1.
            fakeService.DeleteCollection(1, 1);
            var newCount = fakeService.GetAllCollections(1).Count;

            // Assert: Expect the new count to be one less than the initial count.
            Assert.That(newCount, Is.EqualTo(initialCount - 1));
        }

        [Test]
        public void DeleteCollection_NonExisting_DoesNothing()
        {
            // Arrange
            var fakeService = new FakeCollectionsService();
            var before = fakeService.GetAllCollections(1).Count;

            // Act: Attempt to delete a non-existing collection.
            fakeService.DeleteCollection(999, 1);
            var after = fakeService.GetAllCollections(1).Count;

            // Assert: Expect the count to remain unchanged.
            Assert.That(after, Is.EqualTo(before));
        }

        #endregion

        #region CreateCollection

        [Test]
        public void CreateCollection_AddsCollection()
        {
            // Arrange
            var fakeService = new FakeCollectionsService();
            var initialCount = fakeService.GetAllCollections(1).Count;

            // Act: Create a new collection for user 1.
            fakeService.CreateCollection(1, "Created Collection", "coverCreated.jpg", true, DateOnly.FromDateTime(DateTime.Now));
            var newCount = fakeService.GetAllCollections(1).Count;

            // Assert: Expect the new count to be one more than the initial count.
            Assert.That(newCount, Is.EqualTo(initialCount + 1));
        }

        #endregion

        #region UpdateCollection

        [Test]
        public void UpdateCollection_UpdatesProperties()
        {
            // Arrange
            var fakeService = new FakeCollectionsService();

            // Act: Update collection with id 2 for user 1.
            fakeService.UpdateCollection(2, 1, "Updated Collection", "newCover.jpg", false);
            var updated = fakeService.GetCollectionById(2, 1);

            // Assert: Expect the collection's name to be "Updated Collection".
            Assert.That(updated.Name, Is.EqualTo("Updated Collection"));
        }

        #endregion

        #region GetPublicCollectionsForUser

        [Test]
        public void GetPublicCollectionsForUser_ReturnsOnlyPublicCollections()
        {
            // Arrange
            var fakeService = new FakeCollectionsService();

            // Act: Get public collections for user 1.
            var result = fakeService.GetPublicCollectionsForUser(1);

            // Assert: Expect exactly 2 public collections (from seeded data).
            Assert.That(result.Count, Is.EqualTo(2));
        }

        #endregion

        #region GetGamesNotInCollection

        [Test]
        public void GetGamesNotInCollection_ReturnsGameC()
        {
            // Arrange
            var fakeService = new FakeCollectionsService();

            // Act: Retrieve games not in collection for user 1.
            var result = fakeService.GetGamesNotInCollection(5, 1);

            // Assert: Expect at least one game with GameId equal to 3.
            Assert.That(result.Any(g => g.GameId == 3), Is.True);
        }

        #endregion

        #region GetLastThreeCollectionsForUser

        [Test]
        public void GetLastThreeCollectionsForUser_ReturnsLastThreeSorted()
        {
            // Arrange
            var fakeService = new FakeCollectionsService();

            // Act: Retrieve the last three collections for user 1.
            var result = fakeService.GetLastThreeCollectionsForUser(1);

            // Assert: Expect exactly 3 collections and the first's CreatedAt is not earlier than the second's.
            Assert.That(result.Count == 3 && (result.Count < 2 || result[0].CreatedAt.CompareTo(result[1].CreatedAt) >= 0), Is.True);
        }

        [Test]
        public void GetLastThreeCollectionsForUser_UserWithLessThanThree_ReturnsAll()
        {
            // Arrange
            var fakeService = new FakeCollectionsService();

            // Act: For user 2, only one collection exists.
            var result = fakeService.GetLastThreeCollectionsForUser(2);

            // Assert: Expect exactly 1 collection.
            Assert.That(result.Count, Is.EqualTo(1));
        }

        #endregion
    }
}
