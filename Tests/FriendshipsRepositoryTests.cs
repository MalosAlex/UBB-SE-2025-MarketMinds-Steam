using BusinessLayer.Models;
using BusinessLayer.Repositories;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Data;

namespace Tests
{
    [TestFixture]
    public class FriendshipsRepositoryTests
    {
        private IFriendshipsRepository _friendshipsRepository;

        [SetUp]
        public void SetUp()
        {
            // Inject the FakeDataLink into the real repository.
            _friendshipsRepository = new FriendshipsRepository(new FakeDataLink());
        }

        [Test]
        public void GetAllFriendships_UserHasFriendships_ReturnsFriendships()
        {
            // Arrange
            int userId = 1;

            // Act
            List<Friendship> friendships = _friendshipsRepository.GetAllFriendships(userId);

            // Assert
            Assert.That(friendships, Is.Not.Null, "Friendships should not be null.");
            Assert.That(friendships.Count, Is.GreaterThanOrEqualTo(2), "There should be at least two friendships for user 1.");
            foreach (Friendship f in friendships)
            {
                Assert.That(f.UserId, Is.EqualTo(userId), "Each friendship should have the expected UserId.");
                // Check that friend username is populated (from GetUserById fake)
                Assert.That(f.FriendUsername, Is.Not.Null.And.Not.Empty, "Friend username should be populated.");
            }
        }

        [Test]
        public void AddFriendship_NewFriendship_IncreasesCount()
        {
            // Arrange
            int userId = 1;
            // Initially, user 1 has two friendships.
            int initialCount = _friendshipsRepository.GetFriendshipCount(userId);
            int newFriendId = 4; // User 4 exists in fake users

            // Act
            _friendshipsRepository.AddFriendship(userId, newFriendId);
            int finalCount = _friendshipsRepository.GetFriendshipCount(userId);

            // Assert
            Assert.That(finalCount, Is.EqualTo(initialCount + 1), "Friendship count should increase by one after adding a new friendship.");
        }

        [Test]
        public void AddFriendship_DuplicateFriendship_ThrowsException()
        {
            // Arrange
            int userId = 1;
            int friendId = 2; // Already exists.
            Exception caughtException = null;

            // Act
            try
            {
                _friendshipsRepository.AddFriendship(userId, friendId);
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            // Assert
            Assert.That(caughtException, Is.Not.Null, "An exception should be thrown for a duplicate friendship.");
            Assert.That(caughtException.Message, Is.EqualTo("Friendship already exists."), "Exception message should match expected text.");
        }

        [Test]
        public void GetFriendshipById_ExistingFriendship_ReturnsCorrectFriendship()
        {
            // Arrange
            List<Friendship> allFriendships = _friendshipsRepository.GetAllFriendships(1);
            Friendship existing = allFriendships.First();
            int friendshipId = existing.FriendshipId;

            // Act
            Friendship result = _friendshipsRepository.GetFriendshipById(friendshipId);

            // Assert
            Assert.That(result, Is.Not.Null, "Existing friendship should be returned.");
            Assert.That(result.FriendshipId, Is.EqualTo(friendshipId), "Returned friendship should have the correct ID.");
        }

        [Test]
        public void GetFriendshipById_NonExistingFriendship_ReturnsNull()
        {
            // Act
            Friendship result = _friendshipsRepository.GetFriendshipById(999);

            // Assert
            Assert.That(result, Is.Null, "Non-existing friendship should return null.");
        }

        [Test]
        public void RemoveFriendship_ExistingFriendship_DecreasesCount()
        {
            // Arrange
            int userId = 1;
            List<Friendship> friendshipsBefore = _friendshipsRepository.GetAllFriendships(userId);
            Friendship friendshipToRemove = friendshipsBefore.First();
            int countBefore = friendshipsBefore.Count;

            // Act
            _friendshipsRepository.RemoveFriendship(friendshipToRemove.FriendshipId);
            List<Friendship> friendshipsAfter = _friendshipsRepository.GetAllFriendships(userId);
            int countAfter = friendshipsAfter.Count;

            // Assert
            Assert.That(countAfter, Is.EqualTo(countBefore - 1), "Friendship count should decrease by one after removal.");
        }

        [Test]
        public void GetFriendshipCount_ReturnsCorrectCount()
        {
            // Arrange
            int userId = 1;

            // Act
            int count = _friendshipsRepository.GetFriendshipCount(userId);
            List<Friendship> allFriendships = _friendshipsRepository.GetAllFriendships(userId);
            int expectedCount = allFriendships.Count;

            // Assert
            Assert.That(count, Is.EqualTo(expectedCount), "Friendship count should match the actual number of friendships.");
        }

        [Test]
        public void GetFriendshipId_ExistingRelationship_ReturnsId()
        {
            // Arrange
            int userId = 1;
            int friendId = 2;

            // Act
            int? friendshipId = _friendshipsRepository.GetFriendshipId(userId, friendId);

            // Assert
            Assert.That(friendshipId, Is.Not.Null, "Existing relationship should return a friendship ID.");
            Assert.That(friendshipId, Is.EqualTo(1), "The friendship ID should be 1 as seeded.");
        }

        [Test]
        public void GetFriendshipId_NonExistingRelationship_ReturnsNull()
        {
            // Arrange
            int userId = 1;
            int friendId = 999;

            // Act
            int? friendshipId = _friendshipsRepository.GetFriendshipId(userId, friendId);

            // Assert
            Assert.That(friendshipId, Is.Null, "Non-existing relationship should return null.");
        }
    }
}
