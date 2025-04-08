using NUnit.Framework;
using BusinessLayer.Models;
using BusinessLayer.Repositories.Fakes;
using System.Linq;

namespace Tests.RepositoryTests
{
    [TestFixture]
    public class FakeFriendshipsRepositoryTests
    {
        private FakeFriendshipsRepository _repo;

        [SetUp]
        public void SetUp()
        {
            _repo = new FakeFriendshipsRepository();
        }

        [Test]
        public void GetAllFriendships_ReturnsCorrectCount_ForUser1()
        {
            // Act
            var friendships = _repo.GetAllFriendships(1);
            // Assert: seeded data has 2 friendships for user 1.
            Assert.That(friendships.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetAllFriendships_OrdersByFriendUsername()
        {
            // Act
            var friendships = _repo.GetAllFriendships(1);
            // Assert: The first friendship should be "Alice" (alphabetically before "Bob").
            Assert.That(friendships.First().FriendUsername, Is.EqualTo("Alice"));
        }

        [Test]
        public void AddFriendship_AddsNewFriendship_ForUser1()
        {
            // Arrange
            int beforeCount = _repo.GetFriendshipCount(1);
            // Act
            _repo.AddFriendship(1, 4);
            // Assert: Count increases by 1.
            int afterCount = _repo.GetFriendshipCount(1);
            Assert.That(afterCount, Is.EqualTo(beforeCount + 1));
        }

        [Test]
        public void AddFriendship_ThrowsException_WhenFriendshipAlreadyExists()
        {
            // Act & Assert: Adding a duplicate friendship should throw an exception.
            var ex = Assert.Throws<Exception>(() => _repo.AddFriendship(1, 2));
            Assert.That(ex.Message, Is.EqualTo("Friendship already exists."));
        }

        [Test]
        public void GetFriendshipById_ReturnsFriendship_WhenItExists()
        {
            // Act
            var friendship = _repo.GetFriendshipById(1);
            // Assert: Friendship with ID 1 should be returned.
            Assert.That(friendship.FriendshipId, Is.EqualTo(1));
        }

        [Test]
        public void GetFriendshipById_ReturnsNull_WhenItDoesNotExist()
        {
            // Act
            var friendship = _repo.GetFriendshipById(999);
            // Assert: Should return null.
            Assert.That(friendship, Is.Null);
        }

        [Test]
        public void RemoveFriendship_RemovesFriendshipSuccessfully()
        {
            // Arrange
            int beforeCount = _repo.GetFriendshipCount(1);
            // Act
            _repo.RemoveFriendship(1);
            // Assert: Count decreases by 1.
            int afterCount = _repo.GetFriendshipCount(1);
            Assert.That(afterCount, Is.EqualTo(beforeCount - 1));
        }

        [Test]
        public void GetFriendshipCount_ReturnsCorrectCount_ForUser1()
        {
            // Act
            int count = _repo.GetFriendshipCount(1);
            // Assert: Should return 2 (seeded count).
            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public void GetFriendshipId_ReturnsCorrectId_WhenFriendshipExists()
        {
            // Act
            int? friendshipId = _repo.GetFriendshipId(1, 2);
            // Assert: For user 1 and friend 2, the seeded friendship has ID 1.
            Assert.That(friendshipId, Is.EqualTo(1));
        }

        [Test]
        public void GetFriendshipId_ReturnsNull_WhenFriendshipDoesNotExist()
        {
            // Act
            int? friendshipId = _repo.GetFriendshipId(1, 999);
            // Assert: No friendship exists; should return null.
            Assert.That(friendshipId, Is.Null);
        }
    }
}
