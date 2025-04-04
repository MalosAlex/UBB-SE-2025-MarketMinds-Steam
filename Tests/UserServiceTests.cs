using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Services;
using BusinessLayer.Services.fakes;
using BusinessLayer.Repositories.fakes;
using BusinessLayer.Models;

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
        public async Task GetAllUsers_Called_ReturnsCorrectCount()
        {
            // Arrange
            // nothing

            // Act
            List<User> users = _service.GetAllUsers();

            // Assert
            Assert.That(users.Count(), Is.EqualTo(2));

        }

        [Test]
        public async Task GetUserById_Called_ReturnsCorrectUser()
        {
            // Arrange
            // nothing

            // Act
            User user = _service.GetUserById(1);

            // Assert 
            Assert.That(user.UserId , Is.EqualTo(1));
        }
    }
}
