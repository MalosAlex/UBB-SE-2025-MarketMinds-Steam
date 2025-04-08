using System.Collections.Generic;
using NUnit.Framework;
using BusinessLayer.Models;
using BusinessLayer.Services.Fakes;
using BusinessLayer.Services.Interfaces;

namespace Tests.ServiceTests
{
    [TestFixture]
    public class FakeOwnedGamesServiceTests
    {
        private IOwnedGamesService _fakeOwnedGamesService;

        [SetUp]
        public void SetUp()
        {
            _fakeOwnedGamesService = new FakeOwnedGamesService();
        }

        [Test]
        public void GetAllOwnedGames_ReturnsCorrectCountForUser1()
        {
            // Act: For userId = 1, seeded data contains 2 games.
            var result = _fakeOwnedGamesService.GetAllOwnedGames(1);
            // Assert count equals 2.
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetAllOwnedGames_AllReturnedGamesHaveUserId1()
        {
            // Act
            var result = _fakeOwnedGamesService.GetAllOwnedGames(1);
            // Assert each returned game has UserId == 1.
            Assert.That(result.TrueForAll(game => game.UserId == 1), Is.True);
        }

        [Test]
        public void GetOwnedGameById_ReturnsGame_WhenExists()
        {
            // Act: For userId = 1, GameId 100 exists.
            var game = _fakeOwnedGamesService.GetOwnedGameById(100, 1);
            // Assert game is not null.
            Assert.That(game, Is.Not.Null);
        }

        [Test]
        public void GetOwnedGameById_ReturnsGameWithCorrectGameId_WhenExists()
        {
            // Act
            var game = _fakeOwnedGamesService.GetOwnedGameById(100, 1);
            // Assert GameId equals 100.
            Assert.That(game.GameId, Is.EqualTo(100));
        }

        [Test]
        public void GetOwnedGameById_ReturnsNull_WhenGameDoesNotExist()
        {
            // Act: For userId = 1, GameId 999 does not exist.
            var game = _fakeOwnedGamesService.GetOwnedGameById(999, 1);
            // Assert game is null.
            Assert.That(game, Is.Null);
        }

        [Test]
        public void RemoveOwnedGame_DecreasesCount_WhenGameExists()
        {
            // Arrange: Get initial count.
            int initialCount = _fakeOwnedGamesService.GetAllOwnedGames(1).Count;
            // Act: Remove an existing game (GameId 100).
            _fakeOwnedGamesService.RemoveOwnedGame(100, 1);
            int afterCount = _fakeOwnedGamesService.GetAllOwnedGames(1).Count;
            // Assert that count decreased by one.
            Assert.That(afterCount, Is.EqualTo(initialCount - 1));
        }

        [Test]
        public void RemoveOwnedGame_RemovedGameIsNotRetrievable()
        {
            // Act: Remove game with GameId 100 for userId 1.
            _fakeOwnedGamesService.RemoveOwnedGame(100, 1);
            var game = _fakeOwnedGamesService.GetOwnedGameById(100, 1);
            // Assert that the removed game is null.
            Assert.That(game, Is.Null);
        }

        [Test]
        public void RemoveOwnedGame_DoesNothing_WhenGameNotFound()
        {
            // Arrange: Get initial count.
            int initialCount = _fakeOwnedGamesService.GetAllOwnedGames(1).Count;
            // Act: Attempt to remove a game that does not exist.
            _fakeOwnedGamesService.RemoveOwnedGame(999, 1);
            int afterCount = _fakeOwnedGamesService.GetAllOwnedGames(1).Count;
            // Assert that the count remains unchanged.
            Assert.That(afterCount, Is.EqualTo(initialCount));
        }
    }
}
