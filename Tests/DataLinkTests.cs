using BusinessLayer.Data;
using BusinessLayer.Exceptions;
using Microsoft.Data.SqlClient;
using NUnit.Framework;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Tests
{
    [TestFixture]
    public class DataLinkTests
    {
        // DataLink is a singleton with a direct database dependency.
        private DataLink _dataLink;
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Get the singleton instance for use in all tests.
            _dataLink = DataLink.Instance;
        }

        [Test]
        public void Instance_ReturnsNonNullInstance()
        {
            // Act
            var instance = DataLink.Instance;
            // Assert
            Assert.That(instance, Is.Not.Null);
        }

        [Test]
        public void Instance_ReturnsSameInstanceOnMultipleCalls()
        {
            // Act
            var instance1 = DataLink.Instance;
            var instance2 = DataLink.Instance;
            // Assert
            Assert.That(instance2, Is.SameAs(instance1));
        }
        
        [Test]
        public void ExecuteScalar_GetFriendshipCountForUser_ReturnsExpectedValue()
        {
            // Assume that for user_id = 1, the seed data in your test DB indicates 2 friendships.
            string storedProcedure = "GetFriendshipCountForUser";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@user_id", SqlDbType.Int) { Value = 1 }
            };

            // Act
            int count = _dataLink.ExecuteScalar<int>(storedProcedure, parameters);
            // Assert
            Assert.That(count, Is.EqualTo(2));
        }
        
        [Test]
        public void ExecuteScalar_GetFriendshipId_WithValidParameters_ReturnsExpectedValue()
        {
            // Assume that for user_id = 1 and friend_id = 2, the friendship exists with friendship_id = 1.
            string storedProcedure = "GetFriendshipId";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@user_id", SqlDbType.Int) { Value = 1 },
                new SqlParameter("@friend_id", SqlDbType.Int) { Value = 2 }
            };

            // Act
            int friendshipId = _dataLink.ExecuteScalar<int>(storedProcedure, parameters);
            // Assert
            Assert.That(friendshipId, Is.EqualTo(1));
        }
        
        [Test]
        public void ExecuteReader_GetFriendsForUser_ReturnsDataTableWithExpectedStructure()
        {
            // For user_id = 1, expect to get the list of friendships.
            string storedProcedure = "GetFriendsForUser";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@user_id", SqlDbType.Int) { Value = 1 }
            };

            // Act
            DataTable dt = _dataLink.ExecuteReader(storedProcedure, parameters);
            
            // Assert that the DataTable has the correct columns and row count based on your seeded data.
            Assert.That(dt.Columns.Contains("friendship_id"), Is.True);
            Assert.That(dt.Columns.Contains("user_id"), Is.True);
            Assert.That(dt.Columns.Contains("friend_id"), Is.True);
            Assert.That(dt.Rows.Count, Is.EqualTo(2));
        }
        
        [Test]
        public void ExecuteReader_GetUserById_ReturnsUserData()
        {
            // For user_id = 1, assume the user is present and has username "User1".
            string storedProcedure = "GetUserById";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@user_id", SqlDbType.Int) { Value = 1 }
            };

            // Act
            DataTable dt = _dataLink.ExecuteReader(storedProcedure, parameters);

            // Assert that the DataTable contains the expected user data.
            Assert.That(dt.Columns.Contains("user_id"), Is.True);
            Assert.That(dt.Columns.Contains("username"), Is.True);
            Assert.That(dt.Rows.Count, Is.EqualTo(1));
            Assert.That(dt.Rows[0]["username"].ToString(), Is.EqualTo("User1"));
        }
        
        [Test]
        public void ExecuteReader_GetUserProfileByUserId_ReturnsProfileData()
        {
            // For user_id = 2, assume the profile exists and the profile_picture is "alice.jpg".
            string storedProcedure = "GetUserProfileByUserId";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@user_id", SqlDbType.Int) { Value = 2 }
            };

            // Act
            DataTable dt = _dataLink.ExecuteReader(storedProcedure, parameters);

            // Assert that the DataTable contains the expected profile data.
            Assert.That(dt.Columns.Contains("user_id"), Is.True);
            Assert.That(dt.Columns.Contains("profile_picture"), Is.True);
            Assert.That(dt.Rows.Count, Is.EqualTo(1));
            Assert.That(dt.Rows[0]["profile_picture"].ToString(), Is.EqualTo("alice.jpg"));
        }
        
        [Test]
        public void ExecuteNonQuery_WithInvalidProcedureName_ThrowsDatabaseOperationException()
        {
            // Using an invalid stored procedure name should throw an exception.
            string invalidProcedure = "NonExistentProcedure";
            
            // Act & Assert
            Assert.That(() => _dataLink.ExecuteNonQuery(invalidProcedure),
                Throws.TypeOf<DatabaseOperationException>());
        }
        
        [Test]
        public async Task ExecuteReaderAsync_WithInvalidProcedureName_ThrowsDatabaseOperationException()
        {
            // Arrange
            string invalidProcedure = "NonExistentProcedure";
            
            // Act & Assert
            Assert.That(async () => await _dataLink.ExecuteReaderAsync(invalidProcedure),
                Throws.TypeOf<DatabaseOperationException>());
        }
        
        [Test]
        public async Task ExecuteNonQueryAsync_WithInvalidProcedureName_ThrowsDatabaseOperationException()
        {
            // Arrange
            string invalidProcedure = "NonExistentProcedure";
            
            // Act & Assert
            Assert.That(async () => await _dataLink.ExecuteNonQueryAsync(invalidProcedure),
                Throws.TypeOf<DatabaseOperationException>());
        }
        
        [Test]
        public void ExecuteScalar_NullResult_ReturnsDefaultValue()
        {
            // Assume that "GetFriendshipId" returns NULL (or no result) when the friendship does not exist.
            // For user_id = 1 and friend_id = 999 (non-existent friend), expect default (0 or null)
            string storedProcedure = "GetFriendshipId";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@user_id", SqlDbType.Int) { Value = 1 },
                new SqlParameter("@friend_id", SqlDbType.Int) { Value = 999 }
            };

            // Act
            int? result = _dataLink.ExecuteScalar<int?>(storedProcedure, parameters);

            // Assert - expecting a null result.
            Assert.That(result, Is.Null);
        }
        
        [Test]
        public void ExecuteReader_GetAllCollectionsForUser_ReturnsAllCollections()
        {
            // For user_id = 1, assume there are 3 collections.
            string storedProcedure = "GetAllCollectionsForUser";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@user_id", SqlDbType.Int) { Value = 1 }
            };

            // Act
            DataTable dt = _dataLink.ExecuteReader(storedProcedure, parameters);
            // Assert
            Assert.That(dt.Columns.Contains("collection_id"), Is.True);
            Assert.That(dt.Columns.Contains("name"), Is.True);
            Assert.That(dt.Rows.Count, Is.EqualTo(3));
        }
        
        [Test]
        public void ExecuteReader_GetPublicCollectionsForUser_ReturnsOnlyPublicCollections()
        {
            // For user_id = 1, assume only 2 of the seeded collections are public.
            string storedProcedure = "GetPublicCollectionsForUser";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@user_id", SqlDbType.Int) { Value = 1 }
            };

            // Act
            DataTable dt = _dataLink.ExecuteReader(storedProcedure, parameters);
            // Assert
            Assert.That(dt.Columns.Contains("collection_id"), Is.True);
            Assert.That(dt.Columns.Contains("is_public"), Is.True);
            Assert.That(dt.Rows.Count, Is.EqualTo(2));
        }
        
        [Test]
        public void ExecuteReader_GetGamesInCollection_ReturnsGamesInCollection()
        {
            // For collection_id = 1, assume there are 2 games.
            string storedProcedure = "GetGamesInCollection";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@collection_id", SqlDbType.Int) { Value = 1 }
            };

            // Act
            DataTable dt = _dataLink.ExecuteReader(storedProcedure, parameters);
            // Assert
            Assert.That(dt.Columns.Contains("game_id"), Is.True);
            Assert.That(dt.Columns.Contains("title"), Is.True);
            Assert.That(dt.Rows.Count, Is.EqualTo(2));
        }
        
        [Test]
        public void ExecuteReader_GetGamesNotInCollection_ReturnsGamesNotInSpecifiedCollection()
        {
            // For user_id = 1 and collection_id = 1, assume there is 1 game not in that collection.
            string storedProcedure = "GetGamesNotInCollection";
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@collection_id", SqlDbType.Int) { Value = 1 },
                new SqlParameter("@user_id", SqlDbType.Int) { Value = 1 }
            };

            // Act
            DataTable dt = _dataLink.ExecuteReader(storedProcedure, parameters);
            // Assert
            Assert.That(dt.Columns.Contains("game_id"), Is.True);
            Assert.That(dt.Rows.Count, Is.EqualTo(1));
        }
        
        [Test]
        public void Dispose_CalledMultipleTimes_DoesNotThrowException()
        {
            // Since DataLink is a singleton, calling Dispose multiple times should be safe.
            using (var dl = DataLink.Instance)
            {
                // First dispose happens at the end of this using block.
            }
            // Second dispose should not throw.
            Assert.That(() => DataLink.Instance.Dispose(), Throws.Nothing);
        }
    }
}
