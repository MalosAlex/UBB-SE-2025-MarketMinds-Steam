// Copyright (c) MarketMinds. All rights reserved.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Threading.Tasks;
using BusinessLayer.Repositories;
using BusinessLayer.Services;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Models;
using BusinessLayer.Data;
using Moq;
using NUnit.Framework;
using System.Reflection;

namespace Tests.ServiceTests
{
    /// <summary>
    /// Tests for the PasswordResetService class.
    /// </summary>
    [TestFixture]
    public class PasswordResetServiceTests
    {
        private PasswordResetService service;
        private Mock<PasswordResetRepository> mockRepository;
        private Mock<IUserService> mockUserService;
        private string testResetCodesPath;

        [SetUp]
        public void Setup()
        {
            this.mockRepository = new Mock<PasswordResetRepository>(new Mock<IDataLink>().Object);
            this.mockUserService = new Mock<IUserService>();
            this.service = new PasswordResetService(this.mockRepository.Object, this.mockUserService.Object);

            // Set up test directory for reset codes
            this.testResetCodesPath = Path.Combine(Path.GetTempPath(), "TestResetCodes");
            if (Directory.Exists(this.testResetCodesPath))
            {
                Directory.Delete(this.testResetCodesPath, true);
            }
            Directory.CreateDirectory(this.testResetCodesPath);

            // Use reflection to set the private resetCodesPath field
            typeof(PasswordResetService)
                .GetField("resetCodesPath", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(this.service, this.testResetCodesPath);
        }

        [TearDown]
        public void Cleanup()
        {
            try
            {
                if (Directory.Exists(this.testResetCodesPath))
                {
                    Directory.Delete(this.testResetCodesPath, true);
                }
            }
            catch (IOException)
            {
                // Ignore directory deletion errors during test cleanup
            }
        }

        [Test]
        public async Task SendResetCode_WithValidEmail_ReturnsSuccessResult()
        {
            // Arrange
            string email = "test@example.com";
            var user = new User { UserId = 1, Email = email, Username = "testuser" };
            mockUserService.Setup(s => s.GetUserByEmail(email)).Returns(user);

            // Act
            var result = await service.SendResetCode(email);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.isValid, Is.True);
                Assert.That(result.message, Is.EqualTo("Reset code sent successfully."));
            });
            
            // Check that a file was created
            string filePath = Path.Combine(testResetCodesPath, $"{email.ToLower()}_reset_code.txt");
            Assert.That(File.Exists(filePath), Is.True);
            
            // Check file content is a 6-digit code
            string code = File.ReadAllText(filePath);
            Assert.That(System.Text.RegularExpressions.Regex.IsMatch(code, @"^\d{6}$"), Is.True);
        }

        [Test]
        public void SendResetCode_WithInvalidEmail_ThrowsInvalidOperationException()
        {
            // Arrange
            string invalidEmail = "invalid";

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => service.SendResetCode(invalidEmail).GetAwaiter().GetResult());
            Assert.That(ex.Message, Is.EqualTo("Invalid email format."));
        }

        [Test]
        public async Task SendResetCode_UnregisteredEmail_ReturnsFalseResult()
        {
            // Arrange
            string email = "nonexistent@example.com";
            mockUserService.Setup(s => s.GetUserByEmail(email)).Returns((User)null);

            // Act
            var result = await service.SendResetCode(email);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.isValid, Is.False);
                Assert.That(result.message, Is.EqualTo("Email is not registered."));
            });
        }

        [Test]
        public void VerifyResetCode_WithValidCodeAndEmail_ReturnsSuccessResult()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string filePath = Path.Combine(testResetCodesPath, $"{email.ToLower()}_reset_code.txt");
            File.WriteAllText(filePath, code);

            // Act
            var result = service.VerifyResetCode(email, code);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.isValid, Is.True);
                Assert.That(result.message, Is.EqualTo("Code verified successfully."));
            });
        }

        [Test]
        public void VerifyResetCode_WithInvalidEmail_ThrowsInvalidOperationException()
        {
            // Arrange
            string invalidEmail = "invalid";
            string code = "123456";

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => service.VerifyResetCode(invalidEmail, code));
            Assert.That(ex.Message, Is.EqualTo("Invalid email format."));
        }

        [Test]
        public void VerifyResetCode_WithInvalidCode_ThrowsInvalidOperationException()
        {
            // Arrange
            string email = "test@example.com";
            string invalidCode = "12345"; // 5 digits instead of 6

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => service.VerifyResetCode(email, invalidCode));
            Assert.That(ex.Message, Is.EqualTo("Reset code must be a 6-digit number."));
        }

        [Test]
        public void VerifyResetCode_WithIncorrectCode_ReturnsFalseResult()
        {
            // Arrange
            string email = "test@example.com";
            string storedCode = "123456";
            string attemptedCode = "654321";
            string filePath = Path.Combine(testResetCodesPath, $"{email.ToLower()}_reset_code.txt");
            File.WriteAllText(filePath, storedCode);

            // Act
            var result = service.VerifyResetCode(email, attemptedCode);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.isValid, Is.False);
                Assert.That(result.message, Is.EqualTo("Invalid reset code."));
            });
        }

        [Test]
        public void ResetPassword_WithValidParameters_ReturnsSuccessResult()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string newPassword = "NewPassword123!";
            var user = new User { UserId = 1, Email = email, Username = "testuser" };
            string filePath = Path.Combine(testResetCodesPath, $"{email.ToLower()}_reset_code.txt");
            File.WriteAllText(filePath, code);

            mockUserService.Setup(s => s.GetUserByEmail(email)).Returns(user);
            mockUserService.Setup(s => s.UpdateUserPassword(user.UserId, newPassword));

            // Act
            var result = service.ResetPassword(email, code, newPassword);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.isValid, Is.True);
                Assert.That(result.message, Is.EqualTo("Password reset successfully."));
            });
            mockUserService.Verify(s => s.UpdateUserPassword(user.UserId, newPassword), Times.Once);
            Assert.That(File.Exists(filePath), Is.False);
        }

        [Test]
        public void ResetPassword_WithInvalidCode_ReturnsFalseResult()
        {
            // Arrange
            string email = "test@example.com";
            string storedCode = "123456";
            string attemptedCode = "654321";
            string newPassword = "NewPassword123!";
            string filePath = Path.Combine(testResetCodesPath, $"{email.ToLower()}_reset_code.txt");
            File.WriteAllText(filePath, storedCode);

            // Act
            var result = service.ResetPassword(email, attemptedCode, newPassword);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.isValid, Is.False);
                Assert.That(result.message, Is.EqualTo("Invalid reset code."));
                Assert.That(File.Exists(filePath), Is.True);
            });
        }

        [Test]
        public void ResetPassword_WithInvalidPassword_ThrowsInvalidOperationException()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string invalidPassword = "weak"; // Too short and missing requirements
            string filePath = Path.Combine(testResetCodesPath, $"{email.ToLower()}_reset_code.txt");
            File.WriteAllText(filePath, code);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => service.ResetPassword(email, code, invalidPassword));
            Assert.That(ex.Message, Is.EqualTo("Password must be at least 8 characters long."));
        }

        [Test]
        public void ResetPassword_WithNonExistentUser_ReturnsFalseResult()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string newPassword = "NewPassword123!";
            string filePath = Path.Combine(testResetCodesPath, $"{email.ToLower()}_reset_code.txt");
            File.WriteAllText(filePath, code);

            mockUserService.Setup(s => s.GetUserByEmail(email)).Returns((User)null);

            // Act
            var result = service.ResetPassword(email, code, newPassword);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.isValid, Is.False);
                Assert.That(result.message, Is.EqualTo("User not found."));
                Assert.That(File.Exists(filePath), Is.True);
            });
        }

        [Test]
        public void CleanupExpiredCodes_RemovesExpiredFiles()
        {
            // Arrange
            string oldEmail = "old@example.com";
            string newEmail = "new@example.com";
            string oldFilePath = Path.Combine(this.testResetCodesPath, $"{oldEmail.ToLower()}_reset_code.txt");
            string newFilePath = Path.Combine(this.testResetCodesPath, $"{newEmail.ToLower()}_reset_code.txt");

            // Create test files
            File.WriteAllText(oldFilePath, "123456");
            File.WriteAllText(newFilePath, "654321");

            // Set creation times - old file created 16 minutes ago, new file created 14 minutes ago
            File.SetCreationTimeUtc(oldFilePath, DateTime.UtcNow.AddMinutes(-16));
            File.SetCreationTimeUtc(newFilePath, DateTime.UtcNow.AddMinutes(-14));

            // Act
            this.service.CleanupExpiredCodes();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(File.Exists(oldFilePath), Is.False, "Old file should be deleted");
                Assert.That(File.Exists(newFilePath), Is.True, "New file should still exist");
            });
        }
        
        [Test]
        public void CleanupExpiredCodes_HandlesNonExistentDirectory()
        {
            // Arrange
            if (Directory.Exists(testResetCodesPath))
            {
                Directory.Delete(testResetCodesPath, true);
            }
            
            // Use reflection to set a non-existent path
            string nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            typeof(PasswordResetService)
                .GetField("resetCodesPath", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(service, nonExistentPath);
                
            // Act
            service.CleanupExpiredCodes();
            
            // Assert
            Assert.That(Directory.Exists(nonExistentPath), Is.False);
        }

        [Test]
        public void VerifyResetCode_WithNonExistentCode_ReturnsFalseResult()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";

            // Act
            var result = service.VerifyResetCode(email, code);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.isValid, Is.False);
                Assert.That(result.message, Is.EqualTo("Reset code has expired or does not exist."));
            });
        }
    }
} 