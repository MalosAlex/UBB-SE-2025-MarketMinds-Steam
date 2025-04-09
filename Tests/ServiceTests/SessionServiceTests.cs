using Moq;
using BusinessLayer.Models;
using BusinessLayer.Services;
using BusinessLayer.Repositories.Interfaces;

namespace Tests.ServiceTests
{
    [TestFixture]
    internal class SessionServiceTests
    {
        private Mock<ISessionRepository> _mockSessionRepository;
        private Mock<IUsersRepository> _mockUsersRepository;
        private SessionService _sessionService;

        [SetUp]
        public void SetUp()
        {
            _mockSessionRepository = new Mock<ISessionRepository>(MockBehavior.Strict);
            _mockUsersRepository = new Mock<IUsersRepository>(MockBehavior.Strict);
            _sessionService = new SessionService(_mockSessionRepository.Object, _mockUsersRepository.Object);

            // Clear the singleton session between tests
            UserSession.Instance.ClearSession();
        }

        [Test]
        public void Constructor_NullSessionRepository_ThrowsArgumentNullException()
        {
            // Arrange
            var usersRepo = new Mock<IUsersRepository>().Object;

            // Act
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new SessionService(null, usersRepo));

            // Assert
            Assert.That(exception.ParamName, Is.EqualTo("sessionRepository"));
        }

        [Test]
        public void Constructor_NullUsersRepository_ThrowsArgumentNullException()
        {
            // Arrange
            var sessionRepo = new Mock<ISessionRepository>().Object;

            // Act
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new SessionService(sessionRepo, null));

            // Assert
            Assert.That(exception.ParamName, Is.EqualTo("usersRepository"));
        }

        [Test]
        public void CreateNewSession_ShouldReturnNewSessionId()
        {
            // Arrange
            var user = new User { UserId = 1 };
            var sessionDetails = new SessionDetails
            {
                SessionId = Guid.NewGuid(),
                UserId = user.UserId,
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddHours(1)
            };

            _mockSessionRepository.Setup(repo => repo.DeleteUserSessions(user.UserId));
            _mockSessionRepository.Setup(repo => repo.CreateSession(user.UserId)).Returns(sessionDetails);

            // Act
            var result = _sessionService.CreateNewSession(user);

            // Assert
            Assert.That(result, Is.EqualTo(sessionDetails.SessionId));
            _mockSessionRepository.Verify(repo => repo.DeleteUserSessions(user.UserId), Times.Once);
            _mockSessionRepository.Verify(repo => repo.CreateSession(user.UserId), Times.Once);
        }

        [Test]
        public void EndSession_ShouldClearCurrentSession()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            UserSession.Instance.UpdateSession(sessionId, 1, DateTime.Now, DateTime.Now.AddHours(1));
            _mockSessionRepository.Setup(repo => repo.DeleteSession(sessionId));

            // Act
            _sessionService.EndSession();

            // Assert
            Assert.That(UserSession.Instance.CurrentSessionId, Is.Null);
            _mockSessionRepository.Verify(repo => repo.DeleteSession(sessionId), Times.Once);
        }

        [Test]
        public void GetCurrentUser_ShouldReturnUserIfSessionIsValid()
        {
            // Arrange
            var userId = 1;
            var user = new User { UserId = userId };
            UserSession.Instance.UpdateSession(Guid.NewGuid(), userId, DateTime.Now, DateTime.Now.AddHours(1));
            _mockUsersRepository.Setup(repo => repo.GetUserById(userId)).Returns(user);

            // Act
            var result = _sessionService.GetCurrentUser();

            // Assert
            Assert.That(result, Is.EqualTo(user));
            _mockUsersRepository.Verify(repo => repo.GetUserById(userId), Times.Once);
        }

        [Test]
        public void GetCurrentUser_WithExpiredSession_ShouldClearSessionAndReturnNull()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            // Set an expired session by setting ExpiresAt in the past
            UserSession.Instance.UpdateSession(sessionId, 1, DateTime.Now.AddHours(-2), DateTime.Now.AddHours(-1));
            _mockSessionRepository.Setup(repo => repo.DeleteSession(sessionId));

            // Act
            var result = _sessionService.GetCurrentUser();

            // Assert
            Assert.That(result, Is.Null);
            Assert.That(UserSession.Instance.CurrentSessionId, Is.Null);
            _mockSessionRepository.Verify(repo => repo.DeleteSession(sessionId), Times.Once);
        }

        [Test]
        public void IsUserLoggedIn_ShouldReturnTrueIfSessionIsValid()
        {
            // Arrange
            UserSession.Instance.UpdateSession(Guid.NewGuid(), 1, DateTime.Now, DateTime.Now.AddHours(1));

            // Act
            var result = _sessionService.IsUserLoggedIn();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void RestoreSessionFromDatabase_ShouldUpdateSessionIfValid()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var sessionDetails = new SessionDetails
            {
                SessionId = sessionId,
                UserId = 1,
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddHours(1)
            };

            _mockSessionRepository.Setup(repo => repo.GetSessionById(sessionId)).Returns(sessionDetails);

            // Act
            _sessionService.RestoreSessionFromDatabase(sessionId);

            // Assert
            Assert.That(UserSession.Instance.CurrentSessionId, Is.EqualTo(sessionId));
            _mockSessionRepository.Verify(repo => repo.GetSessionById(sessionId), Times.Once);
        }

        [Test]
        public void RestoreSessionFromDatabase_WithExpiredSession_ShouldDeleteSession()
        {
            // Arrange
            var sessionId = Guid.NewGuid();

            // Create sessionDetails with ExpiresAt in the past
            var sessionDetails = new SessionDetails
            {
                SessionId = sessionId,
                UserId = 1,
                CreatedAt = DateTime.Now.AddHours(-2),
                ExpiresAt = DateTime.Now.AddHours(-1)
            };

            _mockSessionRepository.Setup(repo => repo.GetSessionById(sessionId)).Returns(sessionDetails);
            _mockSessionRepository.Setup(repo => repo.DeleteSession(sessionId));

            // Act
            _sessionService.RestoreSessionFromDatabase(sessionId);

            // Assert
            Assert.That(UserSession.Instance.CurrentSessionId, Is.Null);
            _mockSessionRepository.Verify(repo => repo.GetSessionById(sessionId), Times.Once);
            _mockSessionRepository.Verify(repo => repo.DeleteSession(sessionId), Times.Once);
        }

        [Test]
        public void CleanupExpiredSessions_ShouldDeleteExpiredSessions()
        {
            // Arrange
            var expiredSessionIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
            _mockSessionRepository.Setup(repo => repo.GetExpiredSessions()).Returns(expiredSessionIds);
            foreach (var sessionId in expiredSessionIds)
            {
                _mockSessionRepository.Setup(repo => repo.DeleteSession(sessionId));
            }

            // Act
            _sessionService.CleanupExpiredSessions();

            // Assert
            foreach (var sessionId in expiredSessionIds)
            {
                _mockSessionRepository.Verify(repo => repo.DeleteSession(sessionId), Times.Once);
            }
        }

        [Test]
        public void CleanupExpiredSessions_WithInvalidCurrentSession_ShouldClearSession()
        {
            // Arrange
            // Create some expired sessions
            var expiredSessionIds = new List<Guid> { Guid.NewGuid() };
            _mockSessionRepository.Setup(repo => repo.GetExpiredSessions()).Returns(expiredSessionIds);
            foreach (var sessionId in expiredSessionIds)
            {
                _mockSessionRepository.Setup(repo => repo.DeleteSession(sessionId));
            }

            // Setup a current session that is expired (thus invalid)
            var currentSessionId = Guid.NewGuid();
            UserSession.Instance.UpdateSession(currentSessionId, 1, DateTime.Now.AddHours(-2), DateTime.Now.AddHours(-1));

            // Act
            _sessionService.CleanupExpiredSessions();

            // Assert
            Assert.That(UserSession.Instance.CurrentSessionId, Is.Null);
        }
    }
}
