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
            var game = CreateValidOwnedGame();

            // Act
            game.GameId = 10;

            // Assert
            Assert.That(game.GameId, Is.EqualTo(10));
        }

        [Test]
        public void UserId_SetValue_GetterReturnsSameValue()
        {
            // Arrange
            var game = CreateValidOwnedGame();

            // Act
            game.UserId = 99;

            // Assert
            Assert.That(game.UserId, Is.EqualTo(99));
        }

        [Test]
        public void Title_SetValue_GetterReturnsSameValue()
        {
            // Arrange
            var game = CreateValidOwnedGame();

            // Act
            game.Title = "Stardew Valley";

            // Assert
            Assert.That(game.Title, Is.EqualTo("Stardew Valley"));
        }

        [Test]
        public void Description_SetValue_GetterReturnsSameValue()
        {
            // Arrange
            var game = CreateValidOwnedGame();

            // Act
            game.Description = "Farming simulator";

            // Assert
            Assert.That(game.Description, Is.EqualTo("Farming simulator"));
        }

        [Test]
        public void CoverPicture_SetValue_GetterReturnsSameValue()
        {
            // Arrange
            var game = CreateValidOwnedGame();

            // Act
            game.CoverPicture = "https://example.com/pic.jpg";

            // Assert
            Assert.That(game.CoverPicture, Is.EqualTo("https://example.com/pic.jpg"));
        }

        [Test]
        public void Constructor_AssignsUserIdCorrectly()
        {
            // Arrange
            var expected = 7;

            // Act
            var game = new OwnedGame(expected, "Title", "Description");

            // Assert
            Assert.That(game.UserId, Is.EqualTo(expected));
        }

        [Test]
        public void Constructor_AssignsTitleCorrectly()
        {
            // Arrange
            var expected = "Terraria";

            // Act
            var game = new OwnedGame(1, expected, "Description");

            // Assert
            Assert.That(game.Title, Is.EqualTo(expected));
        }

        [Test]
        public void Constructor_AssignsDescriptionCorrectly()
        {
            // Arrange
            var expected = "Adventure game";

            // Act
            var game = new OwnedGame(1, "Title", expected);

            // Assert
            Assert.That(game.Description, Is.EqualTo(expected));
        }

        [Test]
        public void Constructor_AssignsCoverPictureCorrectly()
        {
            // Arrange
            var expected = "https://cover.jpg";

            // Act
            var game = new OwnedGame(1, "Title", "Description", expected);

            // Assert
            Assert.That(game.CoverPicture, Is.EqualTo(expected));
        }

        private OwnedGame CreateValidOwnedGame()
        {
            return new OwnedGame(1, "Test Game", "Test description");
        }
    }
}
