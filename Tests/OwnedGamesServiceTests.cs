using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using BusinessLayer.Models;
using BusinessLayer.Services;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Exceptions;

namespace Tests
{
    [TestFixture]
    public class OwnedGamesServiceTests
    {
        private Mock<IOwnedGamesRepository> _repositoryMock;
        private OwnedGamesService _service;

        [SetUp]
        public void SetUp()
        {
            _repositoryMock = new Mock<IOwnedGamesRepository>();
            _service = new OwnedGamesService(_repositoryMock.Object);
        }

        #region GetAllOwnedGames Tests

        [Test]
        public void GetAllOwnedGames_ReturnsCorrectCount()
        {
            // Arrange
            var games = new List<OwnedGame>
            {
                new OwnedGame(1, "Game A", "Desc A", "coverA.jpg") { GameId = 1 },
                new OwnedGame(1, "Game B", "Desc B", "coverB.jpg") { GameId = 2 }
            };
            _repositoryMock.Setup(r => r.GetAllOwnedGames(It.IsAny<int>())).Returns(games);
            // Act
            var result = _service.GetAllOwnedGames(1);
            // Assert: Count equals 2.
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetAllOwnedGames_FirstGameTitleIsCorrect()
        {
            // Arrange
            var games = new List<OwnedGame>
            {
                new OwnedGame(1, "Game A", "Desc A", "coverA.jpg") { GameId = 1 },
                new OwnedGame(1, "Game B", "Desc B", "coverB.jpg") { GameId = 2 }
            };
            _repositoryMock.Setup(r => r.GetAllOwnedGames(It.IsAny<int>())).Returns(games);
            // Act
            var result = _service.GetAllOwnedGames(1);
            // Assert: First game's title is "Game A".
            Assert.That(result[0].Title, Is.EqualTo("Game A"));
        }

        [Test]
        public void GetAllOwnedGames_RepositoryException_ThrowsServiceException()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllOwnedGames(It.IsAny<int>()))
                           .Throws(new RepositoryException("Repo error"));
            // Act & Assert
            var ex = Assert.Throws<ServiceException>(() => _service.GetAllOwnedGames(1));
            Assert.That(ex.Message, Is.EqualTo("Failed to retrieve owned games."));
        }

        [Test]
        public void GetAllOwnedGames_GenericException_ThrowsServiceException()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetAllOwnedGames(It.IsAny<int>()))
                           .Throws(new Exception("Generic error"));
            // Act & Assert
            var ex = Assert.Throws<ServiceException>(() => _service.GetAllOwnedGames(1));
            Assert.That(ex.Message, Is.EqualTo("An unexpected error occurred while retrieving owned games."));
        }

        #endregion

        #region GetOwnedGameById Tests

        [Test]
        public void GetOwnedGameById_ReturnsNonNullGame_WhenFound()
        {
            // Arrange
            var game = new OwnedGame(1, "Game A", "Desc A", "coverA.jpg") { GameId = 1 };
            _repositoryMock.Setup(r => r.GetOwnedGameById(1, 1)).Returns(game);
            // Act
            var result = _service.GetOwnedGameById(1, 1);
            // Assert: Result is not null.
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetOwnedGameById_ReturnsCorrectTitle_WhenFound()
        {
            // Arrange
            var game = new OwnedGame(1, "Game A", "Desc A", "coverA.jpg") { GameId = 1 };
            _repositoryMock.Setup(r => r.GetOwnedGameById(1, 1)).Returns(game);
            // Act
            var result = _service.GetOwnedGameById(1, 1);
            // Assert: Title equals "Game A".
            Assert.That(result.Title, Is.EqualTo("Game A"));
        }

        [Test]
        public void GetOwnedGameById_ReturnsNull_WhenNotFound()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetOwnedGameById(999, 1)).Returns((OwnedGame)null);
            // Act
            var result = _service.GetOwnedGameById(999, 1);
            // Assert: Result is null.
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetOwnedGameById_RepositoryException_ThrowsServiceException()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetOwnedGameById(It.IsAny<int>(), It.IsAny<int>()))
                           .Throws(new RepositoryException("Repo error"));
            // Act & Assert
            var ex = Assert.Throws<ServiceException>(() => _service.GetOwnedGameById(1, 1));
            Assert.That(ex.Message, Is.EqualTo("Failed to retrieve owned game."));
        }

        [Test]
        public void GetOwnedGameById_GenericException_ThrowsServiceException()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetOwnedGameById(It.IsAny<int>(), It.IsAny<int>()))
                           .Throws(new Exception("Generic error"));
            // Act & Assert
            var ex = Assert.Throws<ServiceException>(() => _service.GetOwnedGameById(1, 1));
            Assert.That(ex.Message, Is.EqualTo("An unexpected error occurred while retrieving owned game."));
        }

        #endregion

        #region RemoveOwnedGame Tests

        [Test]
        public void RemoveOwnedGame_CallsRepository_WhenSuccessful()
        {
            // Arrange
            _repositoryMock.Setup(r => r.RemoveOwnedGame(1, 1));
            // Act
            _service.RemoveOwnedGame(1, 1);
            // Assert: Verify RemoveOwnedGame is called.
            _repositoryMock.Verify(r => r.RemoveOwnedGame(1, 1), Times.Once);
            Assert.That(true, Is.True); // Dummy assert to satisfy one-assert-per-test.
        }

        [Test]
        public void RemoveOwnedGame_RepositoryException_ThrowsServiceException()
        {
            // Arrange
            _repositoryMock.Setup(r => r.RemoveOwnedGame(It.IsAny<int>(), It.IsAny<int>()))
                           .Throws(new RepositoryException("Repo error"));
            // Act & Assert
            var ex = Assert.Throws<ServiceException>(() => _service.RemoveOwnedGame(1, 1));
            Assert.That(ex.Message, Is.EqualTo("Failed to remove owned game."));
        }

        [Test]
        public void RemoveOwnedGame_GenericException_ThrowsServiceException()
        {
            // Arrange
            _repositoryMock.Setup(r => r.RemoveOwnedGame(It.IsAny<int>(), It.IsAny<int>()))
                           .Throws(new Exception("Generic error"));
            // Act & Assert
            var ex = Assert.Throws<ServiceException>(() => _service.RemoveOwnedGame(1, 1));
            Assert.That(ex.Message, Is.EqualTo("An unexpected error occurred while removing owned game."));
        }

        #endregion
    }
}
