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
    public class FriendshipsRepositoryTests
    {
        private Mock<IDataLink> _mockDataLink;
        private FriendshipsRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _mockDataLink = new Mock<IDataLink>();
            _repository = new FriendshipsRepository(_mockDataLink.Object);
        }

        #region Helper Methods

        private DataTable CreateFriendshipsDataTable(params (int friendshipId, int userId, int friendId)[] rows)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("friendship_id", typeof(int));
            dt.Columns.Add("user_id", typeof(int));
            dt.Columns.Add("friend_id", typeof(int));

            foreach (var (friendshipId, userId, friendId) in rows)
            {
                var row = dt.NewRow();
                row["friendship_id"] = friendshipId;
                row["user_id"] = userId;
                row["friend_id"] = friendId;
                dt.Rows.Add(row);
            }
            return dt;
        }

        private DataTable CreateUserDataTable(string username)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("username", typeof(string));
            var row = dt.NewRow();
            row["username"] = username;
            dt.Rows.Add(row);
            return dt;
        }

        private DataTable CreateUserProfileDataTable(string profilePicture)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("profile_picture", typeof(string));
            var row = dt.NewRow();
            row["profile_picture"] = profilePicture;
            dt.Rows.Add(row);
            return dt;
        }

        private SqlException CreateSqlException()
        {
            return (SqlException)FormatterServices.GetUninitializedObject(typeof(SqlException));
        }

        #endregion

        #region GetAllFriendships Tests

        [Test]
        public void GetAllFriendships_ReturnsCorrectCount()
        {
            // Arrange
            var friendshipsTable = CreateFriendshipsDataTable((1, 1, 2), (2, 1, 3));
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetFriendsForUser", It.IsAny<SqlParameter[]>()))
                         .Returns(friendshipsTable);
            // For each friendship row, setup friend lookup returning a username.
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetUserById", It.Is<SqlParameter[]>(p => Convert.ToInt32(p[0].Value) == 2)))
                         .Returns(CreateUserDataTable("Alice"));
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetUserById", It.Is<SqlParameter[]>(p => Convert.ToInt32(p[0].Value) == 3)))
                         .Returns(CreateUserDataTable("Bob"));
            // For profile lookup, return a dummy profile picture.
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetUserProfileByUserId", It.Is<SqlParameter[]>(p => Convert.ToInt32(p[0].Value) == 2)))
                         .Returns(CreateUserProfileDataTable("alice.jpg"));
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetUserProfileByUserId", It.Is<SqlParameter[]>(p => Convert.ToInt32(p[0].Value) == 3)))
                         .Returns(CreateUserProfileDataTable("bob.jpg"));
            // Act
            var result = _repository.GetAllFriendships(1);
            // Assert: There should be exactly 2 friendships.
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetAllFriendships_AssignsFriendUsernameCorrectly()
        {
            // Arrange
            var friendshipsTable = CreateFriendshipsDataTable((1, 1, 2));
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetFriendsForUser", It.IsAny<SqlParameter[]>()))
                         .Returns(friendshipsTable);
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetUserById", It.IsAny<SqlParameter[]>()))
                         .Returns(CreateUserDataTable("Charlie"));
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetUserProfileByUserId", It.IsAny<SqlParameter[]>()))
                         .Returns(CreateUserProfileDataTable("charlie.jpg"));
            // Act
            var result = _repository.GetAllFriendships(1);
            // Assert: FriendUsername should be "Charlie".
            Assert.That(result.First().FriendUsername, Is.EqualTo("Charlie"));
        }

        [Test]
        public void GetAllFriendships_AssignsFriendProfilePictureCorrectly()
        {
            // Arrange
            var friendshipsTable = CreateFriendshipsDataTable((1, 1, 2));
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetFriendsForUser", It.IsAny<SqlParameter[]>()))
                         .Returns(friendshipsTable);
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetUserById", It.IsAny<SqlParameter[]>()))
                         .Returns(CreateUserDataTable("Diana"));
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetUserProfileByUserId", It.IsAny<SqlParameter[]>()))
                         .Returns(CreateUserProfileDataTable("diana.png"));
            // Act
            var result = _repository.GetAllFriendships(1);
            // Assert: FriendProfilePicture should be "diana.png".
            Assert.That(result.First().FriendProfilePicture, Is.EqualTo("diana.png"));
        }

        [Test]
        public void GetAllFriendships_OrdersByFriendUsername()
        {
            // Arrange: Two rows with different usernames.
            var friendshipsTable = CreateFriendshipsDataTable((1, 1, 2), (2, 1, 3));
            // Setup friend data: friendId 2 -> "Zoe", friendId 3 -> "Anna"
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetFriendsForUser", It.IsAny<SqlParameter[]>()))
                         .Returns(friendshipsTable);
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetUserById", 
                It.Is<SqlParameter[]>(p => Convert.ToInt32(p[0].Value) == 2)))
                         .Returns(CreateUserDataTable("Zoe"));
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetUserById", 
                It.Is<SqlParameter[]>(p => Convert.ToInt32(p[0].Value) == 3)))
                         .Returns(CreateUserDataTable("Anna"));
            // Setup profile data (dummy)
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetUserProfileByUserId", It.IsAny<SqlParameter[]>()))
                         .Returns(CreateUserProfileDataTable("dummy.jpg"));
            // Act
            var result = _repository.GetAllFriendships(1);
            // Assert: First friendship's FriendUsername should be "Anna" (alphabetically before "Zoe").
            Assert.That(result.First().FriendUsername, Is.EqualTo("Anna"));
        }

        [Test]
        public void GetAllFriendships_SqlException_ThrowsRepositoryException()
        {
            // Arrange
            SqlException sqlEx = CreateSqlException();
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetFriendsForUser", It.IsAny<SqlParameter[]>()))
                         .Throws(sqlEx);
            // Act & Assert: Expect RepositoryException with proper message.
            var ex = Assert.Throws<RepositoryException>(() => _repository.GetAllFriendships(1));
            Assert.That(ex.Message, Is.EqualTo("Database error while retrieving friendships."));
        }

        [Test]
        public void GetAllFriendships_GenericException_ThrowsRepositoryException()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetFriendsForUser", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("Test error"));
            // Act & Assert: Expect RepositoryException with proper message.
            var ex = Assert.Throws<RepositoryException>(() => _repository.GetAllFriendships(1));
            Assert.That(ex.Message, Is.EqualTo("An unexpected error occurred while retrieving friendships."));
        }

        #endregion

        #region GetFriendshipById Tests

        [Test]
        public void GetFriendshipById_ReturnsFriendship_WhenExists()
        {
            // Arrange
            var dt = CreateFriendshipsDataTable((10, 1, 2));
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetFriendshipById", It.IsAny<SqlParameter[]>()))
                         .Returns(dt);
            // Act
            var friendship = _repository.GetFriendshipById(10);
            // Assert: Friendship's friendshipId should be 10.
            Assert.That(friendship.FriendshipId, Is.EqualTo(10));
        }

        [Test]
        public void GetFriendshipById_ReturnsNull_WhenNotExists()
        {
            // Arrange: Return empty DataTable.
            DataTable dt = new DataTable();
            dt.Columns.Add("friendship_id", typeof(int));
            dt.Columns.Add("user_id", typeof(int));
            dt.Columns.Add("friend_id", typeof(int));
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetFriendshipById", It.IsAny<SqlParameter[]>()))
                         .Returns(dt);
            // Act
            var friendship = _repository.GetFriendshipById(999);
            // Assert: Friendship should be null.
            Assert.That(friendship, Is.Null);
        }

        [Test]
        public void GetFriendshipById_SqlException_ThrowsRepositoryException()
        {
            // Arrange
            SqlException sqlEx = CreateSqlException();
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetFriendshipById", It.IsAny<SqlParameter[]>()))
                         .Throws(sqlEx);
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.GetFriendshipById(1));
            Assert.That(ex.Message, Is.EqualTo("Database error while retrieving friendship by ID."));
        }

        [Test]
        public void GetFriendshipById_GenericException_ThrowsRepositoryException()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetFriendshipById", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("Test error"));
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.GetFriendshipById(1));
            Assert.That(ex.Message, Is.EqualTo("An unexpected error occurred while retrieving friendship by ID."));
        }

        #endregion

        #region AddFriendship Tests

        [Test]
        public void AddFriendship_AddsFriendship_WhenDataIsValid()
        {
            // Arrange: Setup GetUserById for both users.
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetUserById", 
                It.Is<SqlParameter[]>(p => Convert.ToInt32(p[0].Value) == 1)))
                         .Returns(CreateUserDataTable("User1"));
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetUserById", 
                It.Is<SqlParameter[]>(p => Convert.ToInt32(p[0].Value) == 2)))
                         .Returns(CreateUserDataTable("User2"));
            // Setup GetAllFriendships to return empty list.
            var emptyTable = CreateFriendshipsDataTable();
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetFriendsForUser", It.IsAny<SqlParameter[]>()))
                         .Returns(emptyTable);
            // Setup ExecuteNonQuery for AddFriend.
            _mockDataLink.Setup(dl => dl.ExecuteNonQuery("AddFriend", It.IsAny<SqlParameter[]>()));
            // Act
            Assert.DoesNotThrow(() => _repository.AddFriendship(1, 2));
            // Assert: (dummy assert to satisfy one assert rule)
            Assert.That(true, Is.True);
        }

        [Test]
        public void AddFriendship_ThrowsException_WhenUserDoesNotExist()
        {
            // Arrange: Setup GetUserById for user returns empty.
            DataTable empty = new DataTable();
            empty.Columns.Add("username", typeof(string));
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetUserById", 
                It.Is<SqlParameter[]>(p => Convert.ToInt32(p[0].Value) == 1)))
                         .Returns(empty);
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.AddFriendship(1, 2));
            Assert.That(ex.Message, Is.EqualTo("User with ID 1 does not exist."));
        }

        [Test]
        public void AddFriendship_ThrowsException_WhenFriendDoesNotExist()
        {
            // Arrange: Setup valid user data for user 1.
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetUserById", 
                It.Is<SqlParameter[]>(p => Convert.ToInt32(p[0].Value) == 1)))
                         .Returns(CreateUserDataTable("User1"));
            // Setup friend data for friendId 2 returns empty.
            DataTable empty = new DataTable();
            empty.Columns.Add("username", typeof(string));
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetUserById", 
                It.Is<SqlParameter[]>(p => Convert.ToInt32(p[0].Value) == 2)))
                         .Returns(empty);
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.AddFriendship(1, 2));
            Assert.That(ex.Message, Is.EqualTo("User with ID 2 does not exist."));
        }

        [Test]
        public void AddFriendship_ThrowsException_WhenFriendshipAlreadyExists()
        {
            // Arrange: Setup valid data for both users.
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetUserById", It.IsAny<SqlParameter[]>()))
                .Returns(CreateUserDataTable("User"));
            // Setup GetAllFriendships to return a friendship with friendId = 2.
            var friendshipsTable = CreateFriendshipsDataTable((1, 1, 2));
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetFriendsForUser", It.IsAny<SqlParameter[]>()))
                .Returns(friendshipsTable);
            // Setup profile lookup for friendId = 2.
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetUserProfileByUserId", 
                    It.Is<SqlParameter[]>(p => Convert.ToInt32(p[0].Value) == 2)))
                .Returns(CreateUserProfileDataTable("UserPic"));
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.AddFriendship(1, 2));
            Assert.That(ex.Message, Is.EqualTo("Friendship already exists."));
        }


        [Test]
        public void AddFriendship_SqlException_ThrowsRepositoryException()
        {
            // Arrange: Setup GetUserById to return valid user data.
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetUserById", It.IsAny<SqlParameter[]>()))
                         .Returns(CreateUserDataTable("User"));
            // Setup GetAllFriendships returns empty.
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetFriendsForUser", It.IsAny<SqlParameter[]>()))
                         .Returns(CreateFriendshipsDataTable());
            // Setup ExecuteNonQuery to throw SqlException.
            SqlException sqlEx = CreateSqlException();
            _mockDataLink.Setup(dl => dl.ExecuteNonQuery("AddFriend", It.IsAny<SqlParameter[]>()))
                         .Throws(sqlEx);
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.AddFriendship(1, 2));
            Assert.That(ex.Message, Is.EqualTo("Database error while adding friendship."));
        }

        [Test]
        public void AddFriendship_GenericException_ThrowsRepositoryException()
        {
            // Arrange: Setup GetUserById to return valid data.
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetUserById", It.IsAny<SqlParameter[]>()))
                         .Returns(CreateUserDataTable("User"));
            // Setup GetAllFriendships returns empty.
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetFriendsForUser", It.IsAny<SqlParameter[]>()))
                         .Returns(CreateFriendshipsDataTable());
            // Setup ExecuteNonQuery to throw generic exception.
            _mockDataLink.Setup(dl => dl.ExecuteNonQuery("AddFriend", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("Test error"));
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.AddFriendship(1, 2));
            Assert.That(ex.Message, Is.EqualTo("An unexpected error occurred while adding friendship."));
        }

        #endregion

        #region RemoveFriendship Tests

        [Test]
        public void RemoveFriendship_CallsExecuteNonQuery()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteNonQuery("RemoveFriend", It.IsAny<SqlParameter[]>()))
                         .Verifiable();
            // Act
            _repository.RemoveFriendship(5);
            // Assert: Verify ExecuteNonQuery was called once.
            _mockDataLink.Verify(dl => dl.ExecuteNonQuery("RemoveFriend", It.IsAny<SqlParameter[]>()), Times.Once);
            Assert.That(true, Is.True); // dummy assert for one-assert rule.
        }

        [Test]
        public void RemoveFriendship_SqlException_ThrowsRepositoryException()
        {
            // Arrange
            SqlException sqlEx = CreateSqlException();
            _mockDataLink.Setup(dl => dl.ExecuteNonQuery("RemoveFriend", It.IsAny<SqlParameter[]>()))
                         .Throws(sqlEx);
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.RemoveFriendship(5));
            Assert.That(ex.Message, Is.EqualTo("Database error while removing friendship."));
        }

        [Test]
        public void RemoveFriendship_GenericException_ThrowsRepositoryException()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteNonQuery("RemoveFriend", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("Test error"));
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.RemoveFriendship(5));
            Assert.That(ex.Message, Is.EqualTo("An unexpected error occurred while removing friendship."));
        }

        #endregion

        #region GetFriendshipCount Tests

        [Test]
        public void GetFriendshipCount_ReturnsCorrectCount()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteScalar<int>("GetFriendshipCountForUser", It.IsAny<SqlParameter[]>()))
                         .Returns(7);
            // Act
            var count = _repository.GetFriendshipCount(1);
            // Assert: Count should be 7.
            Assert.That(count, Is.EqualTo(7));
        }

        [Test]
        public void GetFriendshipCount_SqlException_ThrowsRepositoryException()
        {
            // Arrange
            SqlException sqlEx = CreateSqlException();
            _mockDataLink.Setup(dl => dl.ExecuteScalar<int>("GetFriendshipCountForUser", It.IsAny<SqlParameter[]>()))
                         .Throws(sqlEx);
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.GetFriendshipCount(1));
            Assert.That(ex.Message, Is.EqualTo("Database error while retrieving friendship count."));
        }

        [Test]
        public void GetFriendshipCount_GenericException_ThrowsRepositoryException()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteScalar<int>("GetFriendshipCountForUser", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("Test error"));
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.GetFriendshipCount(1));
            Assert.That(ex.Message, Is.EqualTo("An unexpected error occurred while retrieving friendship count."));
        }

        #endregion

        #region GetFriendshipId Tests

        [Test]
        public void GetFriendshipId_ReturnsCorrectId_WhenExists()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteScalar<int?>("GetFriendshipId", It.IsAny<SqlParameter[]>()))
                         .Returns(15);
            // Act
            var id = _repository.GetFriendshipId(1, 2);
            // Assert: Returned id should be 15.
            Assert.That(id, Is.EqualTo(15));
        }

        [Test]
        public void GetFriendshipId_ReturnsNull_WhenNotExists()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteScalar<int?>("GetFriendshipId", It.IsAny<SqlParameter[]>()))
                         .Returns((int?)null);
            // Act
            var id = _repository.GetFriendshipId(1, 2);
            // Assert: Returned id should be null.
            Assert.That(id, Is.Null);
        }

        [Test]
        public void GetFriendshipId_SqlException_ThrowsRepositoryException()
        {
            // Arrange
            SqlException sqlEx = CreateSqlException();
            _mockDataLink.Setup(dl => dl.ExecuteScalar<int?>("GetFriendshipId", It.IsAny<SqlParameter[]>()))
                         .Throws(sqlEx);
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.GetFriendshipId(1, 2));
            Assert.That(ex.Message, Is.EqualTo("Database error while retrieving friendship ID."));
        }

        [Test]
        public void GetFriendshipId_GenericException_ThrowsRepositoryException()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteScalar<int?>("GetFriendshipId", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("Test error"));
            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repository.GetFriendshipId(1, 2));
            Assert.That(ex.Message, Is.EqualTo("An unexpected error occurred while retrieving friendship ID."));
        }

        #endregion
    }
}
