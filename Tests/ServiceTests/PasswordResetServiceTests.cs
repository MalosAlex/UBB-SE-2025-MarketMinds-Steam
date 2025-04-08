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
            mockRepository = new Mock<PasswordResetRepository>(new Mock<IDataLink>().Object);
            mockUserService = new Mock<IUserService>();
            service = new PasswordResetService(mockRepository.Object, mockUserService.Object);

            // Set up test directory for reset codes
            testResetCodesPath = Path.Combine(Path.GetTempPath(), "TestResetCodes");
            Directory.CreateDirectory(testResetCodesPath);

            // Use reflection to set the private resetCodesPath field
            typeof(PasswordResetService)
                .GetField("resetCodesPath", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(service, testResetCodesPath);
        }

        [TearDown]
        public void Cleanup()
        {
            try
            {
                if (Directory.Exists(testResetCodesPath))
                {
                    Directory.Delete(testResetCodesPath, true);
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
            Assert.That(result.isValid, Is.True);
            Assert.That(result.message, Is.EqualTo("Reset code sent successfully."));
            
            // Check that a file was created
            string filePath = Path.Combine(testResetCodesPath, $"{email}.code");
            Assert.That(File.Exists(filePath), Is.True);
            
            // Check file content is a 6-digit code
            string code = await File.ReadAllTextAsync(filePath);
            Assert.That(System.Text.RegularExpressions.Regex.IsMatch(code, @"^\d{6}$"), Is.True);
        }

        [Test]
        public async Task SendResetCode_WithInvalidEmail_ReturnsErrorResult()
        {
            // Arrange
            string invalidEmail = "invalid";

            // Act
            var result = await service.SendResetCode(invalidEmail);

            // Assert
            Assert.That(result.isValid, Is.False);
            Assert.That(result.message, Is.EqualTo("Invalid email format."));
        }

        [Test]
        public async Task VerifyResetCode_WithValidCodeAndEmail_ReturnsSuccessResult()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string filePath = Path.Combine(testResetCodesPath, $"{email}.code");
            await File.WriteAllTextAsync(filePath, code);

            // Act
            var result = service.VerifyResetCode(email, code);

            // Assert
            Assert.That(result.isValid, Is.True);
            Assert.That(result.message, Is.EqualTo("Code verified successfully."));
        }

        [Test]
        public void VerifyResetCode_WithInvalidEmail_ReturnsErrorResult()
        {
            // Arrange
            string invalidEmail = "invalid";
            string code = "123456";

            // Act
            var result = service.VerifyResetCode(invalidEmail, code);

            // Assert
            Assert.That(result.isValid, Is.False);
            Assert.That(result.message, Is.EqualTo("Invalid email format."));
        }

        [Test]
        public void VerifyResetCode_WithInvalidCode_ReturnsErrorResult()
        {
            // Arrange
            string email = "test@example.com";
            string invalidCode = "12345"; // 5 digits instead of 6

            // Act
            var result = service.VerifyResetCode(email, invalidCode);

            // Assert
            Assert.That(result.isValid, Is.False);
            Assert.That(result.message, Is.EqualTo("Reset code must be a 6-digit number."));
        }

        [Test]
        public async Task VerifyResetCode_WithNonExistentCode_ReturnsErrorResult()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            // Don't create the code file

            // Act
            var result = service.VerifyResetCode(email, code);

            // Assert
            Assert.That(result.isValid, Is.False);
            Assert.That(result.message, Is.EqualTo("Reset code has expired or does not exist."));
        }

        [Test]
        public async Task VerifyResetCode_WithIncorrectCode_ReturnsErrorResult()
        {
            // Arrange
            string email = "test@example.com";
            string storedCode = "123456";
            string attemptedCode = "654321";
            string filePath = Path.Combine(testResetCodesPath, $"{email}.code");
            await File.WriteAllTextAsync(filePath, storedCode);

            // Act
            var result = service.VerifyResetCode(email, attemptedCode);

            // Assert
            Assert.That(result.isValid, Is.False);
            Assert.That(result.message, Is.EqualTo("Invalid reset code."));
        }

        [Test]
        public async Task ResetPassword_WithValidParameters_ReturnsSuccessResult()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string newPassword = "NewPassword123!";
            var user = new User { UserId = 1, Email = email, Username = "testuser" };
            string filePath = Path.Combine(testResetCodesPath, $"{email}.code");
            await File.WriteAllTextAsync(filePath, code);

            mockUserService.Setup(s => s.GetUserByEmail(email)).Returns(user);
            mockUserService.Setup(s => s.UpdateUserPassword(user.UserId, newPassword)).Verifiable();

            // Act
            var result = service.ResetPassword(email, code, newPassword);

            // Assert
            Assert.That(result.isValid, Is.True);
            Assert.That(result.message, Is.EqualTo("Password reset successfully."));
            mockUserService.Verify(s => s.UpdateUserPassword(user.UserId, newPassword), Times.Once);
            
            // Verify code file was deleted
            Assert.That(File.Exists(filePath), Is.False);
        }

        [Test]
        public async Task ResetPassword_WithInvalidCode_ReturnsErrorResult()
        {
            // Arrange
            string email = "test@example.com";
            string storedCode = "123456";
            string attemptedCode = "654321";
            string newPassword = "NewPassword123!";
            string filePath = Path.Combine(testResetCodesPath, $"{email}.code");
            await File.WriteAllTextAsync(filePath, storedCode);

            // Act
            var result = service.ResetPassword(email, attemptedCode, newPassword);

            // Assert
            Assert.That(result.isValid, Is.False);
            Assert.That(result.message, Is.EqualTo("Invalid reset code."));
            
            // Verify code file still exists
            Assert.That(File.Exists(filePath), Is.True);
        }

        [Test]
        public async Task ResetPassword_WithInvalidPassword_ReturnsErrorResult()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string invalidPassword = "weak"; // Too short and missing requirements
            string filePath = Path.Combine(testResetCodesPath, $"{email}.code");
            await File.WriteAllTextAsync(filePath, code);

            // Act
            var result = service.ResetPassword(email, code, invalidPassword);

            // Assert
            Assert.That(result.isValid, Is.False);
            Assert.That(result.message, Is.EqualTo("Password must be at least 8 characters long."));
            
            // Verify code file still exists
            Assert.That(File.Exists(filePath), Is.True);
        }

        [Test]
        public async Task ResetPassword_WithNonExistentUser_ReturnsErrorResult()
        {
            // Arrange
            string email = "test@example.com";
            string code = "123456";
            string newPassword = "NewPassword123!";
            string filePath = Path.Combine(testResetCodesPath, $"{email}.code");
            await File.WriteAllTextAsync(filePath, code);

            mockUserService.Setup(s => s.GetUserByEmail(email)).Returns((User)null);

            // Act
            var result = service.ResetPassword(email, code, newPassword);

            // Assert
            Assert.That(result.isValid, Is.False);
            Assert.That(result.message, Is.EqualTo("User not found."));
            
            // Verify code file still exists
            Assert.That(File.Exists(filePath), Is.True);
        }

        [Test]
        public void CleanupExpiredCodes_RemovesExpiredFiles()
        {
            // Arrange
            var now = DateTime.Now;
            
            // Create some files with creation dates
            var newCodePath = Path.Combine(testResetCodesPath, "new@example.com.code");
            var oldCodePath = Path.Combine(testResetCodesPath, "old@example.com.code");
            
            File.WriteAllText(newCodePath, "123456");
            File.WriteAllText(oldCodePath, "654321");
            
            // Manually set file creation time for the old file to be older than expiry threshold
            File.SetCreationTime(oldCodePath, now.AddHours(-25)); // 25 hours old (more than 1 day)
            File.SetCreationTime(newCodePath, now.AddMinutes(-30)); // 30 minutes old (less than 1 day)
            
            // Act
            service.CleanupExpiredCodes();
            
            // Assert
            Assert.That(File.Exists(newCodePath), Is.True, "New code should not be removed");
            Assert.That(File.Exists(oldCodePath), Is.False, "Old code should be removed");
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
                
            // Act & Assert (should not throw)
            Assert.That(() => service.CleanupExpiredCodes(), Throws.Nothing);
        }
    }
} 