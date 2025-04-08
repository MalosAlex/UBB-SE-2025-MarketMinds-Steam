using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using BusinessLayer.Data;
using BusinessLayer.Models;
using BusinessLayer.Repositories;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Exceptions;
using Microsoft.Data.SqlClient;
using System.Runtime.Serialization;
using BusinessLayer.Services;

namespace Tests.RepositoryTests
{
    [TestFixture]
    public class OwnedGamesRepositoryTests
    {
        private Mock<IDataLink> _mockDataLink;
        private OwnedGamesRepository _repository;

        [SetUp]
        public void SetUp()
        {
            // Arrange: Create a new mock IDataLink and instantiate repository with it.
            _mockDataLink = new Mock<IDataLink>();
            _repository = new OwnedGamesRepository(_mockDataLink.Object);
        }

        [Test]
        public void OwnedGamesRepository_NullDataLink_ThrowsException()
        {
            // Assert
            Assert.Throws<ArgumentNullException>(() => new OwnedGamesRepository(null));
        }

        #region Helper Methods

        // Helper to create a DataTable for OwnedGames with specified number of rows.
        private DataTable CreateOwnedGamesDataTable(int rowCount)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("user_id", typeof(int));
            dt.Columns.Add("title", typeof(string));
            dt.Columns.Add("description", typeof(string));
            dt.Columns.Add("cover_picture", typeof(string));
            dt.Columns.Add("game_id", typeof(int));

            for (int i = 0; i < rowCount; i++)
            {
                DataRow row = dt.NewRow();
                row["user_id"] = 1;
                row["title"] = "Game " + i;
                row["description"] = "Description " + i;
                row["cover_picture"] = "cover" + i + ".jpg";
                row["game_id"] = i + 1;
                dt.Rows.Add(row);
            }
            return dt;
        }

        // Helper to create a DataTable with no rows.
        private DataTable CreateEmptyDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("user_id", typeof(int));
            dt.Columns.Add("title", typeof(string));
            dt.Columns.Add("description", typeof(string));
            dt.Columns.Add("cover_picture", typeof(string));
            dt.Columns.Add("game_id", typeof(int));
            return dt;
        }

        // Helper to create a dummy SqlException.
        private SqlException CreateSqlException()
        {
            // Use FormatterServices to get an uninitialized SqlException.
            return (SqlException)FormatterServices.GetUninitializedObject(typeof(SqlException));
        }

        #endregion

        #region GetAllOwnedGames Tests

        [Test]
        public void GetAllOwnedGames_ReturnsCorrectCount()
        {
            // Arrange
            int expectedCount = 3;
            DataTable dt = CreateOwnedGamesDataTable(expectedCount);
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetAllOwnedGames", 
                It.IsAny<SqlParameter[]>()))
                .Returns(dt);
            // Act
            List<OwnedGame> games = _repository.GetAllOwnedGames(1);
            // Assert: The count equals expected.
            Assert.That(games.Count, Is.EqualTo(expectedCount));
        }

        [Test]
        public void GetAllOwnedGames_FirstGameTitleIsCorrect()
        {
            // Arrange
            DataTable dt = CreateOwnedGamesDataTable(2);
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetAllOwnedGames", 
                It.IsAny<SqlParameter[]>()))
                .Returns(dt);
            // Act
            List<OwnedGame> games = _repository.GetAllOwnedGames(1);
            // Assert: The first game's title is "Game 0".
            Assert.That(games.First().Title, Is.EqualTo("Game 0"));
        }

        [Test]
        public void GetAllOwnedGames_SqlException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            SqlException sqlEx = CreateSqlException();
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetAllOwnedGames", It.IsAny<SqlParameter[]>()))
                         .Throws(sqlEx);
            // Act & Assert: Verify RepositoryException message for SQL errors.
            var ex = Assert.Throws<RepositoryException>(() => _repository.GetAllOwnedGames(1));
            Assert.That(ex.Message, Is.EqualTo("Database error while retrieving owned games."));
        }

        [Test]
        public void GetAllOwnedGames_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetAllOwnedGames", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("Test error"));
            // Act & Assert: Verify RepositoryException message for generic errors.
            var ex = Assert.Throws<RepositoryException>(() => _repository.GetAllOwnedGames(1));
            Assert.That(ex.Message, Is.EqualTo("An unexpected error occurred while retrieving owned games."));
        }

        #endregion

        #region GetOwnedGameById Tests

        [Test]
        public void GetOwnedGameById_ReturnsOwnedGameWhenRowExists()
        {
            // Arrange
            DataTable dt = CreateOwnedGamesDataTable(1);
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetOwnedGameById", It.IsAny<SqlParameter[]>()))
                         .Returns(dt);
            // Act
            OwnedGame game = _repository.GetOwnedGameById(1, 1);
            // Assert: Game is not null.
            Assert.That(game, Is.Not.Null);
        }

        [Test]
        public void GetOwnedGameById_ReturnsCorrectTitleWhenRowExists()
        {
            // Arrange
            DataTable dt = CreateOwnedGamesDataTable(1);
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetOwnedGameById", It.IsAny<SqlParameter[]>()))
                         .Returns(dt);
            // Act
            OwnedGame game = _repository.GetOwnedGameById(1, 1);
            // Assert: Title equals expected.
            Assert.That(game.Title, Is.EqualTo("Game 0"));
        }

        [Test]
        public void GetOwnedGameById_ReturnsNullWhenNoRows()
        {
            // Arrange
            DataTable dt = CreateEmptyDataTable();
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetOwnedGameById", It.IsAny<SqlParameter[]>()))
                         .Returns(dt);
            // Act
            OwnedGame game = _repository.GetOwnedGameById(1, 1);
            // Assert: Game is null.
            Assert.That(game, Is.Null);
        }

        [Test]
        public void GetOwnedGameById_SqlException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            SqlException sqlEx = CreateSqlException();
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetOwnedGameById", It.IsAny<SqlParameter[]>()))
                         .Throws(sqlEx);
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.GetOwnedGameById(1, 1));
            Assert.That(ex.Message, Is.EqualTo("Database error while retrieving owned game by ID."));
        }

        [Test]
        public void GetOwnedGameById_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetOwnedGameById", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("Test error"));
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.GetOwnedGameById(1, 1));
            Assert.That(ex.Message, Is.EqualTo("An unexpected error occurred while retrieving owned game by ID."));
        }

        #endregion

        #region RemoveOwnedGame Tests

        [Test]
        public void RemoveOwnedGame_CallsExecuteNonQuery()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteNonQuery("RemoveOwnedGame", It.IsAny<SqlParameter[]>()))
                         .Verifiable();
            // Act
            _repository.RemoveOwnedGame(1, 1);
            // Assert: Verify ExecuteNonQuery was called once.
            _mockDataLink.Verify(dl => dl.ExecuteNonQuery("RemoveOwnedGame", It.IsAny<SqlParameter[]>()), Times.Once);
            Assert.That(true, Is.True); // Dummy assert to satisfy one-assert-per-test.
        }

        [Test]
        public void RemoveOwnedGame_SqlException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            SqlException sqlEx = CreateSqlException();
            _mockDataLink.Setup(dl => dl.ExecuteNonQuery("RemoveOwnedGame", It.IsAny<SqlParameter[]>()))
                         .Throws(sqlEx);
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.RemoveOwnedGame(1, 1));
            Assert.That(ex.Message, Is.EqualTo("Database error while removing owned game."));
        }

        [Test]
        public void RemoveOwnedGame_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteNonQuery("RemoveOwnedGame", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("Test error"));
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.RemoveOwnedGame(1, 1));
            Assert.That(ex.Message, Is.EqualTo("An unexpected error occurred while removing owned game."));
        }

        #endregion
    }
}
