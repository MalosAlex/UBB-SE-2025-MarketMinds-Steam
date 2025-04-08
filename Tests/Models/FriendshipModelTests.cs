using BusinessLayer.Models;
using NUnit.Framework;

namespace Tests.Models
{
    [TestFixture]
    public class FriendshipPropertyTests
    {
        [Test]
        public void FriendshipId_SetValue_GetterReturnsSameValue()
        {
            // Arrange
            var friendship = CreateValidFriendship();

            // Act
            friendship.FriendshipId = 101;

            // Assert
            Assert.That(friendship.FriendshipId, Is.EqualTo(101));
        }

        [Test]
        public void UserId_SetValue_GetterReturnsSameValue()
        {
            // Arrange
            var friendship = CreateValidFriendship();

            // Act
            friendship.UserId = 77;

            // Assert
            Assert.That(friendship.UserId, Is.EqualTo(77));
        }

        [Test]
        public void FriendId_SetValue_GetterReturnsSameValue()
        {
            // Arrange
            var friendship = CreateValidFriendship();

            // Act
            friendship.FriendId = 88;

            // Assert
            Assert.That(friendship.FriendId, Is.EqualTo(88));
        }

        [Test]
        public void FriendUsername_SetValue_GetterReturnsSameValue()
        {
            // Arrange
            var friendship = CreateValidFriendship();

            // Act
            friendship.FriendUsername = "gamer123";

            // Assert
            Assert.That(friendship.FriendUsername, Is.EqualTo("gamer123"));
        }

        [Test]
        public void FriendProfilePicture_SetValue_GetterReturnsSameValue()
        {
            // Arrange
            var friendship = CreateValidFriendship();

            // Act
            friendship.FriendProfilePicture = "https://profile.img/user.png";

            // Assert
            Assert.That(friendship.FriendProfilePicture, Is.EqualTo("https://profile.img/user.png"));
        }

        [Test]
        public void Constructor_AssignsFriendshipIdCorrectly()
        {
            // Arrange
            var expected = 5;

            // Act
            var friendship = new Friendship(expected, 1, 2);

            // Assert
            Assert.That(friendship.FriendshipId, Is.EqualTo(expected));
        }

        [Test]
        public void Constructor_AssignsUserIdCorrectly()
        {
            // Arrange
            var expected = 10;

            // Act
            var friendship = new Friendship(1, expected, 2);

            // Assert
            Assert.That(friendship.UserId, Is.EqualTo(expected));
        }

        [Test]
        public void Constructor_AssignsFriendIdCorrectly()
        {
            // Arrange
            var expected = 20;

            // Act
            var friendship = new Friendship(1, 1, expected);

            // Assert
            Assert.That(friendship.FriendId, Is.EqualTo(expected));
        }

        [Test]
        public void Constructor_AssignsFriendUsernameCorrectly()
        {
            // Arrange
            var expected = "co_op_buddy";

            // Act
            var friendship = new Friendship(1, 1, 1, expected);

            // Assert
            Assert.That(friendship.FriendUsername, Is.EqualTo(expected));
        }

        [Test]
        public void Constructor_AssignsFriendProfilePictureCorrectly()
        {
            // Arrange
            var expected = "https://img.url/pic.png";

            // Act
            var friendship = new Friendship(1, 1, 1, "user", expected);

            // Assert
            Assert.That(friendship.FriendProfilePicture, Is.EqualTo(expected));
        }

        private Friendship CreateValidFriendship()
        {
            return new Friendship(1, 1, 2, "testuser", "https://img.test/user.png");
        }
    }
}
