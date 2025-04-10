using BusinessLayer.Models;
using NUnit.Framework;

namespace Tests.Models
{
    [TestFixture]
    public class OwnedGamePropertyTests
    {
        [Test]
        public void GameId_SetValue_GetterReturnsSameValue()
        {
            // Arrange
            var ownedGame = CreateValidOwnedGame();

            // Act
            ownedGame.GameId = 10;

            // Assert
            Assert.That(ownedGame.GameId, Is.EqualTo(10));
        }

        [Test]
        public void UserId_SetValue_GetterReturnsSameValue()
        {
            // Arrange
            var ownedGame = CreateValidOwnedGame();

            // Act
            ownedGame.UserId = 99;

            // Assert
            Assert.That(ownedGame.UserId, Is.EqualTo(99));
        }

        [Test]
        public void Title_SetValue_GetterReturnsSameValue()
        {
            // Arrange
            var ownedGame = CreateValidOwnedGame();

            // Act
            ownedGame.GameTitle = "Stardew Valley";

            // Assert
            Assert.That(ownedGame.GameTitle, Is.EqualTo("Stardew Valley"));
        }

        [Test]
        public void Description_SetValue_GetterReturnsSameValue()
        {
            // Arrange
            var ownedGame = CreateValidOwnedGame();

            // Act
            ownedGame.Description = "Farming simulator";

            // Assert
            Assert.That(ownedGame.Description, Is.EqualTo("Farming simulator"));
        }

        [Test]
        public void CoverPicture_SetValue_GetterReturnsSameValue()
        {
            // Arrange
            var ownedGame = CreateValidOwnedGame();

            // Act
            ownedGame.CoverPicture = "https://example.com/pic.jpg";

            // Assert
            Assert.That(ownedGame.CoverPicture, Is.EqualTo("https://example.com/pic.jpg"));
        }

        [Test]
        public void Constructor_AssignsUserIdCorrectly()
        {
            // Arrange
            var expected = 7;

            // Act
            var ownedGame = new OwnedGame(expected, "GameTitle", "Description");

            // Assert
            Assert.That(ownedGame.UserId, Is.EqualTo(expected));
        }

        [Test]
        public void Constructor_AssignsTitleCorrectly()
        {
            // Arrange
            var expected = "Terraria";

            // Act
            var ownedGame = new OwnedGame(1, expected, "Description");

            // Assert
            Assert.That(ownedGame.GameTitle, Is.EqualTo(expected));
        }

        [Test]
        public void Constructor_AssignsDescriptionCorrectly()
        {
            // Arrange
            var expected = "Adventure game";

            // Act
            var ownedGame = new OwnedGame(1, "GameTitle", expected);

            // Assert
            Assert.That(ownedGame.Description, Is.EqualTo(expected));
        }

        [Test]
        public void Constructor_AssignsCoverPictureCorrectly()
        {
            // Arrange
            var expected = "https://cover.jpg";

            // Act
            var ownedGame = new OwnedGame(1, "GameTitle", "Description", expected);

            // Assert
            Assert.That(ownedGame.CoverPicture, Is.EqualTo(expected));
        }

        private OwnedGame CreateValidOwnedGame()
        {
            return new OwnedGame(1, "Test Game", "Test description");
        }
    }
}
