using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Services;
using BusinessLayer.Services.fakes;
using BusinessLayer.Repositories.fakes;
using BusinessLayer.Models;
using BusinessLayer.Exceptions;

namespace Tests
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
            // nothing

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
            // nothing

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
            // nothing

            // Act & Assert
            var ex = Assert.Throws<UserValidationException>(() =>
            {
                _service.ValidateUserAndEmail(email, username);
            });

            Assert.That(ex.Message, Is.EqualTo("Unknown validation error: OTHER_ERROR"));
        }


    }
}
