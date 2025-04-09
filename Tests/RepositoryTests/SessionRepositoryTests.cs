using BusinessLayer.Data;
using BusinessLayer.Repositories;
using Moq;
using System.Data;
using Microsoft.Data.SqlClient;
using BusinessLayer.Exceptions;

namespace Tests.RepositoryTests
{
    [TestFixture]
    internal class SessionRepositoryTests
    {
        private Mock<IDataLink> _mockDataLink;
        private SessionRepository _sessionRepository;

        [SetUp]
        public void Setup()
        {
            _mockDataLink = new Mock<IDataLink>();
            _sessionRepository = new SessionRepository(_mockDataLink.Object);
        }

        [Test]
        public void Constructor_NullDataLink_ThrowsArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SessionRepository(null));
        }

        [Test]
        public void CreateSession_ValidUserId_ReturnsSessionDetails()
        {
            // Arrange
            var userId = 1;
            var sessionId = Guid.NewGuid();
            var createdAt = DateTime.Now;
            var expiresAt = createdAt.AddHours(1);

            var dataTable = new DataTable();
            dataTable.Columns.Add("session_id", typeof(Guid));
            dataTable.Columns.Add("user_id", typeof(int));
            dataTable.Columns.Add("created_at", typeof(DateTime));
            dataTable.Columns.Add("expires_at", typeof(DateTime));
            dataTable.Rows.Add(sessionId, userId, createdAt, expiresAt);

            _mockDataLink
                .Setup(dataLink => dataLink.ExecuteReader("CreateSession", It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act
            var result = _sessionRepository.CreateSession(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void CreateSession_NoRows_ThrowsInvalidOperationException()
        {
            // Arrange
            var userId = 1;
            var dataTable = new DataTable();

            _mockDataLink
                .Setup(dataLink => dataLink.ExecuteReader("CreateSession", It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _sessionRepository.CreateSession(userId));
        }

        [Test]
        public void DeleteUserSessions_ValidUserId_ExecutesNonQuery()
        {
            // Arrange
            var userId = 1;

            // Act
            _sessionRepository.DeleteUserSessions(userId);

            // Assert
            _mockDataLink.Verify(dataLink => dataLink.ExecuteNonQuery("DeleteUserSessions", It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Test]
        public void DeleteSession_ValidSessionId_ExecutesNonQuery()
        {
            // Arrange
            var sessionId = Guid.NewGuid();

            // Act
            _sessionRepository.DeleteSession(sessionId);

            // Assert
            _mockDataLink.Verify(dataLink => dataLink.ExecuteNonQuery("DeleteSession", It.IsAny<SqlParameter[]>()), Times.Once);
        }

        [Test]
        public void GetSessionById_ValidSessionId_ReturnsSessionDetails()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var userId = 1;
            var createdAt = DateTime.Now;
            var expiresAt = createdAt.AddHours(1);

            var dataTable = new DataTable();
            dataTable.Columns.Add("session_id", typeof(Guid));
            dataTable.Columns.Add("user_id", typeof(int));
            dataTable.Columns.Add("created_at", typeof(DateTime));
            dataTable.Columns.Add("expires_at", typeof(DateTime));
            dataTable.Rows.Add(sessionId, userId, createdAt, expiresAt);

            _mockDataLink
                .Setup(dataLink => dataLink.ExecuteReader("GetSessionById", It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act
            var result = _sessionRepository.GetSessionById(sessionId);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetSessionById_NoRows_ReturnsNull()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var dataTable = new DataTable();

            _mockDataLink
                .Setup(dataLink => dataLink.ExecuteReader("GetSessionById", It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act
            var result = _sessionRepository.GetSessionById(sessionId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetUserFromSession_ValidSessionId_ReturnsUserWithSessionDetails()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var userId = 1;
            var username = "testuser";
            var email = "test@example.com";
            var createdAt = DateTime.Now;
            var expiresAt = createdAt.AddHours(1);
            var lastLogin = DateTime.Now.AddDays(-1);

            var dataTable = new DataTable();
            dataTable.Columns.Add("session_id", typeof(Guid));
            dataTable.Columns.Add("user_id", typeof(int));
            dataTable.Columns.Add("username", typeof(string));
            dataTable.Columns.Add("email", typeof(string));
            dataTable.Columns.Add("developer", typeof(bool));
            dataTable.Columns.Add("created_at", typeof(DateTime));
            dataTable.Columns.Add("expires_at", typeof(DateTime));
            dataTable.Columns.Add("last_login", typeof(DateTime));
            dataTable.Rows.Add(sessionId, userId, username, email, true, createdAt, expiresAt, lastLogin);

            _mockDataLink
                .Setup(dataLink => dataLink.ExecuteReader("GetUserFromSession", It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act
            var result = _sessionRepository.GetUserFromSession(sessionId);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetUserFromSession_LastLoginIsDbNull_SetsLastLoginToNull()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var userId = 1;
            var username = "testuser";
            var email = "test@example.com";
            var createdAt = DateTime.Now;
            var expiresAt = createdAt.AddHours(1);

            var dataTable = new DataTable();
            dataTable.Columns.Add("session_id", typeof(Guid));
            dataTable.Columns.Add("user_id", typeof(int));
            dataTable.Columns.Add("username", typeof(string));
            dataTable.Columns.Add("email", typeof(string));
            dataTable.Columns.Add("developer", typeof(bool));
            dataTable.Columns.Add("created_at", typeof(DateTime));
            dataTable.Columns.Add("expires_at", typeof(DateTime));
            dataTable.Columns.Add("last_login", typeof(DateTime));
            dataTable.Rows.Add(sessionId, userId, username, email, true, createdAt, expiresAt, DBNull.Value);

            _mockDataLink
                .Setup(dataLink => dataLink.ExecuteReader("GetUserFromSession", It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act
            var result = _sessionRepository.GetUserFromSession(sessionId);

            // Assert
            Assert.That(result.LastLogin, Is.Null);
        }

        [Test]
        public void GetUserFromSession_NoRows_ReturnsNull()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var dataTable = new DataTable();

            _mockDataLink
                .Setup(dataLink => dataLink.ExecuteReader("GetUserFromSession", It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act
            var result = _sessionRepository.GetUserFromSession(sessionId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetUserFromSession_DatabaseOperationException_ThrowsDatabaseException()
        {
            // Arrange
            var sessionId = Guid.NewGuid();

            _mockDataLink
                .Setup(dataLink => dataLink.ExecuteReader("GetUserFromSession", It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            Assert.Throws<DatabaseOperationException>(() => _sessionRepository.GetUserFromSession(sessionId));
        }

        [Test]
        public void GetExpiredSessions_ReturnsListOfSessionIds()
        {
            // Arrange
            var sessionId1 = Guid.NewGuid();
            var sessionId2 = Guid.NewGuid();

            var dataTable = new DataTable();
            dataTable.Columns.Add("session_id", typeof(Guid));
            dataTable.Rows.Add(sessionId1);
            dataTable.Rows.Add(sessionId2);

            _mockDataLink
                .Setup(dataLink => dataLink.ExecuteReader("GetExpiredSessions", null))
                .Returns(dataTable);

            // Act
            var result = _sessionRepository.GetExpiredSessions();

            // Assert
            Assert.That(result, Is.Not.Empty);
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetExpiredSessions_NoRows_ReturnsEmptyList()
        {
            // Arrange
            var dataTable = new DataTable();

            _mockDataLink
                .Setup(dataLink => dataLink.ExecuteReader("GetExpiredSessions", null))
                .Returns(dataTable);

            // Act
            var result = _sessionRepository.GetExpiredSessions();

            // Assert
            Assert.That(result, Is.Empty);
        }
    }
}
