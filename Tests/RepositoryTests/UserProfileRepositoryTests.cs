using BusinessLayer.Data;
using BusinessLayer.Exceptions;
using BusinessLayer.Models;
using BusinessLayer.Repositories;
using Microsoft.Data.SqlClient;
using Moq;
using System.Data;

namespace Tests.RepositoryTests
{
    [TestFixture]
    internal class UserProfileRepositoryTests
    {
        private UserProfilesRepository _userProfilesRepository;
        private Mock<IDataLink> _mockDataLink;

        [SetUp]
        public void Setup()
        {
            _mockDataLink = new Mock<IDataLink>();
            _userProfilesRepository = new UserProfilesRepository(_mockDataLink.Object);
        }

        [Test]
        public void Constructor_NullDataLink_ThrowsArgumentNullException()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new UserProfilesRepository(null));
        }

        [Test]
        public void GetUserProfileByUserId_ValidUserId_ReturnsUserProfile()
        {
            // Arrange
            var userId = 1;
            var profileId = 101; // Example profile ID

            var dataTable = new DataTable();
            dataTable.Columns.Add("profile_id", typeof(int)); // Add the missing column
            dataTable.Columns.Add("user_id", typeof(int));
            dataTable.Columns.Add("bio", typeof(string));
            dataTable.Columns.Add("profile_picture", typeof(string));
            dataTable.Columns.Add("last_modified", typeof(DateTime));
            dataTable.Rows.Add(profileId, userId, "Test Bio", "TestPicture.jpg", DateTime.Now);

            _mockDataLink
                .Setup(dataLink => dataLink.ExecuteReader("GetUserProfileByUserId", It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act
            var result = _userProfilesRepository.GetUserProfileByUserId(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void GetUserProfileByUserId_NoRows_ReturnsNull()
        {
            // Arrange
            var userId = 1;
            var dataTable = new DataTable();

            _mockDataLink
                .Setup(dataLink => dataLink.ExecuteReader("GetUserProfileByUserId", It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act
            var result = _userProfilesRepository.GetUserProfileByUserId(userId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetUserProfileByUserId_DatabaseOperationException_ThrowsRepositoryException()
        {
            // Arrange
            var userId = 1;

            _mockDataLink
                .Setup(dataLink => dataLink.ExecuteReader("GetUserProfileByUserId", It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database Error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => _userProfilesRepository.GetUserProfileByUserId(userId));
        }

        [Test]
        public void UpdateProfile_ValidProfile_ReturnsUpdatedProfile()
        {
            // Arrange
            var profile = new UserProfile
            {
                ProfileId = 1,
                UserId = 1,
                Bio = "Test Bio",
            };

            var dataTable = new DataTable();
            dataTable.Columns.Add("profile_id", typeof(int)); // Add the missing column
            dataTable.Columns.Add("user_id", typeof(int));
            dataTable.Columns.Add("bio", typeof(string));
            dataTable.Columns.Add("profile_picture", typeof(string));
            dataTable.Columns.Add("last_modified", typeof(DateTime));
            dataTable.Rows.Add(profile.ProfileId, profile.UserId, profile.Bio, "TestPicture.jpg", DateTime.Now);

            _mockDataLink
                .Setup(dataLink => dataLink.ExecuteReader("UpdateUserProfile", It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act
            var result = _userProfilesRepository.UpdateProfile(profile);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void UpdateProfile_NoRows_ReturnsNull()
        {
            // Arrange
            var profile = new UserProfile
            {
                ProfileId = 1,
                UserId = 1,
                Bio = "Test Bio",
            };

            var dataTable = new DataTable();

            _mockDataLink
                .Setup(dataLink => dataLink.ExecuteReader("UpdateUserProfile", It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act
            var result = _userProfilesRepository.UpdateProfile(profile);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void UpdateProfile_DatabaseOperationException_ThrowsRepositoryException()
        {
            // Arrange
            var profile = new UserProfile
            {
                ProfileId = 1,
                UserId = 1,
                Bio = "Test Bio",
            };

            _mockDataLink
                .Setup(dataLink => dataLink.ExecuteReader("UpdateUserProfile", It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database Error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => _userProfilesRepository.UpdateProfile(profile));
        }

        [Test]
        public void UpdateProfile_NullBio_SetsBioToDBNull()
        {
            // Arrange
            var profile = new UserProfile
            {
                ProfileId = 1,
                UserId = 1,
                Bio = null,
                ProfilePicture = "pic.png"
            };

            var dataTable = new DataTable();
            dataTable.Columns.Add("profile_id", typeof(int));
            dataTable.Columns.Add("user_id", typeof(int));
            dataTable.Columns.Add("bio", typeof(string));
            dataTable.Columns.Add("profile_picture", typeof(string));
            dataTable.Columns.Add("last_modified", typeof(DateTime));
            dataTable.Rows.Add(profile.ProfileId, profile.UserId, DBNull.Value, "pic.png", DateTime.Now);

            _mockDataLink
                .Setup(dataLink => dataLink.ExecuteReader("UpdateUserProfile", It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act
            var result = _userProfilesRepository.UpdateProfile(profile);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void UpdateProfile_WithProfilePicture_SetsProfilePictureParam()
        {
            // Arrange
            var profile = new UserProfile
            {
                ProfileId = 1,
                UserId = 1,
                Bio = "Test Bio",
                ProfilePicture = "pic.png"
            };

            var dataTable = new DataTable();
            dataTable.Columns.Add("profile_id", typeof(int));
            dataTable.Columns.Add("user_id", typeof(int));
            dataTable.Columns.Add("bio", typeof(string));
            dataTable.Columns.Add("profile_picture", typeof(string));
            dataTable.Columns.Add("last_modified", typeof(DateTime));
            dataTable.Rows.Add(profile.ProfileId, profile.UserId, profile.Bio, profile.ProfilePicture, DateTime.Now);

            _mockDataLink
                .Setup(dataLink => dataLink.ExecuteReader("UpdateUserProfile", It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act
            var result = _userProfilesRepository.UpdateProfile(profile);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void CreateProfile_ValidUserId_ReturnsNewProfile()
        {
            // Arrange
            var userId = 1;
            var profileId = 101; // Example profile ID

            var dataTable = new DataTable();
            dataTable.Columns.Add("profile_id", typeof(int)); // Add the missing column
            dataTable.Columns.Add("user_id", typeof(int));
            dataTable.Columns.Add("bio", typeof(string));
            dataTable.Columns.Add("profile_picture", typeof(string));
            dataTable.Columns.Add("last_modified", typeof(DateTime));
            dataTable.Rows.Add(profileId, userId, "Test Bio", "TestPicture.jpg", DateTime.Now);

            _mockDataLink
                .Setup(dataLink => dataLink.ExecuteReader("CreateUserProfile", It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act
            var result = _userProfilesRepository.CreateProfile(userId);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void CreateProfile_NoRows_ReturnsNull()
        {
            // Arrange
            var userId = 1;
            var dataTable = new DataTable();

            _mockDataLink
                .Setup(dataLink => dataLink.ExecuteReader("CreateUserProfile", It.IsAny<SqlParameter[]>()))
                .Returns(dataTable);

            // Act
            var result = _userProfilesRepository.CreateProfile(userId);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void CreateProfile_DatabaseOperationException_ThrowsRepositoryException()
        {
            // Arrange
            var userId = 1;

            _mockDataLink
                .Setup(dataLink => dataLink.ExecuteReader("CreateUserProfile", It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database Error"));

            // Act & Assert
            Assert.Throws<RepositoryException>(() => _userProfilesRepository.CreateProfile(userId));
        }

        [Test]
        public void UpdateProfileBio_ValidInputs_ExecutesWithoutException()
        {
            // Arrange
            var userId = 1;
            var bio = "This is a test bio.";

            _mockDataLink
                .Setup(dataLink => dataLink.ExecuteReader("UpdateUserProfileBio", It.IsAny<SqlParameter[]>()))
                .Returns(new DataTable());

            // Act & Assert
            Assert.DoesNotThrow(() => _userProfilesRepository.UpdateProfileBio(userId, bio));
        }

        [Test]
        public void UpdateProfileBio_DatabaseOperationException_ThrowsRepositoryException()
        {
            // Arrange
            var userId = 1;
            var bio = "This is a test bio.";

            _mockDataLink
                .Setup(dataLink => dataLink.ExecuteReader("UpdateUserProfileBio", It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _userProfilesRepository.UpdateProfileBio(userId, bio));
            Assert.That(ex.Message, Is.EqualTo($"Failed to update profile for user {userId}."));
        }

        [Test]
        public void UpdateProfilePicture_ValidInputs_ExecutesWithoutException()
        {
            // Arrange
            var userId = 1;
            var picture = "profile_picture_url";

            _mockDataLink
                .Setup(dataLink => dataLink.ExecuteReader("UpdateUserProfilePicture", It.IsAny<SqlParameter[]>()))
                .Returns(new DataTable());

            // Act & Assert
            Assert.DoesNotThrow(() => _userProfilesRepository.UpdateProfilePicture(userId, picture));
        }

        [Test]
        public void UpdateProfilePicture_DatabaseOperationException_ThrowsRepositoryException()
        {
            // Arrange
            var userId = 1;
            var picture = "profile_picture_url";

            _mockDataLink
                .Setup(dataLink => dataLink.ExecuteReader("UpdateUserProfilePicture", It.IsAny<SqlParameter[]>()))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _userProfilesRepository.UpdateProfilePicture(userId, picture));
            Assert.That(ex.Message, Is.EqualTo($"Failed to update profile for user {userId}."));
        }
    }
}
