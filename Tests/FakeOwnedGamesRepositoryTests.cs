using NUnit.Framework;
using BusinessLayer.Models;
using BusinessLayer.Repositories.Fakes;

namespace Tests
{
    [TestFixture]
    public class FakeOwnedGamesRepositoryTests
    {
        private FakeOwnedGamesRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _repository = new FakeOwnedGamesRepository();
        }

        [Test]
        public void GetAllOwnedGames_WithValidUserId_ReturnsCorrectCount()
        {
            // Arrange
            int userId = 1;

            // Act
            var result = _repository.GetAllOwnedGames(userId);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetAllOwnedGames_WithNoOwnedGames_ReturnsEmptyList()
        {
            // Arrange
            int userId = 999;

            // Act
            var result = _repository.GetAllOwnedGames(userId);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetOwnedGameById_WithValidIdAndUser_ReturnsCorrectGame()
        {
            // Arrange
            int userId = 1;
            int gameId = 1;

            // Act
            var result = _repository.GetOwnedGameById(gameId, userId);

            // Assert
            Assert.That(result?.GameId, Is.EqualTo(gameId));
        }

        [Test]
        public void GetOwnedGameById_WithInvalidUser_ReturnsNull()
        {
            // Arrange
            int gameId = 1;
            int userId = 999;

            // Act
            var result = _repository.GetOwnedGameById(gameId, userId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void RemoveOwnedGame_RemovesGameSuccessfully()
        {
            // Arrange
            int userId = 1;
            int gameId = 1;

            // Act
            _repository.RemoveOwnedGame(gameId, userId);
            var result = _repository.GetOwnedGameById(gameId, userId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void RemoveOwnedGame_WithInvalidGame_DoesNothing()
        {
            // Arrange
            int userId = 1;
            int gameId = 999;

            // Act
            _repository.RemoveOwnedGame(gameId, userId);
            var result = _repository.GetAllOwnedGames(userId);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }
    }
}
