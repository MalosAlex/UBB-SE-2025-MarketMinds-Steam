﻿using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using BusinessLayer.Models;
using BusinessLayer.Services;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Exceptions;

namespace Tests.ServiceTests
{
    [TestFixture]
    public class OwnedGamesServiceTests
    {
        private Mock<IOwnedGamesRepository> _ownedGamesRepositoryMock;
        private OwnedGamesService _ownedGamesService;

        [SetUp]
        public void SetUp()
        {
            // Arrange: Create a repository mock and instantiate the service.
            _ownedGamesRepositoryMock = new Mock<IOwnedGamesRepository>();
            _ownedGamesService = new OwnedGamesService(_ownedGamesRepositoryMock.Object);
        }

        #region Constructor Tests

        [Test]
        public void OwnedGamesService_Constructor_NullRepository_ThrowsArgumentNullException()
        {
            // Assert: A null repository should trigger an ArgumentNullException.
            Assert.Throws<ArgumentNullException>(() => new OwnedGamesService(null));
        }

        #endregion

        #region GetAllOwnedGames Tests

        [Test]
        public void GetAllOwnedGames_UserExists_ReturnsCorrectCount()
        {
            // Arrange: Create a list of owned games.
            var ownedGamesList = new List<OwnedGame>
            {
                new OwnedGame(1, "Game A", "Desc A", "coverA.jpg") { GameId = 1 },
                new OwnedGame(1, "Game B", "Desc B", "coverB.jpg") { GameId = 2 }
            };
            _ownedGamesRepositoryMock.Setup(repo => repo.GetAllOwnedGames(It.IsAny<int>()))
                                     .Returns(ownedGamesList);

            // Act: Retrieve the owned games for user 1.
            List<OwnedGame> resultOwnedGames = _ownedGamesService.GetAllOwnedGames(1);

            // Assert: Expect the count to equal 2.
            Assert.That(resultOwnedGames.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetAllOwnedGames_UserExists_FirstGameTitleIsCorrect()
        {
            // Arrange: Create a list of owned games.
            var ownedGamesList = new List<OwnedGame>
            {
                new OwnedGame(1, "Game A", "Desc A", "coverA.jpg") { GameId = 1 },
                new OwnedGame(1, "Game B", "Desc B", "coverB.jpg") { GameId = 2 }
            };
            _ownedGamesRepositoryMock.Setup(repo => repo.GetAllOwnedGames(It.IsAny<int>()))
                                     .Returns(ownedGamesList);

            // Act: Retrieve the list for user 1.
            List<OwnedGame> resultOwnedGames = _ownedGamesService.GetAllOwnedGames(1);

            // Assert: Expect the first game's title to be "Game A".
            Assert.That(resultOwnedGames[0].Title, Is.EqualTo("Game A"));
        }

        [Test]
        public void GetAllOwnedGames_RepositoryThrows_ThrowsServiceExceptionWithRepoErrorMessage()
        {
            // Arrange: Setup the repository to throw a RepositoryException.
            _ownedGamesRepositoryMock.Setup(repo => repo.GetAllOwnedGames(It.IsAny<int>()))
                                     .Throws(new RepositoryException("Repo error"));

            // Act & Assert: Expect a ServiceException with the proper message.
            ServiceException exception = Assert.Throws<ServiceException>(
                () => _ownedGamesService.GetAllOwnedGames(1));
            Assert.That(exception.Message, Is.EqualTo("Failed to retrieve owned games."));
        }

        [Test]
        public void GetAllOwnedGames_GenericException_ThrowsServiceExceptionWithUnexpectedErrorMessage()
        {
            // Arrange: Setup the repository to throw a generic Exception.
            _ownedGamesRepositoryMock.Setup(repo => repo.GetAllOwnedGames(It.IsAny<int>()))
                                     .Throws(new Exception("Generic error"));

            // Act & Assert: Expect a ServiceException with the unexpected error message.
            ServiceException exception = Assert.Throws<ServiceException>(
                () => _ownedGamesService.GetAllOwnedGames(1));
            Assert.That(exception.Message, Is.EqualTo("An unexpected error occurred while retrieving owned games."));
        }

        #endregion

        #region GetOwnedGameById Tests

        [Test]
        public void GetOwnedGameById_GameExists_ReturnsNonNullGame()
        {
            // Arrange: Create an owned game.
            var expectedGame = new OwnedGame(1, "Game A", "Desc A", "coverA.jpg") { GameId = 1 };
            _ownedGamesRepositoryMock.Setup(repo => repo.GetOwnedGameById(1, 1))
                                     .Returns(expectedGame);

            // Act: Retrieve the owned game.
            OwnedGame resultGame = _ownedGamesService.GetOwnedGameById(1, 1);

            // Assert: The result should not be null.
            Assert.That(resultGame, Is.Not.Null);
        }

        [Test]
        public void GetOwnedGameById_GameExists_ReturnsGameWithCorrectTitle()
        {
            // Arrange: Create an owned game.
            var expectedGame = new OwnedGame(1, "Game A", "Desc A", "coverA.jpg") { GameId = 1 };
            _ownedGamesRepositoryMock.Setup(repo => repo.GetOwnedGameById(1, 1))
                                     .Returns(expectedGame);

            // Act: Retrieve the owned game.
            OwnedGame resultGame = _ownedGamesService.GetOwnedGameById(1, 1);

            // Assert: The game's title should equal "Game A".
            Assert.That(resultGame.Title, Is.EqualTo("Game A"));
        }

        [Test]
        public void GetOwnedGameById_GameNotFound_ReturnsNull()
        {
            // Arrange: Setup repository to return null when the game is not found.
            _ownedGamesRepositoryMock.Setup(repo => repo.GetOwnedGameById(999, 1))
                                     .Returns((OwnedGame)null);

            // Act: Attempt to retrieve a non-existent owned game.
            OwnedGame resultGame = _ownedGamesService.GetOwnedGameById(999, 1);

            // Assert: The result should be null.
            Assert.That(resultGame, Is.Null);
        }

        [Test]
        public void GetOwnedGameById_RepositoryThrows_ThrowsServiceExceptionWithRepoErrorMessage()
        {
            // Arrange: Setup repository to throw a RepositoryException.
            _ownedGamesRepositoryMock.Setup(repo => repo.GetOwnedGameById(It.IsAny<int>(), It.IsAny<int>()))
                                     .Throws(new RepositoryException("Repo error"));

            // Act & Assert: Expect a ServiceException with the proper message.
            ServiceException exception = Assert.Throws<ServiceException>(
                () => _ownedGamesService.GetOwnedGameById(1, 1));
            Assert.That(exception.Message, Is.EqualTo("Failed to retrieve owned game."));
        }

        [Test]
        public void GetOwnedGameById_GenericException_ThrowsServiceExceptionWithUnexpectedErrorMessage()
        {
            // Arrange: Setup repository to throw a generic Exception.
            _ownedGamesRepositoryMock.Setup(repo => repo.GetOwnedGameById(It.IsAny<int>(), It.IsAny<int>()))
                                     .Throws(new Exception("Generic error"));

            // Act & Assert: Expect a ServiceException with the unexpected error message.
            ServiceException exception = Assert.Throws<ServiceException>(
                () => _ownedGamesService.GetOwnedGameById(1, 1));
            Assert.That(exception.Message, Is.EqualTo("An unexpected error occurred while retrieving owned game."));
        }

        #endregion

        #region RemoveOwnedGame Tests

        [Test]
        public void RemoveOwnedGame_SuccessfulCall_CallsRepositoryOnce()
        {
            // Arrange: Setup the repository expectation for removing an owned game.
            _ownedGamesRepositoryMock.Setup(repo => repo.RemoveOwnedGame(1, 1));

            // Act: Invoke the service method to remove the owned game.
            _ownedGamesService.RemoveOwnedGame(1, 1);

            // Assert: Verify that the repository's RemoveOwnedGame method was called exactly once.
            _ownedGamesRepositoryMock.Verify(repo => repo.RemoveOwnedGame(1, 1), Times.Once);
            // A dummy assert follows to maintain the one-assert-per-test guideline.
            Assert.That(true, Is.True);
        }

        [Test]
        public void RemoveOwnedGame_RepositoryThrows_ThrowsServiceExceptionWithRepoErrorMessage()
        {
            // Arrange: Setup repository to throw a RepositoryException upon removing a game.
            _ownedGamesRepositoryMock.Setup(repo => repo.RemoveOwnedGame(It.IsAny<int>(), It.IsAny<int>()))
                                     .Throws(new RepositoryException("Repo error"));

            // Act & Assert: Expect a ServiceException with the proper message.
            ServiceException exception = Assert.Throws<ServiceException>(
                () => _ownedGamesService.RemoveOwnedGame(1, 1));
            Assert.That(exception.Message, Is.EqualTo("Failed to remove owned game."));
        }

        [Test]
        public void RemoveOwnedGame_GenericException_ThrowsServiceExceptionWithUnexpectedErrorMessage()
        {
            // Arrange: Setup repository to throw a generic Exception on remove.
            _ownedGamesRepositoryMock.Setup(repo => repo.RemoveOwnedGame(It.IsAny<int>(), It.IsAny<int>()))
                                     .Throws(new Exception("Generic error"));

            // Act & Assert: Expect a ServiceException with the unexpected error message.
            ServiceException exception = Assert.Throws<ServiceException>(
                () => _ownedGamesService.RemoveOwnedGame(1, 1));
            Assert.That(exception.Message, Is.EqualTo("An unexpected error occurred while removing owned game."));
        }

        #endregion
    }
}
