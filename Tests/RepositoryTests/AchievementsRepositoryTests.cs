using BusinessLayer.Data;
using BusinessLayer.Exceptions;
using BusinessLayer.Models;
using BusinessLayer.Repositories;
using Microsoft.Data.SqlClient;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tests.RepositoryTests
{
    [TestFixture]
    public class AchievementsRepositoryTests
    {
        private AchievementsRepository _repo;
        private Mock<IDataLink> _mockDataLink;

        [SetUp]
        public void Setup()
        {
            _mockDataLink = new Mock<IDataLink>();
            _repo = new AchievementsRepository(_mockDataLink.Object);
        }

        [Test]
        public void Constructor_WhenDataLinkIsNull_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => new AchievementsRepository(null));
            Assert.That(ex.ParamName, Is.EqualTo("datalink"));
        }

        [Test]
        public void InsertAchievements_WhenNoException_DoesNotThrow()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteNonQuery("InsertAchievements", null)).Returns(1);


            // Act & Assert
            Assert.DoesNotThrow(() => _repo.InsertAchievements());
        }

        [Test]
        public void InsertAchievements_WhenException_ThrowsRepositoryException()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteNonQuery("InsertAchievements", null)).Throws(new Exception("Simulated DB failure"));

            // Act A
            var ex = Assert.Throws<RepositoryException>(() => _repo.InsertAchievements());

            // Assert
            Assert.That(ex.Message, Does.Contain("inserting achievements"));
        }

        public class FakeSqlException : Exception { }

        [Test]
        public void IsAchievementsTableEmpty_WhenResultIsZero_ReturnsTrue()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteScalar<int>("IsAchievementsTableEmpty" , null)).Returns(0);

            // Act
            bool result = _repo.IsAchievementsTableEmpty();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsAchievementsTableEmpty_WhenResultIsFive_ReturnsFalse()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteScalar<int>("IsAchievementsTableEmpty" , null)).Returns(5);

            // Act
            bool result = _repo.IsAchievementsTableEmpty();

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsAchievementsTableEmpty_WhenGeneralExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            _mockDataLink
                .Setup(dl => dl.ExecuteScalar<int>("IsAchievementsTableEmpty" , null))
                .Throws(new Exception("Something went wrong"));

            // Act
            var ex = Assert.Throws<RepositoryException>(() => _repo.IsAchievementsTableEmpty());

            // Assert
            Assert.That(ex.Message, Does.Contain("unexpected error occurred while checking if achievements table is empty"));
        }

        [Test]
        public void UpdateAchievementIconUrl_WhenCalledWithValidData_DoesNotThrow()
        {
            // Arrange
            _mockDataLink
                .Setup(dl => dl.ExecuteNonQuery("UpdateAchievementIcon", It.IsAny<SqlParameter[]>()))
                .Returns(1); // simulate success

            // Act & Assert
            Assert.DoesNotThrow(() => _repo.UpdateAchievementIconUrl(10, "icon.png"));
        }

        [Test]
        public void UpdateAchievementIconUrl_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteNonQuery("UpdateAchievementIcon", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("test"));

            // Act
            var ex = Assert.Throws<RepositoryException>(() => _repo.UpdateAchievementIconUrl(10, "url"));

            // Assert
            Assert.That(ex.Message, Does.Contain("updating achievement icon URL"));
        }

        [Test]
        public void GetAllAchievements_WhenDataReturned_MapsCorrectly()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("achievement_id", typeof(int));
            table.Columns.Add("achievement_name", typeof(string));
            table.Columns.Add("description", typeof(string));
            table.Columns.Add("achievement_type", typeof(string));
            table.Columns.Add("points", typeof(int));
            table.Columns.Add("icon_url", typeof(string));

            table.Rows.Add(1, "ACH1", "Desc", "TypeA", 10, "url.png");

            _mockDataLink.Setup(dl => dl.ExecuteReader("GetAllAchievements" , null)).Returns(table);

            // Act
            var result = _repo.GetAllAchievements();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].AchievementName, Is.EqualTo("ACH1"));
        }

        [Test]
        public void GetAllAchievements_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetAllAchievements" , null))
                         .Throws(new Exception("test"));

            // Act
            var ex = Assert.Throws<RepositoryException>(() => _repo.GetAllAchievements());

            // Assert
            Assert.That(ex.Message, Does.Contain("retrieving achievements"));
        }

        [Test]
        public void GetUnlockedAchievementsForUser_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            var parameters = new SqlParameter[] { new SqlParameter("@userId", 1) };

            _mockDataLink.Setup(dl => dl.ExecuteReader("GetUnlockedAchievements", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("test"));

            // Act
            var ex = Assert.Throws<RepositoryException>(() => _repo.GetUnlockedAchievementsForUser(1));
            
            // Assert
            Assert.That(ex.Message, Does.Contain("retrieving unlocked achievements"));
        }

        [Test]
        public void GetUnlockedAchievementsForUser_WhenCalledWithValidUserId_ReturnsAchievements()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("achievement_id", typeof(int));
            table.Columns.Add("achievement_name", typeof(string));
            table.Columns.Add("description", typeof(string));
            table.Columns.Add("achievement_type", typeof(string));
            table.Columns.Add("points", typeof(int));
            table.Columns.Add("icon_url", typeof(string));

            table.Rows.Add(1, "ACH1", "Desc", "Type", 10, "url.png");

            _mockDataLink
                .Setup(dl => dl.ExecuteReader("GetUnlockedAchievements", It.IsAny<SqlParameter[]>()))
                .Returns(table);

            // Act
            var result = _repo.GetUnlockedAchievementsForUser(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].AchievementName, Is.EqualTo("ACH1"));
        }

        [Test]
        public void UnlockAchievement_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteNonQuery("UnlockAchievement", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("test"));

            // Act
            var ex = Assert.Throws<RepositoryException>(() => _repo.UnlockAchievement(1, 100));
            
            // Assert
            Assert.That(ex.Message, Does.Contain("unlocking achievement"));
        }

        [Test]
        public void UnlockAchievement_WhenCalledWithValidData_DoesNotThrow()
        {
            // Arrange
            _mockDataLink
                .Setup(dl => dl.ExecuteNonQuery("UnlockAchievement", It.IsAny<SqlParameter[]>()))
                .Returns(1); // simulate successful update

            // Act & Assert
            Assert.DoesNotThrow(() => _repo.UnlockAchievement(1, 100));
        }

        [Test]
        public void RemoveAchievement_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteNonQuery("RemoveAchievement", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("test"));

            // Act 
            var ex = Assert.Throws<RepositoryException>(() => _repo.RemoveAchievement(1, 100));

            // Assert
            Assert.That(ex.Message, Does.Contain("removing achievement"));
        }

        [Test]
        public void RemoveAchievement_WhenCalledWithValidData_DoesNotThrow()
        {
            // Arrange
            _mockDataLink
                .Setup(dl => dl.ExecuteNonQuery("RemoveAchievement", It.IsAny<SqlParameter[]>()))
                .Returns(1);

            // Act & Assert
            Assert.DoesNotThrow(() => _repo.RemoveAchievement(1, 100));
        }

        [Test]
        public void GetUnlockedDataForAchievement_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetUnlockedDataForAchievement", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("test"));

            // Act 
            var ex = Assert.Throws<RepositoryException>(() => _repo.GetUnlockedDataForAchievement(1, 2));

            // Assert
            Assert.That(ex.Message, Does.Contain("retrieving achievement data"));
        }

        [Test]
        public void GetUnlockedDataForAchievement_WhenNoData_ReturnsNull()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("AchievementName", typeof(string));
            table.Columns.Add("AchievementDescription", typeof(string));
            table.Columns.Add("UnlockDate", typeof(DateTime));

            // Note: No rows added!

            _mockDataLink
                .Setup(dl => dl.ExecuteReader("GetUnlockedDataForAchievement", It.IsAny<SqlParameter[]>()))
                .Returns(table);

            // Act
            var result = _repo.GetUnlockedDataForAchievement(1, 1);

            // Assert
            Assert.That(result, Is.Null);
        }


        [Test]
        public void GetUnlockedDataForAchievement_WhenDataExists_ReturnsData()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("AchievementName", typeof(string));
            table.Columns.Add("AchievementDescription", typeof(string));
            table.Columns.Add("UnlockDate", typeof(DateTime));

            table.Rows.Add("ACH1", "Description", DateTime.Today);

            _mockDataLink
                .Setup(dl => dl.ExecuteReader("GetUnlockedDataForAchievement", It.IsAny<SqlParameter[]>()))
                .Returns(table);

            // Act
            var result = _repo.GetUnlockedDataForAchievement(1, 1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.AchievementName, Is.EqualTo("ACH1"));
        }


        [Test]
        public void GetAllAchievements_WhenDataReturned_MapsAllPropertiesCorrectly()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("achievement_id", typeof(int));
            table.Columns.Add("achievement_name", typeof(string));
            table.Columns.Add("description", typeof(string));
            table.Columns.Add("achievement_type", typeof(string));
            table.Columns.Add("points", typeof(int));
            table.Columns.Add("icon_url", typeof(string));

            table.Rows.Add(1, "ACH1", "Great job!", "TypeA", 10, "http://icon.url");

            _mockDataLink
                .Setup(dl => dl.ExecuteReader("GetAllAchievements", null))
                .Returns(table);

            // Act
            var results = _repo.GetAllAchievements();

            // Assert
            Assert.That(results.Count, Is.EqualTo(1));
            var ach = results[0];
            Assert.Multiple(() =>
            {
                Assert.That(ach.AchievementId, Is.EqualTo(1));
                Assert.That(ach.AchievementName, Is.EqualTo("ACH1"));
                Assert.That(ach.Description, Is.EqualTo("Great job!"));
                Assert.That(ach.AchievementType, Is.EqualTo("TypeA"));
                Assert.That(ach.Points, Is.EqualTo(10)); // ✅ this is the key line for that yellow mark
                Assert.That(ach.Icon, Is.EqualTo("http://icon.url"));
            });
        }
        [Test]
        public void IsAchievementUnlocked_WhenResultIs1_ReturnsTrue()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteScalar<int>("IsAchievementUnlocked", It.IsAny<SqlParameter[]>())).Returns(1);

            // Act
            bool isUnlocked = _repo.IsAchievementUnlocked(1, 2);

            // Assert
            Assert.That(isUnlocked, Is.True);
        }

        [Test]
        public void IsAchievementUnlocked_WhenResultIs0_ReturnsFalse()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteScalar<int>("IsAchievementUnlocked", It.IsAny<SqlParameter[]>())).Returns(0);

            // Act
            bool isUnlocked = _repo.IsAchievementUnlocked(1, 2);

            // Assert
            Assert.That(isUnlocked, Is.False);
        }

        [Test]
        public void IsAchievementUnlocked_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            _mockDataLink
                .Setup(dl => dl.ExecuteScalar<int>("IsAchievementUnlocked", It.IsAny<SqlParameter[]>()))
                .Throws(new Exception("Simulated failure"));

            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repo.IsAchievementUnlocked(1, 10));
            Assert.That(ex.Message, Does.Contain("achievement is unlocked"));
        }


        private DataTable CreateMockAchievementTable()
        {
            var table = new DataTable();
            table.Columns.Add("achievement_id", typeof(int));
            table.Columns.Add("achievement_name", typeof(string));
            table.Columns.Add("description", typeof(string));
            table.Columns.Add("achievement_type", typeof(string));
            table.Columns.Add("points", typeof(int));
            table.Columns.Add("icon_url", typeof(string));

            table.Rows.Add(1, "ACH1", "Desc", "Type", 10, "url.png");
            return table;
        }

        private DataTable CreateMockUnlockDataTable()
        {
            var table = new DataTable();
            table.Columns.Add("AchievementName", typeof(string));
            table.Columns.Add("AchievementDescription", typeof(string));
            table.Columns.Add("UnlockDate", typeof(DateTime));

            table.Rows.Add("ACH1", "Desc", DateTime.Today);
            return table;
        }

        [Test]
        public void GetAchievementsWithStatusForUser_WhenCalled_ReturnsAchievementsWithStatus()
        {
            // Arrange
            var achievements = new List<Achievement>
    {
        new Achievement { AchievementId = 1, AchievementName = "A1" }
    };

            _mockDataLink.Setup(dl => dl.ExecuteReader("GetAllAchievements" ,null))
                         .Returns(CreateMockAchievementTable());

            _mockDataLink.Setup(dl => dl.ExecuteScalar<int>("IsAchievementUnlocked", It.IsAny<SqlParameter[]>()))
                         .Returns(1);

            _mockDataLink.Setup(dl => dl.ExecuteReader("GetUnlockedDataForAchievement", It.IsAny<SqlParameter[]>()))
                         .Returns(CreateMockUnlockDataTable());

            // Act
            var result = _repo.GetAchievementsWithStatusForUser(1);

            // Assert
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].IsUnlocked, Is.True);
        }

        [Test]
        public void GetAchievementsWithStatusForUser_WhenUnlockedDataIsNull_SetsUnlockedDateToNull()
        {
            // Arrange
            var table = new DataTable();
            table.Columns.Add("achievement_id", typeof(int));
            table.Columns.Add("achievement_name", typeof(string));
            table.Columns.Add("description", typeof(string));
            table.Columns.Add("achievement_type", typeof(string));
            table.Columns.Add("points", typeof(int));
            table.Columns.Add("icon_url", typeof(string));
            table.Rows.Add(1, "A1", "Desc", "Type", 10, "url");

            _mockDataLink.Setup(dl => dl.ExecuteReader("GetAllAchievements",null))
                         .Returns(table);

            _mockDataLink.Setup(dl => dl.ExecuteScalar<int>("IsAchievementUnlocked", It.IsAny<SqlParameter[]>()))
                         .Returns(1);

            _mockDataLink.Setup(dl => dl.ExecuteReader("GetUnlockedDataForAchievement", It.IsAny<SqlParameter[]>()))
                         .Returns(new DataTable()); // empty table = null unlockedData

            // Act
            var result = _repo.GetAchievementsWithStatusForUser(1);

            // Assert
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].UnlockedDate, Is.Null); // <-- key point!
        }


        [Test]
        public void GetAchievementsWithStatusForUser_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteReader("GetAllAchievements" , null))
                         .Throws(new Exception("DB error"));

            // Act
            var ex = Assert.Throws<RepositoryException>(() => _repo.GetAchievementsWithStatusForUser(1));
            
            // Assert
            Assert.That(ex.Message, Does.Contain("achievements with status"));
        }

        [Test]
        public void GetNumberOfSoldGames_WhenCalled_ReturnsValue()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteScalar<int>("GetNumberOfSoldGames", It.IsAny<SqlParameter[]>()))
                         .Returns(5);

            // Act
            var result = _repo.GetNumberOfSoldGames(1);

            // Assert
            Assert.That(result, Is.EqualTo(5));
        }

        [Test]
        public void GetNumberOfSoldGames_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteScalar<int>("GetNumberOfSoldGames", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("fail"));

            // Act
            var ex = Assert.Throws<RepositoryException>(() => _repo.GetNumberOfSoldGames(1));

            // Assert
            Assert.That(ex.Message, Does.Contain("sold games"));
        }

        [Test]
        public void GetFriendshipCount_WhenCalled_ReturnsCount()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteScalar<int>("GetFriendshipCountForUser", It.IsAny<SqlParameter[]>()))
                         .Returns(2);

            // Act
            var result = _repo.GetFriendshipCount(1);

            // Assert
            Assert.That(result, Is.EqualTo(2));
        }

        [Test]
        public void GetFriendshipCount_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteScalar<int>("GetFriendshipCountForUser", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("fail"));

            // Act
            var ex = Assert.Throws<RepositoryException>(() => _repo.GetFriendshipCount(1));
            
            // Assert
            Assert.That(ex.Message, Does.Contain("friendship count"));
        }

        [Test]
        public void GetNumberOfOwnedGames_WhenCalled_ReturnsCount()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteScalar<int>("GetNumberOfOwnedGames", It.IsAny<SqlParameter[]>()))
                         .Returns(3);

            // Act
            var result = _repo.GetNumberOfOwnedGames(1);

            // Assert
            Assert.That(result, Is.EqualTo(3));
        }

        [Test]
        public void GetNumberOfOwnedGames_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteScalar<int>("GetNumberOfOwnedGames", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("fail"));

            // Act
            var ex = Assert.Throws<RepositoryException>(() => _repo.GetNumberOfOwnedGames(1));

            // Assert
            Assert.That(ex.Message, Does.Contain("owned games"));
        }

        [Test]
        public void GetNumberOfReviewsGiven_WhenCalled_ReturnsCount()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteScalar<int>("GetNumberOfReviewsGiven", It.IsAny<SqlParameter[]>()))
                         .Returns(10);

            // Act
            var result = _repo.GetNumberOfReviewsGiven(1);

            // Assert
            Assert.That(result, Is.EqualTo(10));
        }

        [Test]
        public void GetNumberOfReviewsGiven_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteScalar<int>("GetNumberOfReviewsGiven", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("fail"));

            // Act
            var ex = Assert.Throws<RepositoryException>(() => _repo.GetNumberOfReviewsGiven(1));

            // Assert
            Assert.That(ex.Message, Does.Contain("reviews given"));
        }

        [Test]
        public void GetNumberOfReviewsReceived_WhenCalled_ReturnsCount()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteScalar<int>("GetNumberOfReviewsReceived", It.IsAny<SqlParameter[]>()))
                         .Returns(8);

            // Act
            var result = _repo.GetNumberOfReviewsReceived(1);

            // Assert
            Assert.That(result, Is.EqualTo(8));
        }

        [Test]
        public void GetNumberOfReviewsReceived_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteScalar<int>("GetNumberOfReviewsReceived", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("Simulated failure"));

            // Act
            var ex = Assert.Throws<RepositoryException>(() => _repo.GetNumberOfReviewsReceived(1));

            // Assert
            Assert.That(ex.Message, Does.Contain("reviews received"));
        }

        [Test]
        public void GetYearsOfActivity_WhenUserCreatedEarlier_ReturnsCorrectYears()
        {
            // Arrange
            var now = DateTime.Now;
            var fiveYearsAgo = now.AddYears(-5);

            _mockDataLink.Setup(dl => dl.ExecuteScalar<DateTime>("GetUserCreatedAt", It.IsAny<SqlParameter[]>()))
                         .Returns(fiveYearsAgo);

            // Act
            var result = _repo.GetYearsOfAcftivity(1);

            // Assert
            Assert.That(result, Is.EqualTo(5).Or.EqualTo(4)); // Adjusted by day of year
        }

        [Test]
        public void GetYearsOfActivity_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteScalar<DateTime>("GetUserCreatedAt", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("Database fail"));

            // Act
            var ex = Assert.Throws<RepositoryException>(() => _repo.GetYearsOfAcftivity(1));

            // Assert
            Assert.That(ex.Message, Does.Contain("years of activity"));
        }

        [Test]
        public void GetAchievementIdByName_WhenFound_ReturnsId()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteScalar<int?>("GetAchievementIdByName", It.IsAny<SqlParameter[]>()))
                         .Returns(42);

            // Act
            var result = _repo.GetAchievementIdByName("ACH_NAME");

            // Assert
            Assert.That(result, Is.EqualTo(42));
        }

        [Test]
        public void GetAchievementIdByName_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteScalar<int?>("GetAchievementIdByName", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("Database fail"));

            // Act
            var ex = Assert.Throws<RepositoryException>(() => _repo.GetAchievementIdByName("ACH_NAME"));

            // Assert
            Assert.That(ex.Message, Does.Contain("achievement ID"));
        }

        [Test]
        public void IsUserDeveloper_WhenUserIsDeveloper_ReturnsTrue()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteScalar<bool>("IsUserDeveloper", It.IsAny<SqlParameter[]>()))
                         .Returns(true);

            // Act
            var result = _repo.IsUserDeveloper(1);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsUserDeveloper_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteScalar<bool>("IsUserDeveloper", It.IsAny<SqlParameter[]>()))
                         .Throws(new Exception("fail"));

            // Act
            var ex = Assert.Throws<RepositoryException>(() => _repo.IsUserDeveloper(1));

            // Assert
            Assert.That(ex.Message, Does.Contain("developer status"));
        }


        [Test]
        public void GetNumberOfPosts_WhenReturns3_Returns3()
        {
            // Arrange
            _mockDataLink.Setup(dl => dl.ExecuteScalar<int>("GetNumberOfPosts", It.IsAny<SqlParameter[]>())).Returns(3);

            // Act
            int posts = _repo.GetNumberOfPosts(1);

            // Assert
            Assert.That(posts, Is.EqualTo(3));
        }

        [Test]
        public void GetNumberOfPosts_WhenExceptionThrown_ThrowsRepositoryException()
        {
            // Arrange
            _mockDataLink
                .Setup(dl => dl.ExecuteScalar<int>("GetNumberOfPosts", It.IsAny<SqlParameter[]>()))
                .Throws(new Exception("Simulated DB failure"));

            // Act & Assert
            var ex = Assert.Throws<RepositoryException>(() => _repo.GetNumberOfPosts(1));
            Assert.That(ex.Message, Does.Contain("number of posts"));
        }
    }
}
