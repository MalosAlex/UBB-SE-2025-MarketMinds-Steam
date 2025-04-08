using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Services;
using BusinessLayer.Services.Fakes;
using BusinessLayer.Repositories.Fakes;
using BusinessLayer.Models;
using BusinessLayer.Exceptions;
using BusinessLayer.Utils;


namespace Tests.ServiceTests
{
    [TestFixture]
    internal class UserServiceTests
    {
        private UserService _service;

        [SetUp]
        public void Setup()
        {
            _service = new UserService(new FakeUsersRepository(), new FakeSessionService());
        }

        [Test]
        public void GetAllUsers_Called_ReturnsCorrectCount()
        {
            // Arrange
            // nothing

            // Act
            List<User> users = _service.GetAllUsers();

            // Assert
            Assert.That(users.Count(), Is.EqualTo(2));

        }

        [Test]
        public void GetUserById_Called_ReturnsCorrectUser()
        {
            // Arrange
            // nothing

            // Act
            User user = _service.GetUserById(1);

            // Assert 
            Assert.That(user.UserId , Is.EqualTo(1));
        }

        [Test]
        public void GetUserByEmail_Called_ReturnsCorrectUser()
        {
            // Arrange
            // nothing

            // Act
            User user = _service.GetUserByEmail("email");

            // Assert 
            Assert.That(user.Email, Is.EqualTo("email"));
        }

        [Test]
        public void GetUseByUsername_Called_ReturnsCorrectUser()
        {
            // Arrange
            // nothing

            // Act
            User user = _service.GetUserByUsername("user");

            // Assert 
            Assert.That(user.Username, Is.EqualTo("user"));
        }

        [Test]
        public void ValidateUserAndEmail_DuplicateEmail_ThrowsException()
        {
            // Arrange 
            var email = "DupeEmail";
            var username = "";

            // Act & Assert
            var ex = Assert.Throws<EmailAlreadyExistsException>(() =>
            {
                _service.ValidateUserAndEmail(email, username);
            });

            Assert.That(ex.Message, Is.EqualTo("An account with the email '" + email + "' already exists."));
        }

        [Test]
        public void ValidateUserAndEmail_DuplicateUser_ThrowsException()
        {
            // Arrange 
            var email = "";
            var username = "DupeUsername";

            // Act & Assert
            var ex = Assert.Throws<UsernameAlreadyTakenException>(() =>
            {
                _service.ValidateUserAndEmail(email, username);
            });

            Assert.That(ex.Message, Is.EqualTo($"The username '{username}' is already taken."));
        }

        [Test]
        public void ValidateUserAndEmail_EmptyEmailAndEmptyUsername_ThrowsException()
        {
            // Arrange 
            var email = "";
            var username = "";

            // Act & Assert
            var ex = Assert.Throws<UserValidationException>(() =>
            {
                _service.ValidateUserAndEmail(email, username);
            });

            Assert.That(ex.Message, Is.EqualTo("Unknown validation error: OTHER_ERROR"));
        }

        [Test]
        public void CreateUser_CorrectCredentials_UserCreated()
        {
            // Arrange 
            User user = new User();
            user.Email = "test@gmail.com";
            user.Username = "username";
            user.Password = "password";

            // Act
            User user1 = _service.CreateUser(user);

            // Assert
            Assert.That(user1.Username, Is.EqualTo("username"));
        }

        [Test]
        public void UpdateUser_CorrectUser_UserUpdated()
        {
            // Arrange
            User user = new User();
            user.Username = "test";

            // Act
            User user1 = _service.UpdateUser(user);

            // Assert
            Assert.That(user1.Username, Is.EqualTo("test"));
        }

        [Test]
        public void DeleteUser_CorrectUser_DoesntThrow()
        {
            // Arrange
            var user = new User();
            user.Username= "test";
            user.UserId = 1;

            // Act and Assert
            Assert.DoesNotThrow(() => _service.DeleteUser(user.UserId));

        }

        [Test]
        public void AcceptChanges_MatchingPasswords_ReturnsTrue()
        {
            // Arrange
            var user = new User();
            user.Username = "test";
            user.Password = "password";
            user.UserId = 1;

            // Act
            bool result = _service.AcceptChanges(user.UserId = 1, "password");

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void AcceptChanges_NotMatchingPasswords_ReturnsFalse()
        {
            // Arrange
            var user = new User();
            user.Username = "test";
            user.Password = "password";
            user.UserId = 1;

            // Act
            bool result = _service.AcceptChanges(user.UserId = 1, "notPassword");

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void UpdateUserEmail_CorrectUser_DoesntThrow()
        {
            // Arrange
            var user = new User();
            user.Username = "test";
            user.UserId = 1;
            user.Email = "test@gmail.com";

            // Act and Assert
            Assert.DoesNotThrow(() => _service.UpdateUserEmail(user.UserId, "test@yahoo.com"));

        }

        [Test]
        public void UpdateUserPassword_CorrectUser_DoesntThrow()
        {
            // Arrange
            var user = new User();
            user.Username = "test";
            user.UserId = 1;
            user.Password = "password";

            // Act and Assert
            Assert.DoesNotThrow(() => _service.UpdateUserPassword(user.UserId, PasswordHasher.HashPassword(user.Password)));

        }

        [Test]
        public void UpdateUserUsername_CorrectUser_DoesntThrow()
        {
            // Arrange
            var user = new User();
            user.Username = "test";
            user.UserId = 1;

            // Act and Assert
            Assert.DoesNotThrow(() => _service.UpdateUserUsername(user.UserId, "newTest"));

        }

        [Test]
        public void Login_UserNotNullAndGoodPassword_ReturnsUser()
        {
            // Arrange
            string mailOrUser = "test";
            string password = "password";

            // Act
            User user = _service.Login(mailOrUser, password);

            // Assert 
            Assert.That(user, Is.Not.Null);
        }

        [Test]
        public void Login_UserNotNullAndBadPassword_ReturnsNull()
        {
            // Arrange
            string mailOrUser = "test";
            string password = "passWrong";

            // Act
            User user = _service.Login(mailOrUser, password);

            // Assert 
            Assert.That(user, Is.Null);
        }

        [Test]
        public void Logout_Called_NoError()
        {
            // Arrange

            // Act and Assert
            Assert.DoesNotThrow(() => _service.Logout());
        }

        [Test]
        public void GetCurrentUser_Called_ReturnsUser()
        {
            // Arrange

            // Act
            User user = _service.GetCurrentUser();

            // Assert
            Assert.That(user, Is.Not.Null);
        }

        [Test]
        public void IsUserLoggedIn_Called_ReturnsTrue()
        {
            // Arrange

            // Act
            bool value = _service.IsUserLoggedIn();

            // Assert
            Assert.That(value, Is.True);
        }

        [Test]
        public void VerifyUserPassword_CorrectPassword_ReturnsTrue()
        {
            // Arrange
            string password = "password";

            // Act
            bool value = _service.VerifyUserPassword(password);

            // Assert
            Assert.That(value, Is.True);
        }

        [Test]
        public void VerifyUserPassword_WrongPassword_ReturnsFalse()
        {
            // Arrange
            string password = "passWrong";

            // Act
            bool value = _service.VerifyUserPassword(password);

            // Assert
            Assert.That(value, Is.False);
        }

        [Test]
        public void UpdateUserUsername_CorrectPassword_ReturnsTrue()
        {
            // Arrange
            string username = "username";
            string password = "password";

            // Act
            bool value = _service.UpdateUserUsername(username, password);

            // Assert
            Assert.That(value, Is.True);
        }

        [Test]
        public void UpdateUserUsername_WrongPassword_ReturnsFalse()
        {
            // Arrange
            string username = "username";
            string password = "passWrong";

            // Act
            bool value = _service.UpdateUserUsername(username, password);

            // Assert
            Assert.That(value, Is.False);
        }

        [Test]
        public void UpdateUserPassword_CorrectPassword_ReturnsTrue()
        {
            // Arrange
            string newPassword = "test";
            string password = "password";

            // Act
            bool value = _service.UpdateUserPassword(newPassword, password);

            // Assert
            Assert.That(value, Is.True);
        }

        [Test]
        public void UpdateUserPassword_WrongPassword_ReturnsFalse()
        {
            // Arrange
            string newPassword = "test";
            string password = "passWrong";

            // Act
            bool value = _service.UpdateUserPassword(newPassword, password);

            // Assert
            Assert.That(value, Is.False);
        }

        [Test]
        public void UpdateUserEmail_CorrectPassword_ReturnsTrue()
        {
            // Arrange
            string email = "test@gmail.com";
            string password = "password";

            // Act
            bool value = _service.UpdateUserEmail(email, password);

            // Assert
            Assert.That(value, Is.True);
        }

        [Test]
        public void UpdateUserEmail_WrongPassword_ReturnsFalse()
        {
            // Arrange
            string email = "test@gmail.com";
            string password = "passWrong";

            // Act
            bool value = _service.UpdateUserEmail(email, password);

            // Assert
            Assert.That(value, Is.False);
        }

        [Test]
        public void UserService_nullUserRepository_ThrowsException()
        {
            // Arange
            FakeSessionService mockService = new FakeSessionService();

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => new UserService(null, mockService));
        }

        [Test]
        public void UserService_nullSessionService_ThrowsException()
        {
            // Arange
            FakeUsersRepository mockUsersRepository = new FakeUsersRepository();

            // Act and Assert
            Assert.Throws<ArgumentNullException>(() => new UserService(mockUsersRepository, null));
        }
    }
}
