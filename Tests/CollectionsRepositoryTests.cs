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

namespace Tests
{
    [TestFixture]
    public class CollectionsRepositoryTests
    {
        private Mock<IDataLink> _mockDataLink;
        private CollectionsRepository _repository;

        [SetUp]
        public void SetUp()
        {
            // Arrange: create a new mock IDataLink and instantiate CollectionsRepository.
            _mockDataLink = new Mock<IDataLink>();
            _repository = new CollectionsRepository(_mockDataLink.Object);
        }

        #region Helper Methods

        private DataTable CreateCollectionsDataTable(int rowCount)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("user_id", typeof(int));
            dt.Columns.Add("name", typeof(string));
            dt.Columns.Add("created_at", typeof(DateTime));
            dt.Columns.Add("cover_picture", typeof(string));
            dt.Columns.Add("is_public", typeof(bool));
            dt.Columns.Add("collection_id", typeof(int));

            for (int i = 0; i < rowCount; i++)
            {
                DataRow row = dt.NewRow();
                row["user_id"] = 1;
                row["name"] = "Collection " + i;
                // set CreatedAt so that later tests for ordering can work
                row["created_at"] = DateTime.Now.AddDays(-i);
                row["cover_picture"] = "cover" + i + ".jpg";
                row["is_public"] = i % 2 == 0;
                row["collection_id"] = i + 1;
                dt.Rows.Add(row);
            }
            return dt;
        }

        private DataTable CreateEmptyCollectionsDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("user_id", typeof(int));
            dt.Columns.Add("name", typeof(string));
            dt.Columns.Add("created_at", typeof(DateTime));
            dt.Columns.Add("cover_picture", typeof(string));
            dt.Columns.Add("is_public", typeof(bool));
            dt.Columns.Add("collection_id", typeof(int));
            return dt;
        }

        private DataTable CreateGamesDataTable(int rowCount)
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
                row["cover_picture"] = "gamecover" + i + ".jpg";
                row["game_id"] = i + 1;
                dt.Rows.Add(row);
            }
            return dt;
        }

        private DataTable CreateEmptyGamesDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("user_id", typeof(int));
            dt.Columns.Add("title", typeof(string));
            dt.Columns.Add("description", typeof(string));
            dt.Columns.Add("cover_picture", typeof(string));
            dt.Columns.Add("game_id", typeof(int));
            return dt;
        }

        private SqlException CreateSqlException()
        {
            // Create an uninitialized SqlException
            return (SqlException)FormatterServices.GetUninitializedObject(typeof(SqlException));
        }

        #endregion

        #region GetAllCollections Tests

        [Test]
        public void GetAllCollections_ReturnsEmptyList_WhenDataTableIsNull()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetAllCollectionsForUser", It.IsAny<SqlParameter[]>()))
                         .Returns((DataTable)null);
            // Act
            var result = _repository.GetAllCollections(1);
            // Assert: list is empty.
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetAllCollections_ReturnsEmptyList_WhenDataTableHasNoRows()
        {
            // Arrange
            DataTable dt = CreateEmptyCollectionsDataTable();
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetAllCollectionsForUser", It.IsAny<SqlParameter[]>()))
                         .Returns(dt);
            // Act
            var result = _repository.GetAllCollections(1);
            // Assert: list is empty.
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetAllCollections_ReturnsCorrectCount_WhenDataTableHasRows()
        {
            // Arrange
            int expectedCount = 5;
            DataTable dt = CreateCollectionsDataTable(expectedCount);
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetAllCollectionsForUser", It.IsAny<SqlParameter[]>()))
                         .Returns(dt);
            // Act
            var result = _repository.GetAllCollections(1);
            // Assert: list count equals expected.
            Assert.That(result.Count, Is.EqualTo(expectedCount));
        }

        [Test]
        public void GetAllCollections_SqlException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            SqlException sqlEx = CreateSqlException();
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetAllCollectionsForUser", It.IsAny<SqlParameter[]>()))
                         .Throws(sqlEx);
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.GetAllCollections(1));
            Assert.That(ex.Message, Is.EqualTo("Database error while retrieving collections."));
        }

        [Test]
        public void GetAllCollections_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetAllCollectionsForUser", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("Test error"));
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.GetAllCollections(1));
            Assert.That(ex.Message, Is.EqualTo("An unexpected error occurred while retrieving collections."));
        }

        #endregion

        #region GetLastThreeCollectionsForUser Tests

        [Test]
        public void GetLastThreeCollectionsForUser_ReturnsOnlyThreeCollections()
        {
            // Arrange
            int total = 5;
            DataTable dt = CreateCollectionsDataTable(total);
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetAllCollectionsForUser", It.IsAny<SqlParameter[]>()))
                         .Returns(dt);
            // Act
            var result = _repository.GetLastThreeCollectionsForUser(1);
            // Assert: only three collections are returned.
            Assert.That(result.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetLastThreeCollectionsForUser_ReturnsMostRecentCollections_FirstHasLatestCreatedAt()
        {
            // Arrange
            // Create 3 collections with known CreatedAt values.
            DataTable dt = CreateCollectionsDataTable(3);
            // Modify created_at so that the first row is the oldest and last is the newest.
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["created_at"] = DateTime.Now.AddDays(-10 + i);
            }
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetAllCollectionsForUser", It.IsAny<SqlParameter[]>()))
                         .Returns(dt);
            // Act
            var result = _repository.GetLastThreeCollectionsForUser(1);
            // Assert: the first collection in result has the greatest created_at.
            Assert.That(result.First().CreatedAt, Is.EqualTo(DateOnly.FromDateTime(DateTime.Now.AddDays(-8))));
        }

        [Test]
        public void GetLastThreeCollectionsForUser_Exception_ThrowsRepositoryException()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetAllCollectionsForUser", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("Test error"));
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.GetLastThreeCollectionsForUser(1));
            Assert.That(ex.Message, Is.EqualTo("An unexpected error occurred while retrieving the last three collections."));
        }

        #endregion

        #region GetCollectionById Tests

        [Test]
        public void GetCollectionById_ReturnsNull_WhenDataTableHasNoRows()
        {
            // Arrange
            DataTable dt = CreateEmptyCollectionsDataTable();
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetCollectionById", It.IsAny<SqlParameter[]>()))
                         .Returns(dt);
            // Act
            var result = _repository.GetCollectionById(1, 1);
            // Assert: result is null.
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetCollectionById_ReturnsCollection_WhenRowExists()
        {
            // Arrange
            DataTable dt = CreateCollectionsDataTable(1);
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetCollectionById", It.IsAny<SqlParameter[]>()))
                         .Returns(dt);
            // Act
            var result = _repository.GetCollectionById(1, 1);
            // Assert: collection is not null.
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetCollectionById_SqlException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            SqlException sqlEx = CreateSqlException();
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetCollectionById", It.IsAny<SqlParameter[]>()))
                         .Throws(sqlEx);
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.GetCollectionById(1, 1));
            Assert.That(ex.Message, Is.EqualTo("Database error while retrieving collection by ID."));
        }

        [Test]
        public void GetCollectionById_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetCollectionById", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("Test error"));
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.GetCollectionById(1, 1));
            Assert.That(ex.Message, Is.EqualTo("An unexpected error occurred while retrieving collection by ID."));
        }

        #endregion

        #region GetGamesInCollection (single parameter) Tests
        
        [Test]
        public void GetGamesInCollection_ReturnsEmptyList_WhenDataTableHasNoRows()
        {
            // Arrange: Create an empty DataTable with expected columns.
            DataTable emptyTable = new DataTable();
            emptyTable.Columns.Add("user_id", typeof(int));
            emptyTable.Columns.Add("title", typeof(string));
            emptyTable.Columns.Add("description", typeof(string));
            emptyTable.Columns.Add("cover_picture", typeof(string));
            emptyTable.Columns.Add("game_id", typeof(int));
    
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetGamesInCollection", It.IsAny<SqlParameter[]>()))
                .Returns(emptyTable);
            // Act
            var result = _repository.GetGamesInCollection(1);
            // Assert: Verify the returned list is empty.
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetGamesInCollection_ReturnsEmptyList_WhenDataTableHasNoRows_SingleParam()
        {
            // Arrange
            DataTable dt = CreateEmptyGamesDataTable();
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetGamesInCollection", It.IsAny<SqlParameter[]>()))
                         .Returns(dt);
            // Act
            var result = _repository.GetGamesInCollection(1);
            // Assert: result is empty.
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetGamesInCollection_ReturnsCorrectCount_WhenDataTableHasRows_SingleParam()
        {
            // Arrange
            int expectedCount = 4;
            DataTable dt = CreateGamesDataTable(expectedCount);
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetGamesInCollection", It.IsAny<SqlParameter[]>()))
                         .Returns(dt);
            // Act
            var result = _repository.GetGamesInCollection(1);
            // Assert: count equals expected.
            Assert.That(result.Count, Is.EqualTo(expectedCount));
        }

        [Test]
        public void GetGamesInCollection_SqlException_ThrowsRepositoryExceptionWithDatabaseErrorMessage_SingleParam()
        {
            // Arrange
            SqlException sqlEx = CreateSqlException();
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetGamesInCollection", It.IsAny<SqlParameter[]>()))
                         .Throws(sqlEx);
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.GetGamesInCollection(1));
            Assert.That(ex.Message, Is.EqualTo("Database error while retrieving games in collection."));
        }

        [Test]
        public void GetGamesInCollection_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage_SingleParam()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetGamesInCollection", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("Test error"));
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.GetGamesInCollection(1));
            Assert.That(ex.Message, Is.EqualTo("An unexpected error occurred while retrieving games in collection."));
        }
        
        [Test]
        public void GetGamesInCollection_ReturnsEmptyList_WhenDataTableIsNull()
        {
            // Arrange: Return null instead of a DataTable
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetGamesInCollection", It.IsAny<SqlParameter[]>()))
                .Returns((DataTable)null);
    
            // Act
            var result = _repository.GetGamesInCollection(1);
    
            // Assert: The returned list should be empty
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetGamesInCollection_WithCollectionIdOne_ReturnsEmptyList_WhenDataTableIsNull()
        {
            // Arrange: Return null instead of a DataTable
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetAllGamesForUser", It.IsAny<SqlParameter[]>()))
                .Returns((DataTable)null);
    
            // Act
            var result = _repository.GetGamesInCollection(1, 1);
    
            // Assert: The returned list should be empty
            Assert.That(result, Is.Empty);
        }

        #endregion

        #region GetGamesInCollection (two parameters) Tests

        [Test]
        public void GetGamesInCollection_WithCollectionIdOne_ReturnsAllGamesForUser()
        {
            // Arrange
            int expectedCount = 3;
            DataTable dt = CreateGamesDataTable(expectedCount);
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetAllGamesForUser", It.IsAny<SqlParameter[]>()))
                         .Returns(dt);
            // Act
            var result = _repository.GetGamesInCollection(1, 1);
            // Assert: count equals expected.
            Assert.That(result.Count, Is.EqualTo(expectedCount));
        }

        [Test]
        public void GetGamesInCollection_WithCollectionIdNotOne_ReturnsGamesInCollection_SameAsSingleParam()
        {
            // Arrange
            int expectedCount = 2;
            DataTable dt = CreateGamesDataTable(expectedCount);
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetGamesInCollection", It.IsAny<SqlParameter[]>()))
                         .Returns(dt);
            // Act
            var result = _repository.GetGamesInCollection(2, 1);
            // Assert: count equals expected.
            Assert.That(result.Count, Is.EqualTo(expectedCount));
        }

        [Test]
        public void GetGamesInCollection_WithCollectionIdOne_SqlException_ThrowsRepositoryException()
        {
            // Arrange
            SqlException sqlEx = CreateSqlException();
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetAllGamesForUser", It.IsAny<SqlParameter[]>()))
                         .Throws(sqlEx);
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.GetGamesInCollection(1, 1));
            Assert.That(ex.Message, Is.EqualTo("Database error while retrieving games in collection."));
        }

        [Test]
        public void GetGamesInCollection_WithCollectionIdNotOne_GenericException_ThrowsRepositoryException()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetGamesInCollection", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("Test error"));
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.GetGamesInCollection(2, 1));
            Assert.That(ex.Message, Is.EqualTo("An unexpected error occurred while retrieving games in collection."));
        }

        #endregion

        #region AddGameToCollection Tests

        [Test]
        public void AddGameToCollection_CallsExecuteNonQuery()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteNonQuery("AddGameToCollection", It.IsAny<SqlParameter[]>()))
                         .Verifiable();
            // Act
            _repository.AddGameToCollection(1, 1, 1);
            // Assert: verify ExecuteNonQuery was called.
            _mockDataLink.Verify(dl => dl.ExecuteNonQuery("AddGameToCollection", It.IsAny<SqlParameter[]>()), Times.Once);
            Assert.That(true, Is.True); // dummy assert
        }

        [Test]
        public void AddGameToCollection_SqlException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            SqlException sqlEx = CreateSqlException();
            _mockDataLink.Setup(dl => dl.ExecuteNonQuery("AddGameToCollection", It.IsAny<SqlParameter[]>()))
                         .Throws(sqlEx);
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.AddGameToCollection(1, 1, 1));
            Assert.That(ex.Message, Is.EqualTo("Database error while adding game to collection."));
        }

        [Test]
        public void AddGameToCollection_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteNonQuery("AddGameToCollection", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("Test error"));
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.AddGameToCollection(1, 1, 1));
            Assert.That(ex.Message, Is.EqualTo("An unexpected error occurred while adding game to collection."));
        }

        #endregion

        #region RemoveGameFromCollection Tests

        [Test]
        public void RemoveGameFromCollection_CallsExecuteNonQuery()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteNonQuery("RemoveGameFromCollection", It.IsAny<SqlParameter[]>()))
                         .Verifiable();
            // Act
            _repository.RemoveGameFromCollection(1, 1);
            // Assert: verify ExecuteNonQuery was called.
            _mockDataLink.Verify(dl => dl.ExecuteNonQuery("RemoveGameFromCollection", It.IsAny<SqlParameter[]>()), Times.Once);
            Assert.That(true, Is.True); // dummy assert
        }

        [Test]
        public void RemoveGameFromCollection_SqlException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            SqlException sqlEx = CreateSqlException();
            _mockDataLink.Setup(dl => dl.ExecuteNonQuery("RemoveGameFromCollection", It.IsAny<SqlParameter[]>()))
                         .Throws(sqlEx);
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.RemoveGameFromCollection(1, 1));
            Assert.That(ex.Message, Is.EqualTo("Database error while removing game from collection."));
        }

        [Test]
        public void RemoveGameFromCollection_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteNonQuery("RemoveGameFromCollection", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("Test error"));
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.RemoveGameFromCollection(1, 1));
            Assert.That(ex.Message, Is.EqualTo("An unexpected error occurred while removing game from collection."));
        }

        #endregion

        #region MakeCollectionPrivateForUser Tests

        [Test]
        public void MakeCollectionPrivateForUser_CallsExecuteReader()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteReader("MakeCollectionPrivate", It.IsAny<SqlParameter[]>()))
                         .Verifiable();
            // Act
            _repository.MakeCollectionPrivateForUser("1", "1");
            // Assert: verify ExecuteReader was called.
            _mockDataLink.Verify(dl => dl.ExecuteReader("MakeCollectionPrivate", It.IsAny<SqlParameter[]>()), Times.Once);
            Assert.That(true, Is.True); // dummy assert
        }

        [Test]
        public void MakeCollectionPrivateForUser_DatabaseOperationException_ThrowsRepositoryException()
        {
            // Arrange
            var dbEx = new DatabaseOperationException("Error");
            _mockDataLink.Setup(dl => dl.ExecuteReader("MakeCollectionPrivate", It.IsAny<SqlParameter[]>()))
                         .Throws(dbEx);
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.MakeCollectionPrivateForUser("1", "1"));
            Assert.That(ex.Message, Is.EqualTo("Failed to make collection 1 private for user 1."));
        }

        #endregion

        #region MakeCollectionPublicForUser Tests

        [Test]
        public void MakeCollectionPublicForUser_CallsExecuteReader()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteReader("MakeCollectionPublic", It.IsAny<SqlParameter[]>()))
                         .Verifiable();
            // Act
            _repository.MakeCollectionPublicForUser("1", "1");
            // Assert: verify ExecuteReader was called.
            _mockDataLink.Verify(dl => dl.ExecuteReader("MakeCollectionPublic", It.IsAny<SqlParameter[]>()), Times.Once);
            Assert.That(true, Is.True); // dummy assert
        }

        [Test]
        public void MakeCollectionPublicForUser_DatabaseOperationException_ThrowsRepositoryException()
        {
            // Arrange
            var dbEx = new DatabaseOperationException("Error");
            _mockDataLink.Setup(dl => dl.ExecuteReader("MakeCollectionPublic", It.IsAny<SqlParameter[]>()))
                         .Throws(dbEx);
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.MakeCollectionPublicForUser("1", "1"));
            Assert.That(ex.Message, Is.EqualTo("Failed to make collection 1 public for user 1."));
        }

        #endregion

        #region RemoveCollectionForUser Tests

        [Test]
        public void RemoveCollectionForUser_CallsExecuteReader()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteReader("DeleteCollection", It.IsAny<SqlParameter[]>()))
                         .Verifiable();
            // Act
            _repository.RemoveCollectionForUser("1", "1");
            // Assert: verify ExecuteReader was called.
            _mockDataLink.Verify(dl => dl.ExecuteReader("DeleteCollection", It.IsAny<SqlParameter[]>()), Times.Once);
            Assert.That(true, Is.True); // dummy assert
        }

        [Test]
        public void RemoveCollectionForUser_DatabaseOperationException_ThrowsRepositoryException()
        {
            // Arrange
            var dbEx = new DatabaseOperationException("Error");
            _mockDataLink.Setup(dl => dl.ExecuteReader("DeleteCollection", It.IsAny<SqlParameter[]>()))
                         .Throws(dbEx);
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.RemoveCollectionForUser("1", "1"));
            Assert.That(ex.Message, Is.EqualTo("Failed to remove collection 1 for user 1."));
        }

        #endregion

        #region SaveCollection Tests

        [Test]
        public void SaveCollection_NewCollection_CallsCreateCollection()
        {
            // Arrange
            var collection = new Collection(1, "New Collection", DateOnly.FromDateTime(DateTime.Now), "cover.jpg", true);
            // Ensure CollectionId is 0 to simulate new collection.
            collection.CollectionId = 0;
            _mockDataLink.Setup(dl => dl.ExecuteReader("CreateCollection", It.IsAny<SqlParameter[]>()))
                         .Verifiable();
            // Act
            _repository.SaveCollection("1", collection);
            // Assert: verify ExecuteReader was called with "CreateCollection"
            _mockDataLink.Verify(dl => dl.ExecuteReader("CreateCollection", It.IsAny<SqlParameter[]>()), Times.Once);
            Assert.That(true, Is.True); // dummy assert
        }

        [Test]
        public void SaveCollection_ExistingCollection_CallsUpdateCollection()
        {
            // Arrange
            var collection = new Collection(1, "Existing Collection", DateOnly.FromDateTime(DateTime.Now), "cover.jpg", false);
            collection.CollectionId = 10; // non-zero
            _mockDataLink.Setup(dl => dl.ExecuteReader("UpdateCollection", It.IsAny<SqlParameter[]>()))
                         .Verifiable();
            // Act
            _repository.SaveCollection("1", collection);
            // Assert: verify ExecuteReader was called with "UpdateCollection"
            _mockDataLink.Verify(dl => dl.ExecuteReader("UpdateCollection", It.IsAny<SqlParameter[]>()), Times.Once);
            Assert.That(true, Is.True); // dummy assert
        }

        [Test]
        public void SaveCollection_DatabaseOperationException_ThrowsRepositoryException()
        {
            // Arrange - testing new collection case.
            var collection = new Collection(1, "New Collection", DateOnly.FromDateTime(DateTime.Now), "cover.jpg", true);
            collection.CollectionId = 0;
            var dbEx = new DatabaseOperationException("Error");
            _mockDataLink.Setup(dl => dl.ExecuteReader("CreateCollection", It.IsAny<SqlParameter[]>()))
                         .Throws(dbEx);
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.SaveCollection("1", collection));
            Assert.That(ex.Message, Is.EqualTo("Failed to save collection for user 1."));
        }

        #endregion

        #region DeleteCollection Tests

        [Test]
        public void DeleteCollection_CallsExecuteNonQuery()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteNonQuery("DeleteCollection", It.IsAny<SqlParameter[]>()))
                         .Verifiable();
            // Act
            _repository.DeleteCollection(1, 1);
            // Assert: verify ExecuteNonQuery was called.
            _mockDataLink.Verify(dl => dl.ExecuteNonQuery("DeleteCollection", It.IsAny<SqlParameter[]>()), Times.Once);
            Assert.That(true, Is.True); // dummy assert
        }

        [Test]
        public void DeleteCollection_SqlException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            SqlException sqlEx = CreateSqlException();
            _mockDataLink.Setup(dl => dl.ExecuteNonQuery("DeleteCollection", It.IsAny<SqlParameter[]>()))
                         .Throws(sqlEx);
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.DeleteCollection(1, 1));
            Assert.That(ex.Message, Is.EqualTo("Database error while deleting collection."));
        }

        [Test]
        public void DeleteCollection_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteNonQuery("DeleteCollection", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("Test error"));
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.DeleteCollection(1, 1));
            Assert.That(ex.Message, Is.EqualTo("An unexpected error occurred while deleting collection."));
        }

        #endregion

        #region CreateCollection Tests

        [Test]
        public void CreateCollection_CallsExecuteNonQuery()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteNonQuery("CreateCollection", It.IsAny<SqlParameter[]>()))
                         .Verifiable();
            // Act
            _repository.CreateCollection(1, "New Collection", "cover.jpg", true, DateOnly.FromDateTime(DateTime.Now));
            // Assert: verify ExecuteNonQuery was called.
            _mockDataLink.Verify(dl => dl.ExecuteNonQuery("CreateCollection", It.IsAny<SqlParameter[]>()), Times.Once);
            Assert.That(true, Is.True); // dummy assert
        }

        [Test]
        public void CreateCollection_SqlException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            SqlException sqlEx = CreateSqlException();
            _mockDataLink.Setup(dl => dl.ExecuteNonQuery("CreateCollection", It.IsAny<SqlParameter[]>()))
                         .Throws(sqlEx);
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.CreateCollection(1, "New Collection", "cover.jpg", true, DateOnly.FromDateTime(DateTime.Now)));
            Assert.That(ex.Message, Is.EqualTo("Database error while creating collection."));
        }

        [Test]
        public void CreateCollection_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteNonQuery("CreateCollection", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("Test error"));
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.CreateCollection(1, "New Collection", "cover.jpg", true, DateOnly.FromDateTime(DateTime.Now)));
            Assert.That(ex.Message, Is.EqualTo("An unexpected error occurred while creating collection."));
        }

        #endregion

        #region UpdateCollection Tests

        [Test]
        public void UpdateCollection_CallsExecuteReader()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteReader("UpdateCollection", It.IsAny<SqlParameter[]>()))
                         .Verifiable();
            // Act
            _repository.UpdateCollection(1, 1, "Updated Collection", "cover_updated.jpg", false);
            // Assert: verify ExecuteReader was called.
            _mockDataLink.Verify(dl => dl.ExecuteReader("UpdateCollection", It.IsAny<SqlParameter[]>()), Times.Once);
            Assert.That(true, Is.True); // dummy assert
        }

        [Test]
        public void UpdateCollection_SqlException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            SqlException sqlEx = CreateSqlException();
            _mockDataLink.Setup(dl => dl.ExecuteReader("UpdateCollection", It.IsAny<SqlParameter[]>()))
                         .Throws(sqlEx);
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.UpdateCollection(1, 1, "Updated Collection", "cover_updated.jpg", false));
            Assert.That(ex.Message, Is.EqualTo("Database error while updating collection."));
        }

        [Test]
        public void UpdateCollection_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteReader("UpdateCollection", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("Test error"));
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.UpdateCollection(1, 1, "Updated Collection", "cover_updated.jpg", false));
            Assert.That(ex.Message, Is.EqualTo("An unexpected error occurred while updating collection."));
        }

        #endregion

        #region GetPublicCollectionsForUser Tests

        [Test]
        public void GetPublicCollectionsForUser_ReturnsEmptyList_WhenDataTableHasNoRows()
        {
            // Arrange
            DataTable dt = CreateEmptyCollectionsDataTable();
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetPublicCollectionsForUser", It.IsAny<SqlParameter[]>()))
                         .Returns(dt);
            // Act
            var result = _repository.GetPublicCollectionsForUser(1);
            // Assert: list is empty.
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void GetPublicCollectionsForUser_ReturnsCorrectCount_WhenDataTableHasRows()
        {
            // Arrange
            int expectedCount = 4;
            DataTable dt = CreateCollectionsDataTable(expectedCount);
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetPublicCollectionsForUser", It.IsAny<SqlParameter[]>()))
                         .Returns(dt);
            // Act
            var result = _repository.GetPublicCollectionsForUser(1);
            // Assert: count equals expected.
            Assert.That(result.Count, Is.EqualTo(expectedCount));
        }

        [Test]
        public void GetPublicCollectionsForUser_SqlException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            SqlException sqlEx = CreateSqlException();
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetPublicCollectionsForUser", It.IsAny<SqlParameter[]>()))
                         .Throws(sqlEx);
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.GetPublicCollectionsForUser(1));
            Assert.That(ex.Message, Is.EqualTo("Database error while retrieving public collections."));
        }

        [Test]
        public void GetPublicCollectionsForUser_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetPublicCollectionsForUser", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("Test error"));
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.GetPublicCollectionsForUser(1));
            Assert.That(ex.Message, Is.EqualTo("An unexpected error occurred while retrieving public collections."));
        }

        #endregion

        #region GetGamesNotInCollection Tests

        [Test]
        public void GetGamesNotInCollection_ReturnsEmptyList_WhenDataTableHasNoRows()
        {
            // Arrange: Create an empty DataTable with the expected columns.
            DataTable emptyTable = new DataTable();
            emptyTable.Columns.Add("user_id", typeof(int));
            emptyTable.Columns.Add("title", typeof(string));
            emptyTable.Columns.Add("description", typeof(string));
            emptyTable.Columns.Add("cover_picture", typeof(string));
            emptyTable.Columns.Add("game_id", typeof(int));
    
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetGamesNotInCollection", It.IsAny<SqlParameter[]>()))
                .Returns(emptyTable);
    
            // Act
            var result = _repository.GetGamesNotInCollection(1, 1);
    
            // Assert: The returned list should be empty.
            Assert.That(result, Is.Empty);
        }


        [Test]
        public void GetGamesNotInCollection_ReturnsCorrectCount_WhenDataTableHasRows()
        {
            // Arrange
            int expectedCount = 3;
            DataTable dt = CreateGamesDataTable(expectedCount);
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetGamesNotInCollection", It.IsAny<SqlParameter[]>()))
                         .Returns(dt);
            // Act
            var result = _repository.GetGamesNotInCollection(1, 1);
            // Assert: count equals expected.
            Assert.That(result.Count, Is.EqualTo(expectedCount));
        }

        [Test]
        public void GetGamesNotInCollection_SqlException_ThrowsRepositoryExceptionWithDatabaseErrorMessage()
        {
            // Arrange
            SqlException sqlEx = CreateSqlException();
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetGamesNotInCollection", It.IsAny<SqlParameter[]>()))
                         .Throws(sqlEx);
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.GetGamesNotInCollection(1, 1));
            Assert.That(ex.Message, Is.EqualTo("Database error while getting games not in collection."));
        }

        [Test]
        public void GetGamesNotInCollection_GenericException_ThrowsRepositoryExceptionWithUnexpectedErrorMessage()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetGamesNotInCollection", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("Test error"));
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.GetGamesNotInCollection(1, 1));
            Assert.That(ex.Message, Is.EqualTo("An unexpected error occurred while getting games not in collection."));
        }

        #endregion
    }
}
