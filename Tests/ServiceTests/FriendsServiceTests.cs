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
using BusinessLayer.Services.Fakes;

namespace Tests.ServiceTests
{
    [TestFixture]
    public class FriendsServiceTests
    {
        private FakeFriendshipsRepository _fakeFriendshipsRepository;
        private FakeUserService _fakeUserService;
        private IFriendsService _friendsService;

        [SetUp]
        public void SetUp()
        {
            // Arrange: Use the existing fakes for normal flows.
            _fakeFriendshipsRepository = new FakeFriendshipsRepository();
            _fakeUserService = new FakeUserService();
            _friendsService = new FriendsService(_fakeFriendshipsRepository, _fakeUserService);
        }

        #region Exception Tests using Moq

        [Test]
        public void FriendsService_NullFriendshipRepository_ThrowsException()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(() => new FriendsService(null, new FakeUserService()));
        }

        [Test]
        public void FriendsService_NullUserService_ThrowsException()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(() => new FriendsService(new FakeFriendshipsRepository(), null));
        }

        [Test]
        public void GetAllFriendships_RepositoryThrows_ServiceExceptionThrown()
        {
            // Arrange
            var mockRepo = new Mock<IFriendshipsRepository>();
            mockRepo.Setup(r => r.GetAllFriendships(It.IsAny<int>()))
                    .Throws(new RepositoryException("Repository error"));
            var service = new FriendsService(mockRepo.Object, _fakeUserService);

            // Act & Assert (one assertion)
            var ex = Assert.Throws<ServiceException>(() => service.GetAllFriendships());
            Assert.That(ex.Message, Is.EqualTo("Error retrieving friendships for user."));
        }

        [Test]
        public void RemoveFriend_RepositoryThrows_ServiceExceptionThrown()
        {
            // Arrange
            var mockRepo = new Mock<IFriendshipsRepository>();
            mockRepo.Setup(r => r.RemoveFriendship(It.IsAny<int>()))
                    .Throws(new RepositoryException("Repository error"));
            var service = new FriendsService(mockRepo.Object, _fakeUserService);

            // Act & Assert
            var ex = Assert.Throws<ServiceException>(() => service.RemoveFriend(1));
            Assert.That(ex.Message, Is.EqualTo("Error removing friend."));
        }

        [Test]
        public void GetFriendshipCount_RepositoryThrows_ServiceExceptionThrown()
        {
            // Arrange
            var mockRepo = new Mock<IFriendshipsRepository>();
            mockRepo.Setup(r => r.GetFriendshipCount(It.IsAny<int>()))
                    .Throws(new RepositoryException("Repository error"));
            var service = new FriendsService(mockRepo.Object, _fakeUserService);

            // Act & Assert
            var ex = Assert.Throws<ServiceException>(() => service.GetFriendshipCount(1));
            Assert.That(ex.Message, Is.EqualTo("Error retrieving friendship count."));
        }

        [Test]
        public void AreUsersFriends_RepositoryThrows_ServiceExceptionThrown()
        {
            // Arrange
            var mockRepo = new Mock<IFriendshipsRepository>();
            mockRepo.Setup(r => r.GetAllFriendships(It.IsAny<int>()))
                    .Throws(new RepositoryException("Repository error"));
            var service = new FriendsService(mockRepo.Object, _fakeUserService);

            // Act & Assert
            var ex = Assert.Throws<ServiceException>(() => service.AreUsersFriends(1, 2));
            Assert.That(ex.Message, Is.EqualTo("Error checking friendship status."));
        }

        [Test]
        public void GetFriendshipId_RepositoryThrows_ServiceExceptionThrown()
        {
            // Arrange
            var mockRepo = new Mock<IFriendshipsRepository>();
            mockRepo.Setup(r => r.GetAllFriendships(It.IsAny<int>()))
                    .Throws(new RepositoryException("Repository error"));
            var service = new FriendsService(mockRepo.Object, _fakeUserService);

            // Act & Assert
            var ex = Assert.Throws<ServiceException>(() => service.GetFriendshipId(1, 2));
            Assert.That(ex.Message, Is.EqualTo("Error retrieving friendship ID."));
        }

        [Test]
        public void AddFriend_RepositoryThrows_ServiceExceptionThrown()
        {
            // Arrange
            var mockRepo = new Mock<IFriendshipsRepository>();
            mockRepo.Setup(r => r.AddFriendship(It.IsAny<int>(), It.IsAny<int>()))
                    .Throws(new RepositoryException("Repository error"));
            var service = new FriendsService(mockRepo.Object, _fakeUserService);

            // Act & Assert
            var ex = Assert.Throws<ServiceException>(() => service.AddFriend(1, 2));
            Assert.That(ex.Message, Is.EqualTo("Error adding friend."));
        }

        #endregion

        #region Normal Flow Tests

        // Split the previous GetAllFriendships test into separate tests.

        [Test]
        public void GetAllFriendships_ResultIsNotNull()
        {
            // Act
            List<Friendship> result = _friendsService.GetAllFriendships();
            // Assert: Result is not null.
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetAllFriendships_ResultCountIsNonNegative()
        {
            // Act
            List<Friendship> result = _friendsService.GetAllFriendships();
            // Assert: Count is greater than or equal to zero.
            Assert.That(result.Count, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public void GetAllFriendships_AllFriendshipsBelongToUser1()
        {
            // Act
            List<Friendship> result = _friendsService.GetAllFriendships();
            // Assert: Every friendship has UserId equal to 1.
            Assert.That(result, Has.All.Property("UserId").EqualTo(1));
        }

        [Test]
        public void RemoveFriend_RemovesFriendship_DecreasesCountByOne()
        {
            // Arrange
            List<Friendship> before = _fakeFriendshipsRepository.GetAllFriendships(1);
            int countBefore = before.Count;
            Friendship toRemove = before.First();
            // Act
            _friendsService.RemoveFriend(toRemove.FriendshipId);
            List<Friendship> after = _fakeFriendshipsRepository.GetAllFriendships(1);
            // Assert: Count decreased by one.
            Assert.That(after.Count, Is.EqualTo(countBefore - 1));
        }

        [Test]
        public void GetFriendshipCount_ReturnsCorrectCount()
        {
            // Act
            int count = _friendsService.GetFriendshipCount(1);
            // Arrange (from fake repo)
            List<Friendship> all = _fakeFriendshipsRepository.GetAllFriendships(1);
            int expected = all.Count;
            // Assert: Count equals expected.
            Assert.That(count, Is.EqualTo(expected));
        }

        [Test]
        public void AreUsersFriends_ReturnsTrueWhenFriends()
        {
            // Act
            bool result = _friendsService.AreUsersFriends(1, 2);
            // Assert: Should be true.
            Assert.That(result, Is.True);
        }

        [Test]
        public void AreUsersFriends_ReturnsFalseWhenNotFriends()
        {
            // Act
            bool result = _friendsService.AreUsersFriends(1, 999);
            // Assert: Should be false.
            Assert.That(result, Is.False);
        }

        // Splitting the GetFriendshipId tests.
        [Test]
        public void GetFriendshipId_ExistingRelationship_ReturnsNonNullId()
        {
            // Act
            int? id = _friendsService.GetFriendshipId(1, 2);
            // Assert: Id is not null.
            Assert.That(id, Is.Not.Null);
        }

        [Test]
        public void GetFriendshipId_ExistingRelationship_ReturnsExpectedId()
        {
            // Act
            int? id = _friendsService.GetFriendshipId(1, 2);
            // Assert: Expected id equals 1.
            Assert.That(id, Is.EqualTo(1));
        }

        [Test]
        public void GetFriendshipId_NonExistingRelationship_ReturnsNull()
        {
            // Act
            int? id = _friendsService.GetFriendshipId(1, 999);
            // Assert: Id is null.
            Assert.That(id, Is.Null);
        }

        [Test]
        public void AddFriend_AddsNewFriendship_IncreasesCountByOne()
        {
            // Arrange
            List<Friendship> before = _fakeFriendshipsRepository.GetAllFriendships(1);
            int countBefore = before.Count;
            // Act
            _friendsService.AddFriend(1, 4);
            List<Friendship> after = _fakeFriendshipsRepository.GetAllFriendships(1);
            // Assert: Count increased by one.
            Assert.That(after.Count, Is.EqualTo(countBefore + 1));
        }

        #endregion
    }
}
