using System.Collections.Generic;
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

namespace Tests.RepositoryTests
{
    [TestFixture]
    public class PasswordResetRepositoryTests
    {
        private Mock<IDataLink> mockDataLink;
        private PasswordResetRepository repository;

        [SetUp]
        public void Setup()
        {
            this.mockDataLink = new Mock<IDataLink>();
            this.repository = new PasswordResetRepository(this.mockDataLink.Object);
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
        public void StoreResetCode_DeleteExistingResetCodesWasCalled()
        {
            // Arrange
            int userId = 1;
            string code = "123456";
            DateTime expiryTime = DateTime.Now.AddMinutes(30);
            string deleteProc = "DeleteExistingResetCodes";

            this.mockDataLink.Setup(dl => dl.ExecuteNonQuery(deleteProc, It.Is<SqlParameter[]>(p => p.Length == 1 && (int)p[0].Value == userId)))
                .Verifiable();

            this.mockDataLink.Setup(dl => dl.ExecuteNonQuery("StorePasswordResetCode", It.Is<SqlParameter[]>(p => p.Length == 3)))
                .Verifiable();

            // Act
            this.repository.StoreResetCode(userId, code, expiryTime);

            // Assert
            bool deleteProcCalled = false;
            foreach (var invocation in this.mockDataLink.Invocations)
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

            Assert.That(deleteProcCalled, Is.True, "DeleteExistingResetCodes was not called");
        }

        [Test]
        public void StoreResetCode_StorePasswordResetCodeWasCalled()
        {
            // Arrange
            int userId = 1;
            string code = "123456";
            DateTime expiryTime = DateTime.Now.AddMinutes(30);
            string storeProc = "StorePasswordResetCode";

            this.mockDataLink.Setup(dl => dl.ExecuteNonQuery("DeleteExistingResetCodes", It.Is<SqlParameter[]>(p => p.Length == 1)))
                .Verifiable();

            this.mockDataLink.Setup(dl => dl.ExecuteNonQuery(storeProc, It.Is<SqlParameter[]>(p => p.Length == 3)))
                .Verifiable();

            // Act
            this.repository.StoreResetCode(userId, code, expiryTime);

            // Assert
            bool storeProcCalled = false;
            foreach (var invocation in this.mockDataLink.Invocations)
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

            this.mockDataLink.Setup(dl => dl.ExecuteNonQuery(deleteProc, It.Is<SqlParameter[]>(p => p.Length == 1)))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            try
            {
                this.repository.StoreResetCode(userId, code, expiryTime);
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
            var dataTable = new DataTable();
            dataTable.Columns.Add("expiration_time", typeof(DateTime));
            dataTable.Columns.Add("used", typeof(bool));
            dataTable.Rows.Add(DateTime.UtcNow.AddMinutes(10), false); // Valid code

            this.mockDataLink.Setup(dl => dl.ExecuteReader(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p =>
                    p.Length == 2 &&
                    (string)p[0].Value == email &&
                    (string)p[1].Value == code))).Returns(dataTable);

            // Act
            bool result = this.repository.VerifyResetCode(email, code);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void VerifyResetCode_WithInvalidCode_ReturnsFalse()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            var dataTable = new DataTable();
            dataTable.Columns.Add("expiration_time", typeof(DateTime));
            dataTable.Columns.Add("used", typeof(bool));
            dataTable.Rows.Add(DateTime.UtcNow.AddMinutes(-10), false); // Expired code

            this.mockDataLink.Setup(dl => dl.ExecuteReader(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p =>
                    p.Length == 2 &&
                    (string)p[0].Value == email &&
                    (string)p[1].Value == code))).Returns(dataTable);

            // Act
            bool result = this.repository.VerifyResetCode(email, code);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void VerifyResetCode_WithUsedCode_ReturnsFalse()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            var dataTable = new DataTable();
            dataTable.Columns.Add("expiration_time", typeof(DateTime));
            dataTable.Columns.Add("used", typeof(bool));
            dataTable.Rows.Add(DateTime.UtcNow.AddMinutes(10), true); // Used code

            this.mockDataLink.Setup(dl => dl.ExecuteReader(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p =>
                    p.Length == 2 &&
                    (string)p[0].Value == email &&
                    (string)p[1].Value == code))).Returns(dataTable);

            // Act
            bool result = this.repository.VerifyResetCode(email, code);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void VerifyResetCode_WithNoCode_ReturnsFalse()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            var dataTable = new DataTable();
            dataTable.Columns.Add("expiration_time", typeof(DateTime));
            dataTable.Columns.Add("used", typeof(bool));

            // No rows added
            this.mockDataLink.Setup(dl => dl.ExecuteReader(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p =>
                    p.Length == 2 &&
                    (string)p[0].Value == email &&
                    (string)p[1].Value == code))).Returns(dataTable);

            // Act
            bool result = this.repository.VerifyResetCode(email, code);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void VerifyResetCode_WhenDatabaseOperationExceptionOccurs_ThrowsRepositoryException()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";

            this.mockDataLink.Setup(dl => dl.ExecuteReader(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p => p.Length == 2))).Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            try
            {
                this.repository.VerifyResetCode(email, code);
                Assert.Fail("Expected RepositoryException was not thrown");
            }
            catch (RepositoryException)
            {
                // Expected
                Assert.Pass();
            }
        }

        [Test]
        public void ResetPassword_WithValidCodeAndEmail_UpdatesPassword()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string newPassword = "NewPassword123!";
            var dataTable = new DataTable();
            dataTable.Columns.Add("user_id", typeof(int));
            dataTable.Rows.Add(1); // Valid user

            this.mockDataLink.Setup(dl => dl.ExecuteReader(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p => p.Length == 2))).Returns(dataTable);

            this.mockDataLink.Setup(dl => dl.ExecuteNonQuery(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p => p.Length == 2))).Returns(1);

            // Act
            bool result = this.repository.ResetPassword(email, code, newPassword);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void ResetPassword_WithInvalidCode_ReturnsFalse()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string newPassword = "NewPassword123!";
            var dataTable = new DataTable();
            dataTable.Columns.Add("user_id", typeof(int));

            // No rows added - invalid code
            this.mockDataLink.Setup(dl => dl.ExecuteReader(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p => p.Length == 2))).Returns(dataTable);

            // Act
            bool result = this.repository.ResetPassword(email, code, newPassword);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ResetPassword_WithNonExistentUser_ReturnsFalse()
        {
            // Arrange
            string email = "nonexistent@example.com";
            string code = "123456";
            string newPassword = "NewPassword123!";
            var dataTable = new DataTable();
            dataTable.Columns.Add("user_id", typeof(int));

            // No rows added - non-existent user
            this.mockDataLink.Setup(dl => dl.ExecuteReader(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p => p.Length == 2))).Returns(dataTable);

            // Act
            bool result = this.repository.ResetPassword(email, code, newPassword);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ResetPassword_WhenDatabaseOperationExceptionOccurs_ThrowsRepositoryException()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string newPassword = "NewPassword123!";

            this.mockDataLink.Setup(dl => dl.ExecuteReader(
                It.IsAny<string>(),
                It.Is<SqlParameter[]>(p => p.Length == 2))).Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            try
            {
                this.repository.ResetPassword(email, code, newPassword);
                Assert.Fail("Expected RepositoryException was not thrown");
            }
            catch (RepositoryException)
            {
                // Expected
                Assert.Pass();
            }
        }

        [Test]
        public void CleanupExpiredCodes_ExecutesCorrectProcedure()
        {
            // Arrange
            string procedureName = "CleanupExpiredResetCodes";

            this.mockDataLink.Setup(dl => dl.ExecuteNonQuery(procedureName, It.Is<SqlParameter[]>(p => p.Length == 0)))
                .Verifiable();

            // Act
            this.repository.CleanupExpiredCodes();

            // Assert
            bool procedureCalled = false;
            foreach (var invocation in this.mockDataLink.Invocations)
            {
                if (invocation.Method.Name == "ExecuteNonQuery")
                {
                    if ((string)invocation.Arguments[0] == procedureName)
                    {
                        procedureCalled = true;
                        break;
                    }
                }
            }

            Assert.That(procedureCalled, Is.True, "CleanupExpiredResetCodes was not called.");
        }

        [Test]
        public void CleanupExpiredCodes_WhenDatabaseOperationExceptionOccurs_ThrowsRepositoryException()
        {
            // Arrange
            string procedureName = "CleanupExpiredResetCodes";

            this.mockDataLink.Setup(dl => dl.ExecuteNonQuery(procedureName, It.Is<SqlParameter[]>(p => p.Length == 0)))
                .Throws(new DatabaseOperationException("Database error"));

            // Act & Assert
            try
            {
                this.repository.CleanupExpiredCodes();
                Assert.Fail("Expected RepositoryException was not thrown.");
            }
            catch (RepositoryException)
            {
                // Expected
                Assert.Pass();
            }
        }
    }
}