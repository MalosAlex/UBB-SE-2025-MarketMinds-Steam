using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using BusinessLayer.Models;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Exceptions;
using BusinessLayer.Services;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Repositories.Fakes;

namespace Tests.ServiceTests
{
    // Normal flow tests for CollectionsService using FakeCollectionsRepository.
    [TestFixture]
    public class CollectionsServiceTests
    {
        private ICollectionsService _collectionsService;
        private FakeCollectionsRepository _fakeCollectionsRepository;

        [SetUp]
        public void SetUp()
        {
            // Arrange: Use the existing fake repository.
            _fakeCollectionsRepository = new FakeCollectionsRepository();
            _collectionsService = new CollectionsService(_fakeCollectionsRepository);
        }

        #region GetAllCollections Normal Flow

        [Test]
        public void GetAllCollections_ResultIsNotNull()
        {
            // Act
            List<Collection> result = _collectionsService.GetAllCollections(1);
            // Assert: Result is not null.
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetAllCollections_ResultCountIsAtLeastThree()
        {
            // Act
            List<Collection> result = _collectionsService.GetAllCollections(1);
            // Assert: Count is at least 3.
            Assert.That(result.Count, Is.GreaterThanOrEqualTo(3));
        }

        [Test]
        public void GetAllCollections_AllCollectionsBelongToUser1()
        {
            // Act
            List<Collection> result = _collectionsService.GetAllCollections(1);
            // Assert: Every collection belongs to user 1.
            Assert.That(result, Has.All.Property("UserId").EqualTo(1));
        }

        #endregion

        #region GetCollectionById Normal Flow

        [Test]
        public void GetCollectionById_ResultIsNotNull()
        {
            // Arrange: Use an existing collection id from the fake repository.
            List<Collection> all = _fakeCollectionsRepository.GetAllCollections(1);
            int colId = all[0].CollectionId;
            // Act
            Collection result = _collectionsService.GetCollectionById(colId, 1);
            // Assert: Collection is not null.
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetCollectionById_ResultHasCorrectCollectionId()
        {
            // Arrange
            List<Collection> all = _fakeCollectionsRepository.GetAllCollections(1);
            int colId = all[0].CollectionId;
            // Act
            Collection result = _collectionsService.GetCollectionById(colId, 1);
            // Assert: Returned collection's CollectionId equals expected.
            Assert.That(result.CollectionId, Is.EqualTo(colId));
        }

        [Test]
        public void GetCollectionById_ResultHasGamesLoaded()
        {
            // Arrange
            List<Collection> all = _fakeCollectionsRepository.GetAllCollections(1);
            int colId = all[0].CollectionId;
            // Act
            Collection result = _collectionsService.GetCollectionById(colId, 1);
            // Assert: The Games property is not null.
            Assert.That(result.Games, Is.Not.Null);
        }

        #endregion

        #region GetGamesInCollection Normal Flow

        [Test]
        public void GetGamesInCollection_ResultIsNotNull()
        {
            // Act
            List<OwnedGame> result = _collectionsService.GetGamesInCollection(1);
            // Assert: Result is not null.
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetGamesInCollection_ResultCountIsCorrect()
        {
            // Act
            List<OwnedGame> result = _collectionsService.GetGamesInCollection(1);
            // Assert: Count is exactly 0 (per fake repository design).
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetGamesInCollection_AllGamesBelongToUser1()
        {
            // Arrange & Act
            List<OwnedGame> result = _collectionsService.GetGamesInCollection(1);
            // Assert: Every returned game has UserId equal to 1.
            Assert.That(result, Has.All.Property("UserId").EqualTo(1));
        }

        #endregion

        #region AddGameToCollection Normal Flow

        [Test]
        public void AddGameToCollection_DoesNotThrowException()
        {
            // Act & Assert: Verify that adding a game does not throw.
            Assert.DoesNotThrow(() => _collectionsService.AddGameToCollection(1, 10, 1));
        }

        #endregion

        #region RemoveGameFromCollection Normal Flow

        [Test]
        public void RemoveGameFromCollection_DoesNotThrowException()
        {
            // Act & Assert: Verify that removing a game does not throw.
            Assert.DoesNotThrow(() => _collectionsService.RemoveGameFromCollection(1, 10));
        }

        #endregion

        #region DeleteCollection Normal Flow

        [Test]
        public void DeleteCollection_RemovesExistingCollection()
        {
            // Arrange: Create a new collection.
            _collectionsService.CreateCollection(1, "Service Delete Test", "test.jpg", true, DateOnly.FromDateTime(DateTime.Now));
            List<Collection> all = _fakeCollectionsRepository.GetAllCollections(1);
            Collection toDelete = all.FirstOrDefault(c => c.Name == "Service Delete Test");
            // (Assume the fake repository always returns the created collection)
            _collectionsService.DeleteCollection(toDelete.CollectionId, 1);
            // Act: Check if collection still exists.
            List<Collection> after = _fakeCollectionsRepository.GetAllCollections(1);
            bool exists = after.Any(c => c.CollectionId == toDelete.CollectionId);
            // Assert: The collection no longer exists.
            Assert.That(exists, Is.False);
        }

        #endregion

        #region CreateCollection Normal Flow

        [Test]
        public void CreateCollection_IncreasesCollectionCountByOne()
        {
            // Arrange: Get initial count.
            int initialCount = _fakeCollectionsRepository.GetAllCollections(1).Count;
            // Act: Create a new collection.
            _collectionsService.CreateCollection(1, "Service Create Test", "cover.jpg", false, DateOnly.FromDateTime(DateTime.Now));
            int newCount = _fakeCollectionsRepository.GetAllCollections(1).Count;
            // Assert: Count increased by one.
            Assert.That(newCount, Is.EqualTo(initialCount + 1));
        }

        #endregion

        #region UpdateCollection Normal Flow

        [Test]
        public void UpdateCollection_UpdatesCollectionPropertiesCorrectly()
        {
            // Arrange: Get an existing collection.
            List<Collection> all = _fakeCollectionsRepository.GetAllCollections(1);
            Collection col = new Collection(1, all[0].Name, all[0].CreatedAt, all[0].CoverPicture, all[0].IsPublic)
            {
                CollectionId = all[0].CollectionId
            };
            int id = col.CollectionId;
            string newName = "Updated Service Name";
            string newCover = "newcover.jpg";
            bool newVisibility = !col.IsPublic;
            // Act: Update the collection.
            _collectionsService.UpdateCollection(id, 1, newName, newCover, newVisibility);
            Collection updated = _fakeCollectionsRepository.GetCollectionById(id, 1);
            // Assert: Verify all updated properties using a composite constraint.
            Assert.That(updated, Has.Property("Name").EqualTo(newName)
                                  .And.Property("CoverPicture").EqualTo(newCover)
                                  .And.Property("IsPublic").EqualTo(newVisibility));
        }

        #endregion

        #region GetPublicCollectionsForUser Normal Flow

        [Test]
        public void GetPublicCollectionsForUser_AllCollectionsArePublic()
        {
            // Act
            List<Collection> result = _collectionsService.GetPublicCollectionsForUser(1);
            // Assert: Every returned collection is public.
            Assert.That(result, Has.All.Property("IsPublic").EqualTo(true));
        }

        #endregion

        #region GetGamesNotInCollection Normal Flow

        [Test]
        public void GetGamesNotInCollection_ReturnsExpectedGames()
        {
            // Arrange: Prepare expected games using the proper OwnedGame constructor.
            var expectedGames = new List<OwnedGame>
            {
                new OwnedGame(1, "Title1", "Description1", "cover1.jpg") { GameId = 1 },
                new OwnedGame(1, "Title2", "Description2", "cover2.jpg") { GameId = 2 }
            };
            var mockRepo = new Mock<ICollectionsRepository>();
            mockRepo.Setup(r => r.GetGamesNotInCollection(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(expectedGames);
            var service = new CollectionsService(mockRepo.Object);
            // Act
            var result = service.GetGamesNotInCollection(1, 1);
            // Assert: Returned list exactly equals the expected list.
            Assert.That(result, Is.EqualTo(expectedGames));
        }

        #endregion
    }

    // Exception tests for CollectionsService using Moq.
    [TestFixture]
    public class CollectionsServiceExceptionTests
    {
        #region ConstructorTests

        [Test]
        public void CollectionsService_CollectionRepositoryNull_ThrowException()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(() => new CollectionsService(null));
        }

        #endregion

        #region GetAllCollections Exception Tests

        [Test]
        public void GetAllCollections_RepositoryException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange
            var mockRepo = new Mock<ICollectionsRepository>();
            mockRepo.Setup(r => r.GetAllCollections(It.IsAny<int>()))
                    .Throws(new RepositoryException("Repo error"));
            var service = new CollectionsService(mockRepo.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => service.GetAllCollections(1)).Message,
                        Is.EqualTo("Failed to retrieve collections from database"));
        }

        [Test]
        public void GetAllCollections_GenericException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange
            var mockRepo = new Mock<ICollectionsRepository>();
            mockRepo.Setup(r => r.GetAllCollections(It.IsAny<int>()))
                    .Throws(new Exception("Generic error"));
            var service = new CollectionsService(mockRepo.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => service.GetAllCollections(1)).Message,
                        Is.EqualTo("An unexpected error occurred while retrieving collections"));
        }

        #endregion

        #region GetCollectionById Exception Tests

        [Test]
        public void GetCollectionById_RepositoryException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange
            var mockRepo = new Mock<ICollectionsRepository>();
            mockRepo.Setup(r => r.GetCollectionById(It.IsAny<int>(), It.IsAny<int>()))
                    .Throws(new RepositoryException("Repo error"));
            var service = new CollectionsService(mockRepo.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => service.GetCollectionById(1, 1)).Message,
                        Is.EqualTo("Failed to retrieve collection."));
        }

        [Test]
        public void GetCollectionById_GenericException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange
            var mockRepo = new Mock<ICollectionsRepository>();
            mockRepo.Setup(r => r.GetCollectionById(It.IsAny<int>(), It.IsAny<int>()))
                    .Throws(new Exception("Generic error"));
            var service = new CollectionsService(mockRepo.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => service.GetCollectionById(1, 1)).Message,
                        Is.EqualTo("An unexpected error occurred while retrieving collection."));
        }

        [Test]
        public void GetCollectionById_GetGamesInCollectionRepositoryException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange
            var collection = new Collection(1, "Test", DateOnly.FromDateTime(DateTime.Now)) { CollectionId = 1 };
            var mockRepo = new Mock<ICollectionsRepository>();
            mockRepo.Setup(r => r.GetCollectionById(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(collection);
            mockRepo.Setup(r => r.GetGamesInCollection(It.IsAny<int>(), It.IsAny<int>()))
                    .Throws(new RepositoryException("Repo error"));
            var service = new CollectionsService(mockRepo.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => service.GetCollectionById(1, 1)).Message,
                        Is.EqualTo("Failed to retrieve collection."));
        }

        [Test]
        public void GetCollectionById_GetGamesInCollectionGenericException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange
            var collection = new Collection(1, "Test", DateOnly.FromDateTime(DateTime.Now)) { CollectionId = 1 };
            var mockRepo = new Mock<ICollectionsRepository>();
            mockRepo.Setup(r => r.GetCollectionById(It.IsAny<int>(), It.IsAny<int>()))
                    .Returns(collection);
            mockRepo.Setup(r => r.GetGamesInCollection(It.IsAny<int>(), It.IsAny<int>()))
                    .Throws(new Exception("Generic error"));
            var service = new CollectionsService(mockRepo.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => service.GetCollectionById(1, 1)).Message,
                        Is.EqualTo("An unexpected error occurred while retrieving collection."));
        }

        #endregion

        #region GetGamesInCollection Exception Tests

        [Test]
        public void GetGamesInCollection_RepositoryException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange
            var mockRepo = new Mock<ICollectionsRepository>();
            mockRepo.Setup(r => r.GetGamesInCollection(It.IsAny<int>()))
                    .Throws(new RepositoryException("Repo error"));
            var service = new CollectionsService(mockRepo.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => service.GetGamesInCollection(1)).Message,
                        Is.EqualTo("Failed to retrieve games from database"));
        }

        [Test]
        public void GetGamesInCollection_GenericException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange
            var mockRepo = new Mock<ICollectionsRepository>();
            mockRepo.Setup(r => r.GetGamesInCollection(It.IsAny<int>()))
                    .Throws(new Exception("Generic error"));
            var service = new CollectionsService(mockRepo.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => service.GetGamesInCollection(1)).Message,
                        Is.EqualTo("An unexpected error occurred while retrieving games"));
        }

        #endregion

        #region AddGameToCollection Exception Tests

        [Test]
        public void AddGameToCollection_RepositoryException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange
            var mockRepo = new Mock<ICollectionsRepository>();
            mockRepo.Setup(r => r.AddGameToCollection(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                    .Throws(new RepositoryException("Repo error"));
            var service = new CollectionsService(mockRepo.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => service.AddGameToCollection(1, 10, 1)).Message,
                        Is.EqualTo("Failed to add game to collection"));
        }

        [Test]
        public void AddGameToCollection_GenericException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange
            var mockRepo = new Mock<ICollectionsRepository>();
            mockRepo.Setup(r => r.AddGameToCollection(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                    .Throws(new Exception("Generic error"));
            var service = new CollectionsService(mockRepo.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => service.AddGameToCollection(1, 10, 1)).Message,
                        Is.EqualTo("An unexpected error occurred"));
        }

        #endregion

        #region RemoveGameFromCollection Exception Tests

        [Test]
        public void RemoveGameFromCollection_RepositoryException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange
            var mockRepo = new Mock<ICollectionsRepository>();
            mockRepo.Setup(r => r.RemoveGameFromCollection(It.IsAny<int>(), It.IsAny<int>()))
                    .Throws(new RepositoryException("Repo error"));
            var service = new CollectionsService(mockRepo.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => service.RemoveGameFromCollection(1, 10)).Message,
                        Is.EqualTo("Failed to remove game from collection."));
        }

        [Test]
        public void RemoveGameFromCollection_GenericException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange
            var mockRepo = new Mock<ICollectionsRepository>();
            mockRepo.Setup(r => r.RemoveGameFromCollection(It.IsAny<int>(), It.IsAny<int>()))
                    .Throws(new Exception("Generic error"));
            var service = new CollectionsService(mockRepo.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => service.RemoveGameFromCollection(1, 10)).Message,
                        Is.EqualTo("An unexpected error occurred while removing game from collection."));
        }

        #endregion

        #region DeleteCollection Exception Tests

        [Test]
        public void DeleteCollection_GenericException_ThrowsExceptionWithExpectedMessage()
        {
            // Arrange
            var mockRepo = new Mock<ICollectionsRepository>();
            mockRepo.Setup(r => r.DeleteCollection(It.IsAny<int>(), It.IsAny<int>()))
                    .Throws(new Exception("Generic error"));
            var service = new CollectionsService(mockRepo.Object);
            // Act & Assert
            Assert.That(Assert.Throws<Exception>(() => service.DeleteCollection(1, 1)).Message,
                        Is.EqualTo("Failed to delete collection"));
        }

        #endregion

        #region CreateCollection Exception Tests

        [Test]
        public void CreateCollection_RepositoryException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange
            var mockRepo = new Mock<ICollectionsRepository>();
            mockRepo.Setup(r => r.CreateCollection(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<DateOnly>()))
                    .Throws(new RepositoryException("Repo error"));
            var service = new CollectionsService(mockRepo.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => service.CreateCollection(1, "Test", "cover.jpg", true, DateOnly.FromDateTime(DateTime.Now))).Message,
                        Is.EqualTo("Failed to create collection in database"));
        }

        [Test]
        public void CreateCollection_GenericException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange
            var mockRepo = new Mock<ICollectionsRepository>();
            mockRepo.Setup(r => r.CreateCollection(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<DateOnly>()))
                    .Throws(new Exception("Generic error"));
            var service = new CollectionsService(mockRepo.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => service.CreateCollection(1, "Test", "cover.jpg", true, DateOnly.FromDateTime(DateTime.Now))).Message,
                        Is.EqualTo("An unexpected error occurred while creating collection"));
        }

        #endregion

        #region UpdateCollection Exception Tests

        [Test]
        public void UpdateCollection_RepositoryException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange
            var mockRepo = new Mock<ICollectionsRepository>();
            mockRepo.Setup(r => r.UpdateCollection(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                    .Throws(new RepositoryException("Repo error"));
            var service = new CollectionsService(mockRepo.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => service.UpdateCollection(1, 1, "New Name", "newcover.jpg", true)).Message,
                        Is.EqualTo("Failed to update collection in database"));
        }

        [Test]
        public void UpdateCollection_GenericException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange
            var mockRepo = new Mock<ICollectionsRepository>();
            mockRepo.Setup(r => r.UpdateCollection(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                    .Throws(new Exception("Generic error"));
            var service = new CollectionsService(mockRepo.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => service.UpdateCollection(1, 1, "New Name", "newcover.jpg", true)).Message,
                        Is.EqualTo("An unexpected error occurred while updating collection"));
        }

        #endregion

        #region GetPublicCollectionsForUser Exception Tests

        [Test]
        public void GetPublicCollectionsForUser_RepositoryException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange
            var mockRepo = new Mock<ICollectionsRepository>();
            mockRepo.Setup(r => r.GetPublicCollectionsForUser(It.IsAny<int>()))
                    .Throws(new RepositoryException("Repo error"));
            var service = new CollectionsService(mockRepo.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => service.GetPublicCollectionsForUser(1)).Message,
                        Is.EqualTo("Failed to retrieve public collections from database"));
        }

        [Test]
        public void GetPublicCollectionsForUser_GenericException_ThrowsServiceExceptionWithExpectedMessage()
        {
            // Arrange
            var mockRepo = new Mock<ICollectionsRepository>();
            mockRepo.Setup(r => r.GetPublicCollectionsForUser(It.IsAny<int>()))
                    .Throws(new Exception("Generic error"));
            var service = new CollectionsService(mockRepo.Object);
            // Act & Assert
            Assert.That(Assert.Throws<ServiceException>(() => service.GetPublicCollectionsForUser(1)).Message,
                        Is.EqualTo("An unexpected error occurred while retrieving public collections"));
        }

        #endregion
    }
}
