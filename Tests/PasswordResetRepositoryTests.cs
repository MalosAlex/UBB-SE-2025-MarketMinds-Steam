using System;
using System.Data;
using System.Linq;
using BusinessLayer.Data;
using BusinessLayer.Exceptions;
using BusinessLayer.Repositories;
using BusinessLayer.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Moq;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class PasswordResetRepositoryTests
    {
        private Mock<IDataLink> mockDataLink;
        private PasswordResetRepository repository;

        [SetUp]
        public void Setup()
        {
            mockDataLink = new Mock<IDataLink>();
            repository = new PasswordResetRepository(mockDataLink.Object);
        }

        [Test]
        public void Constructor_WithNullDataLink_ThrowsArgumentNullException()
        {
            // Act & Assert
            try
            {
                new PasswordResetRepository(null);
                Assert.Fail("Expected ArgumentNullException was not thrown");
            }
            catch (ArgumentNullException)
            {
                // Expected
                Assert.Pass();
            }
        }

        [Test]
        public void StoreResetCode_CallsDeleteExistingResetCodesAndStorePasswordResetCode()
        {
            // Arrange
            int userId = 1;
            string code = "123456";
            DateTime expiryTime = DateTime.Now.AddMinutes(30);
            string deleteProc = "DeleteExistingResetCodes";
            string storeProc = "StorePasswordResetCode";

            mockDataLink.Setup(dl => dl.ExecuteNonQuery(deleteProc, It.Is<SqlParameter[]>(p => p.Length == 1 && (int)p[0].Value == userId)))
                .Verifiable();

            mockDataLink.Setup(dl => dl.ExecuteNonQuery(storeProc, It.Is<SqlParameter[]>(p => p.Length == 3 && 
                                      (int)p[0].Value == userId && 
                                      (string)p[1].Value == code && 
                                      (DateTime)p[2].Value == expiryTime)))
                .Verifiable();

            // Act
            repository.StoreResetCode(userId, code, expiryTime);

            // Assert
            mockDataLink.Verify();
            // Check if delete procedure was called
            bool deleteProcCalled = false;
            foreach (var invocation in mockDataLink.Invocations)
            {
                if (invocation.Method.Name == "ExecuteNonQuery")
                {
                    if ((string)invocation.Arguments[0] == deleteProc)
                    {
                        deleteProcCalled = true;
                        break;
                    }
                }
            }
            // Check if store procedure was called
            bool storeProcCalled = false;
            foreach (var invocation in mockDataLink.Invocations)
            {
                if (invocation.Method.Name == "ExecuteNonQuery")
                {
                    if ((string)invocation.Arguments[0] == storeProc)
                    {
                        storeProcCalled = true;
                        break;
                    }
                }
            }
            
            Assert.That(deleteProcCalled, Is.True, "DeleteExistingResetCodes was not called");
            Assert.That(storeProcCalled, Is.True, "StorePasswordResetCode was not called");
        }

        [Test]
        public void StoreResetCode_WhenDatabaseOperationExceptionOccurs_ThrowsRepositoryException()
        {
            // Arrange
            int userId = 1;
            string code = "123456";
            DateTime expiryTime = DateTime.Now.AddMinutes(30);
            string deleteProc = "DeleteExistingResetCodes";

            mockDataLink.Setup(dl => dl.ExecuteNonQuery(deleteProc, It.Is<SqlParameter[]>(p => p.Length == 1)))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            try
            {
                repository.StoreResetCode(userId, code, expiryTime);
                Assert.Fail("Expected RepositoryException was not thrown");
            }
            catch (RepositoryException)
            {
                // Expected
                Assert.Pass();
            }
        }

        [Test]
        public void VerifyResetCode_WithValidCode_ReturnsTrue()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string procName = "GetResetCodeData";
            
            var dataTable = new DataTable();
            dataTable.Columns.Add("expiration_time", typeof(DateTime));
            dataTable.Columns.Add("used", typeof(bool));
            dataTable.Rows.Add(DateTime.UtcNow.AddMinutes(10), false); // Not expired, not used

            mockDataLink.Setup(dl => dl.ExecuteReader(procName, It.Is<SqlParameter[]>(p => p.Length == 2 && 
                                      (string)p[0].Value == email && 
                                      (string)p[1].Value == code)))
                .Returns(dataTable);

            // Act
            bool result = repository.VerifyResetCode(email, code);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void VerifyResetCode_WithExpiredCode_ReturnsFalse()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string procName = "GetResetCodeData";
            
            var dataTable = new DataTable();
            dataTable.Columns.Add("expiration_time", typeof(DateTime));
            dataTable.Columns.Add("used", typeof(bool));
            dataTable.Rows.Add(DateTime.UtcNow.AddMinutes(-10), false); // Expired, not used

            mockDataLink.Setup(dl => dl.ExecuteReader(procName, It.Is<SqlParameter[]>(p => p.Length == 2)))
                .Returns(dataTable);

            // Act
            bool result = repository.VerifyResetCode(email, code);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void VerifyResetCode_WithUsedCode_ReturnsFalse()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string procName = "GetResetCodeData";
            
            var dataTable = new DataTable();
            dataTable.Columns.Add("expiration_time", typeof(DateTime));
            dataTable.Columns.Add("used", typeof(bool));
            dataTable.Rows.Add(DateTime.UtcNow.AddMinutes(10), true); // Not expired, but used

            mockDataLink.Setup(dl => dl.ExecuteReader(procName, It.Is<SqlParameter[]>(p => p.Length == 2)))
                .Returns(dataTable);

            // Act
            bool result = repository.VerifyResetCode(email, code);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void VerifyResetCode_WithNonExistentCode_ReturnsFalse()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string procName = "GetResetCodeData";
            
            var dataTable = new DataTable();
            dataTable.Columns.Add("expiration_time", typeof(DateTime));
            dataTable.Columns.Add("used", typeof(bool));
            // Empty table means code not found

            mockDataLink.Setup(dl => dl.ExecuteReader(procName, It.Is<SqlParameter[]>(p => p.Length == 2)))
                .Returns(dataTable);

            // Act
            bool result = repository.VerifyResetCode(email, code);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void VerifyResetCode_WhenDatabaseOperationExceptionOccurs_ThrowsRepositoryException()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string procName = "GetResetCodeData";

            mockDataLink.Setup(dl => dl.ExecuteReader(procName, It.Is<SqlParameter[]>(p => p.Length == 2)))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            try
            {
                repository.VerifyResetCode(email, code);
                Assert.Fail("Expected RepositoryException was not thrown");
            }
            catch (RepositoryException ex)
            {
                Assert.That(ex.Message, Is.EqualTo("Failed to verify reset code."));
            }
        }

        [Test]
        public void ResetPassword_WithValidCodeAndEmail_UpdatesPasswordAndReturnsTrue()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string hashedPassword = "hashedPassword123";
            string getCodeProc = "GetResetCodeData";
            string getUserProc = "GetUserByEmail";
            string updateProc = "UpdatePasswordAndRemoveResetCode";
            
            // Setup for code verification
            var resetCodeTable = new DataTable();
            resetCodeTable.Columns.Add("expiration_time", typeof(DateTime));
            resetCodeTable.Columns.Add("used", typeof(bool));
            resetCodeTable.Rows.Add(DateTime.UtcNow.AddMinutes(10), false); // Valid code
            
            // Setup for user lookup
            var userTable = new DataTable();
            userTable.Columns.Add("user_id", typeof(int));
            userTable.Rows.Add(1); // User ID 1

            mockDataLink.Setup(dl => dl.ExecuteReader(getCodeProc, It.Is<SqlParameter[]>(p => p.Length == 2)))
                .Returns(resetCodeTable);
                
            mockDataLink.Setup(dl => dl.ExecuteReader(getUserProc, It.Is<SqlParameter[]>(p => p.Length == 1 && 
                                      (string)p[0].Value == email)))
                .Returns(userTable);
                
            mockDataLink.Setup(dl => dl.ExecuteScalar<int>(updateProc, It.Is<SqlParameter[]>(p => p.Length == 3 && 
                                     (int)p[0].Value == 1 && 
                                     (string)p[1].Value == code && 
                                     (string)p[2].Value == hashedPassword)))
                .Returns(1); // 1 row affected

            // Act
            bool result = repository.ResetPassword(email, code, hashedPassword);

            // Assert
            Assert.That(result, Is.True);
            
            // Check if update procedure was called
            bool updateProcCalled = false;
            foreach (var invocation in mockDataLink.Invocations)
            {
                if (invocation.Method.Name == "ExecuteScalar")
                {
                    if ((string)invocation.Arguments[0] == updateProc)
                    {
                        updateProcCalled = true;
                        break;
                    }
                }
            }
            
            Assert.That(updateProcCalled, Is.True, "UpdatePasswordAndRemoveResetCode was not called");
        }

        [Test]
        public void ResetPassword_WithInvalidCode_ReturnsFalse()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string hashedPassword = "hashedPassword123";
            string getCodeProc = "GetResetCodeData";
            string getUserProc = "GetUserByEmail";
            string updateProc = "UpdatePasswordAndRemoveResetCode";
            
            // Setup for code verification - empty table means invalid code
            var resetCodeTable = new DataTable();
            resetCodeTable.Columns.Add("expiration_time", typeof(DateTime));
            resetCodeTable.Columns.Add("used", typeof(bool));

            mockDataLink.Setup(dl => dl.ExecuteReader(getCodeProc, It.Is<SqlParameter[]>(p => p.Length == 2)))
                .Returns(resetCodeTable);

            // Act
            bool result = repository.ResetPassword(email, code, hashedPassword);

            // Assert
            Assert.That(result, Is.False);
            
            // Check if getUserProc was called
            bool getUserProcCalled = false;
            foreach (var invocation in mockDataLink.Invocations)
            {
                if (invocation.Method.Name == "ExecuteReader")
                {
                    if ((string)invocation.Arguments[0] == getUserProc)
                    {
                        getUserProcCalled = true;
                        break;
                    }
                }
            }
            
            // Check if updateProc was called
            bool updateProcCalled = false;
            foreach (var invocation in mockDataLink.Invocations)
            {
                if (invocation.Method.Name == "ExecuteScalar")
                {
                    if ((string)invocation.Arguments[0] == updateProc)
                    {
                        updateProcCalled = true;
                        break;
                    }
                }
            }
            
            Assert.That(getUserProcCalled, Is.False, "GetUserByEmail should not have been called");
            Assert.That(updateProcCalled, Is.False, "UpdatePasswordAndRemoveResetCode should not have been called");
        }

        [Test]
        public void ResetPassword_WithNonExistentUser_ReturnsFalse()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string hashedPassword = "hashedPassword123";
            string getCodeProc = "GetResetCodeData";
            string getUserProc = "GetUserByEmail";
            string updateProc = "UpdatePasswordAndRemoveResetCode";
            
            // Setup for code verification
            var resetCodeTable = new DataTable();
            resetCodeTable.Columns.Add("expiration_time", typeof(DateTime));
            resetCodeTable.Columns.Add("used", typeof(bool));
            resetCodeTable.Rows.Add(DateTime.UtcNow.AddMinutes(10), false); // Valid code
            
            // Setup for user lookup - empty table means user not found
            var userTable = new DataTable();
            userTable.Columns.Add("user_id", typeof(int));

            mockDataLink.Setup(dl => dl.ExecuteReader(getCodeProc, It.Is<SqlParameter[]>(p => p.Length == 2)))
                .Returns(resetCodeTable);
                
            mockDataLink.Setup(dl => dl.ExecuteReader(getUserProc, It.Is<SqlParameter[]>(p => p.Length == 1)))
                .Returns(userTable);

            // Act
            bool result = repository.ResetPassword(email, code, hashedPassword);

            // Assert
            Assert.That(result, Is.False);
            
            // Check if updateProc was called
            bool updateProcCalled = false;
            foreach (var invocation in mockDataLink.Invocations)
            {
                if (invocation.Method.Name == "ExecuteScalar")
                {
                    if ((string)invocation.Arguments[0] == updateProc)
                    {
                        updateProcCalled = true;
                        break;
                    }
                }
            }
            
            Assert.That(updateProcCalled, Is.False, "UpdatePasswordAndRemoveResetCode should not have been called");
        }

        [Test]
        public void ResetPassword_WhenDatabaseOperationExceptionOccurs_ThrowsRepositoryException()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string hashedPassword = "hashedPassword123";
            string getCodeProc = "GetResetCodeData";

            mockDataLink.Setup(dl => dl.ExecuteReader(getCodeProc, It.Is<SqlParameter[]>(p => p.Length == 2)))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            try
            {
                repository.ResetPassword(email, code, hashedPassword);
                Assert.Fail("Expected RepositoryException was not thrown");
            }
            catch (RepositoryException ex)
            {
                Assert.That(ex.Message, Is.EqualTo("Failed to reset password."));
            }
        }

        [Test]
        public void CleanupExpiredCodes_ExecutesCorrectProcedure()
        {
            // Arrange
            string procedureName = "CleanupResetCodes";
            // Create a new mock without using lambdas with optional arguments
            var localMock = new Mock<IDataLink>();
            var localRepository = new PasswordResetRepository(localMock.Object);

            // Act
            localRepository.CleanupExpiredCodes();

            // Assert - use a direct approach without lambda expressions or LINQ
            // Check if the procedure was called using a foreach loop
            bool procedureCalled = false;
            foreach (var invocation in localMock.Invocations)
            {
                if (invocation.Method.Name == "ExecuteNonQuery")
                {
                    if (invocation.Arguments.Count > 0 && invocation.Arguments[0] is string procName)
                    {
                        if (procName == procedureName)
                        {
                            procedureCalled = true;
                            break;
                        }
                    }
                }
            }
            
            Assert.That(procedureCalled, Is.True, "CleanupResetCodes procedure was not called");
        }

        [Test]
        public void CleanupExpiredCodes_WhenDatabaseOperationExceptionOccurs_ThrowsRepositoryException()
        {
            // Create a simple field-based mock
            var mockDataLink = new Mock<IDataLink>();
            var localRepository = new PasswordResetRepository(mockDataLink.Object);

            // Act & Assert
            try
            {
                localRepository.CleanupExpiredCodes();
                Assert.Fail("Expected RepositoryException was not thrown");
            }
            catch (RepositoryException ex)
            {
                Assert.That(ex.Message, Is.EqualTo("Failed to cleanup expired reset codes."));
            }
        }
    }
} 