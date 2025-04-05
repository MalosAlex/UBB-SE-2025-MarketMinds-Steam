using BusinessLayer.Models;
using BusinessLayer.Repositories;
using BusinessLayer.Repositories.Fakes;
using BusinessLayer.Repositories.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLayer.Data;

namespace Tests
{
    [TestFixture]
    public class CollectionsRepositoryTests
    {
        private ICollectionsRepository _collectionsRepository;

        [SetUp]
        public void SetUp()
        {
            // Use the fake implementation for testing.
            _collectionsRepository = new CollectionsRepository(new FakeDataLink());
        }

        [Test]
        public void GetAllCollections_UserHasCollections_ReturnsCollections()
        {
            // Arrange: userId 1 is seeded with collections in FakeCollectionsRepository.

            // Act
            List<Collection> collections = _collectionsRepository.GetAllCollections(1);

            // Assert
            Assert.That(collections, Is.Not.Null, "Collections should not be null.");
            Assert.That(collections.Count, Is.GreaterThanOrEqualTo(3), "There should be at least three collections for user 1.");
            foreach (Collection c in collections)
            {
                Assert.That(c.UserId, Is.EqualTo(1), "All collections should have UserId equal to 1.");
            }
        }

        [Test]
        public void GetLastThreeCollectionsForUser_ReturnsMostRecentThree()
        {
            // Arrange
            List<Collection> allCollections = _collectionsRepository.GetAllCollections(1);
            int expectedCount = allCollections.Count < 3 ? allCollections.Count : 3;

            // Act
            List<Collection> lastThree = _collectionsRepository.GetLastThreeCollectionsForUser(1);

            // Assert
            Assert.That(lastThree.Count, Is.EqualTo(expectedCount), "The returned count should equal the expected count.");
            // Verify that the collections are in descending order by CreatedAt.
            List<Collection> ordered = lastThree.OrderByDescending(c => c.CreatedAt).ToList();
            for (int i = 0; i < lastThree.Count; i++)
            {
                Assert.That(lastThree[i].CreatedAt, Is.EqualTo(ordered[i].CreatedAt), "Collections should be in descending order by CreatedAt.");
            }
        }

        [Test]
        public void GetCollectionById_ExistingCollection_ReturnsCollection()
        {
            // Arrange
            List<Collection> allCollections = _collectionsRepository.GetAllCollections(1);
            Collection existing = allCollections.First();
            int collectionId = existing.CollectionId;

            // Act
            Collection result = _collectionsRepository.GetCollectionById(collectionId, 1);

            // Assert
            Assert.That(result, Is.Not.Null, "The collection should be found.");
            Assert.That(result.CollectionId, Is.EqualTo(collectionId), "Returned collection ID should match the expected value.");
        }

        [Test]
        public void GetCollectionById_NonExistingCollection_ReturnsNull()
        {
            // Act
            Collection result = _collectionsRepository.GetCollectionById(999, 1);

            // Assert
            Assert.That(result, Is.Null, "Non-existing collection should return null.");
        }

        [Test]
        public void GetGamesInCollection_OverloadWithoutUser_ReturnsEmptyList()
        {
            // Act
            List<OwnedGame> games = _collectionsRepository.GetGamesInCollection(2);

            // Assert
            Assert.That(games, Is.Not.Null, "Games list should not be null.");
            Assert.That(games.Count, Is.EqualTo(0), "Games list should be empty for collectionId 2.");
        }

        [Test]
        public void GetGamesInCollection_WithUserId1_ReturnsDummyGamesForCollectionId1()
        {
            // Act
            List<OwnedGame> games = _collectionsRepository.GetGamesInCollection(1, 1);

            // Assert
            Assert.That(games, Is.Not.Null, "Games list should not be null.");
            Assert.That(games.Count, Is.EqualTo(2), "Dummy games count should equal 2 for collectionId 1.");
            foreach (OwnedGame game in games)
            {
                Assert.That(game.UserId, Is.EqualTo(1), "Each game should have UserId equal to 1.");
            }
        }

        [Test]
        public void AddGameToCollection_NoOp_DoesNotThrow()
        {
            // Act
            Exception exception = null;
            try
            {
                _collectionsRepository.AddGameToCollection(1, 10, 1);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Assert
            Assert.That(exception, Is.Null, "AddGameToCollection should not throw any exception.");
        }

        [Test]
        public void RemoveGameFromCollection_NoOp_DoesNotThrow()
        {
            // Act
            Exception exception = null;
            try
            {
                _collectionsRepository.RemoveGameFromCollection(1, 10);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Assert
            Assert.That(exception, Is.Null, "RemoveGameFromCollection should not throw any exception.");
        }

        [Test]
        public void MakeCollectionPrivateForUser_NoOp_DoesNotThrow()
        {
            // Act
            Exception exception = null;
            try
            {
                _collectionsRepository.MakeCollectionPrivateForUser("1", "1");
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Assert
            Assert.That(exception, Is.Null, "MakeCollectionPrivateForUser should not throw any exception.");
        }

        [Test]
        public void MakeCollectionPublicForUser_NoOp_DoesNotThrow()
        {
            // Act
            Exception exception = null;
            try
            {
                _collectionsRepository.MakeCollectionPublicForUser("1", "1");
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            // Assert
            Assert.That(exception, Is.Null, "MakeCollectionPublicForUser should not throw any exception.");
        }

        [Test]
        public void RemoveCollectionForUser_ValidCollection_RemovesCollection()
        {
            // Arrange
            List<Collection> collectionsBefore = _collectionsRepository.GetAllCollections(1);
            Collection collectionToRemove = collectionsBefore.First();

            // Act
            _collectionsRepository.RemoveCollectionForUser("1", collectionToRemove.CollectionId.ToString());
            List<Collection> collectionsAfter = _collectionsRepository.GetAllCollections(1);

            // Assert
            bool exists = false;
            foreach (Collection c in collectionsAfter)
            {
                if (c.CollectionId == collectionToRemove.CollectionId)
                {
                    exists = true;
                    break;
                }
            }
            Assert.That(exists, Is.False, "The collection should have been removed.");
        }

        [Test]
        public void SaveCollection_NewCollection_AssignsCollectionId()
        {
            // Arrange
            Collection newCollection = new Collection(userId: 1, name: "New Test Collection", createdAt: DateOnly.FromDateTime(DateTime.Now));
            newCollection.CoverPicture = "newpic.jpg";
            newCollection.IsPublic = true;

            // Act
            _collectionsRepository.SaveCollection("1", newCollection);

            // Assert: Check that the new collection has been assigned a nonzero CollectionId.
            List<Collection> collections = _collectionsRepository.GetAllCollections(1);
            bool found = false;
            foreach (Collection c in collections)
            {
                if (c.Name == "New Test Collection" && c.CollectionId != 0)
                {
                    found = true;
                    break;
                }
            }
            Assert.That(found, Is.True, "The new collection should be saved and assigned a CollectionId.");
        }

        [Test]
        public void SaveCollection_UpdateExistingCollection_ChangesValues()
        {
            // Arrange
            Collection collection = _collectionsRepository.GetAllCollections(1).First();
            string originalName = collection.Name;
            string updatedName = originalName + " Updated";

            // Act
            collection.Name = updatedName;
            _collectionsRepository.SaveCollection("1", collection);
            Collection updated = _collectionsRepository.GetCollectionById(collection.CollectionId, 1);

            // Assert
            Assert.That(updated, Is.Not.Null, "Updated collection should not be null.");
            Assert.That(updated.Name, Is.EqualTo(updatedName), "Collection name should be updated.");
        }

        [Test]
        public void DeleteCollection_ExistingCollection_RemovesIt()
        {
            // Arrange: Create a new collection then delete it.
            _collectionsRepository.CreateCollection(1, "Delete Test", "delpic.jpg", true, DateOnly.FromDateTime(DateTime.Now));
            List<Collection> all = _collectionsRepository.GetAllCollections(1);
            Collection toDelete = null;
            foreach (Collection c in all)
            {
                if (c.Name == "Delete Test")
                {
                    toDelete = c;
                    break;
                }
            }
            Assert.That(toDelete, Is.Not.Null, "The new collection to delete should be created.");

            // Act
            _collectionsRepository.DeleteCollection(toDelete.CollectionId, 1);
            List<Collection> afterDelete = _collectionsRepository.GetAllCollections(1);

            // Assert
            bool exists = false;
            foreach (Collection c in afterDelete)
            {
                if (c.CollectionId == toDelete.CollectionId)
                {
                    exists = true;
                    break;
                }
            }
            Assert.That(exists, Is.False, "The collection should have been deleted.");
        }

        [Test]
        public void CreateCollection_NewCollection_AddsCollection()
        {
            // Arrange
            int initialCount = _collectionsRepository.GetAllCollections(1).Count;

            // Act
            _collectionsRepository.CreateCollection(1, "Create Test", "createpic.jpg", false, DateOnly.FromDateTime(DateTime.Now));
            int newCount = _collectionsRepository.GetAllCollections(1).Count;

            // Assert
            Assert.That(newCount, Is.EqualTo(initialCount + 1), "The count of collections should increase by one.");
        }

        [Test]
        public void UpdateCollection_ExistingCollection_UpdatesProperties()
        {
            // Arrange
            Collection collection = _collectionsRepository.GetAllCollections(1).First();
            int collectionId = collection.CollectionId;
            string newName = "Updated Name";
            string newCover = "updatedcover.jpg";
            bool newVisibility = !collection.IsPublic;

            // Act
            _collectionsRepository.UpdateCollection(collectionId, 1, newName, newCover, newVisibility);
            Collection updated = _collectionsRepository.GetCollectionById(collectionId, 1);

            // Assert
            Assert.That(updated, Is.Not.Null, "Updated collection should not be null.");
            Assert.That(updated.Name, Is.EqualTo(newName), "Name should be updated.");
            Assert.That(updated.CoverPicture, Is.EqualTo(newCover), "CoverPicture should be updated.");
            Assert.That(updated.IsPublic, Is.EqualTo(newVisibility), "Visibility should be updated.");
        }

        [Test]
        public void GetPublicCollectionsForUser_ReturnsOnlyPublicCollections()
        {
            // Act
            List<Collection> publicCollections = _collectionsRepository.GetPublicCollectionsForUser(1);

            // Assert
            foreach (Collection c in publicCollections)
            {
                Assert.That(c.IsPublic, Is.True, "Each returned collection should be public.");
            }
        }

        [Test]
        public void GetGamesNotInCollection_ReturnsDummyGames()
        {
            // Act
            List<OwnedGame> games = _collectionsRepository.GetGamesNotInCollection(5, 1);

            // Assert
            Assert.That(games, Is.Not.Null, "Games list should not be null.");
            Assert.That(games.Count, Is.GreaterThan(0), "There should be at least one game returned.");
        }
        
        [Test]
        public void IsAllOwnedGamesCollection_DefaultCollection_ReturnsFalse()
        {
            // Arrange
            var collection = new Collection(userId: 1, name: "Test", createdAt: DateOnly.FromDateTime(DateTime.Now));

            // Act
            bool isAllOwned = collection.IsAllOwnedGamesCollection;

            // Assert
            Assert.That(isAllOwned, Is.False, "IsAllOwnedGamesCollection should be false by default.");
        }

    }
}
