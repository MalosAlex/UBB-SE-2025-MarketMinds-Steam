using System;
using System.Reflection;
using System.Threading.Tasks;
using BusinessLayer.Models;
using BusinessLayer.Repositories.Fakes;
using BusinessLayer.Services.Fakes;
using NUnit.Framework;

namespace Tests.ServiceTests
{
    [TestFixture]
    public class FakePasswordResetServiceTests
    {
        private FakePasswordResetService service;

        [SetUp]
        public void Setup()
        {
            service = new FakePasswordResetService();
        }

        [Test]
        public void Constructor_InitializesWithFakeDependencies()
        {
            // Assert
            Assert.That(service, Is.Not.Null);
            
            // Verify private fields using reflection
            var repositoryField = typeof(FakePasswordResetService).GetField("passwordResetRepository", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            var userServiceField = typeof(FakePasswordResetService).GetField("userService", 
                BindingFlags.NonPublic | BindingFlags.Instance);
                
            var repository = repositoryField.GetValue(service);
            var userService = userServiceField.GetValue(service);
            
            Assert.That(repository, Is.Not.Null);
            Assert.That(userService, Is.Not.Null);
            Assert.That(repository, Is.InstanceOf<FakePasswordResetRepository>());
            Assert.That(userService, Is.InstanceOf<FakeUserService>());
        }

        [Test]
        public async Task SendResetCode_WithExistingUser_ReturnsSuccessResult()
        {
            // Arrange
            string email = "user1@example.com"; // Exists in FakeUserService

            // Act
            var result = await service.SendResetCode(email);

            // Assert
            Assert.That(result.isValid, Is.True);
            Assert.That(result.message, Is.EqualTo("Reset code sent successfully"));
        }

        [Test]
        public async Task SendResetCode_WithNonExistentUser_ReturnsErrorResult()
        {
            // Arrange
            string email = "nonexistent@example.com";

            // Act
            var result = await service.SendResetCode(email);

            // Assert
            Assert.That(result.isValid, Is.False);
            Assert.That(result.message, Is.EqualTo("User not found"));
        }

        [Test]
        public void VerifyResetCode_WithCorrectCode_ReturnsSuccessResult()
        {
            // Arrange
            string email = "user1@example.com";
            string code = "123456";
            int userId = 1;
            DateTime expiryTime = DateTime.Now.AddMinutes(30);
            
            // Store a reset code first
            var repository = GetPrivateRepositoryField();
            repository.StoreResetCode(userId, code, expiryTime);

            // Act
            var result = service.VerifyResetCode(email, code);

            // Assert
            Assert.That(result.isValid, Is.True);
            Assert.That(result.message, Is.EqualTo("Code verified successfully"));
        }

        [Test]
        public void VerifyResetCode_WithIncorrectCode_ReturnsErrorResult()
        {
            // Arrange
            string email = "user1@example.com";
            string validCode = "123456";
            string invalidCode = "654321";
            int userId = 1;
            DateTime expiryTime = DateTime.Now.AddMinutes(30);
            
            // Store a reset code first
            var repository = GetPrivateRepositoryField();
            repository.StoreResetCode(userId, validCode, expiryTime);

            // Act
            var result = service.VerifyResetCode(email, invalidCode);

            // Assert
            Assert.That(result.isValid, Is.False);
            Assert.That(result.message, Is.EqualTo("Invalid or expired code"));
        }

        [Test]
        public void VerifyResetCode_WithNonExistentEmail_ReturnsErrorResult()
        {
            // Arrange
            string email = "nonexistent@example.com";
            string code = "123456";

            // Act
            var result = service.VerifyResetCode(email, code);

            // Assert
            Assert.That(result.isValid, Is.False);
            Assert.That(result.message, Is.EqualTo("Invalid or expired code"));
        }

        [Test]
        public void ResetPassword_WithValidCodeAndEmail_ReturnsSuccessResult()
        {
            // Arrange
            string email = "user1@example.com";
            string code = "123456";
            string newPassword = "NewPassword123!";
            int userId = 1;
            DateTime expiryTime = DateTime.Now.AddMinutes(30);
            
            // Store a reset code first
            var repository = GetPrivateRepositoryField();
            repository.StoreResetCode(userId, code, expiryTime);

            // Act
            var result = service.ResetPassword(email, code, newPassword);

            // Assert
            Assert.That(result.isValid, Is.True);
            Assert.That(result.message, Is.EqualTo("Password reset successfully"));
        }

        [Test]
        public void ResetPassword_WithInvalidCode_ReturnsErrorResult()
        {
            // Arrange
            string email = "user1@example.com";
            string validCode = "123456";
            string invalidCode = "654321";
            string newPassword = "NewPassword123!";
            int userId = 1;
            DateTime expiryTime = DateTime.Now.AddMinutes(30);
            
            // Store a reset code first
            var repository = GetPrivateRepositoryField();
            repository.StoreResetCode(userId, validCode, expiryTime);

            // Act
            var result = service.ResetPassword(email, invalidCode, newPassword);

            // Assert
            Assert.That(result.isValid, Is.False);
            Assert.That(result.message, Is.EqualTo("Invalid or expired code"));
        }

        [Test]
        public void ResetPassword_WithNonExistentUser_ReturnsErrorResult()
        {
            // Arrange
            string email = "nonexistent@example.com";
            string code = "123456";
            string newPassword = "NewPassword123!";

            // Act
            var result = service.ResetPassword(email, code, newPassword);

            // Assert
            Assert.That(result.isValid, Is.False);
            Assert.That(result.message, Is.EqualTo("Invalid or expired code"));
        }

        [Test]
        public void CleanupExpiredCodes_CallsRepositoryMethod()
        {
            // Arrange
            var repository = GetPrivateRepositoryField();
            
            // Store some expired codes
            repository.StoreResetCode(1, "123456", DateTime.Now.AddMinutes(-10));
            
            // Act - note we can't directly test if it was called, but we can test the behavior
            service.CleanupExpiredCodes();
            
            // Assert - verify code can't be verified after cleanup
            Assert.That(repository.VerifyResetCode("user1@example.com", "123456"), Is.False);
        }

        [Test]
        public void GenerateResetCode_ReturnsValidSixDigitCode()
        {
            // Arrange
            var method = typeof(FakePasswordResetService).GetMethod("GenerateResetCode", 
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var code = method.Invoke(service, null) as string;

            // Assert
            Assert.That(code, Is.Not.Null);
            Assert.That(code.Length, Is.EqualTo(6));
            Assert.That(int.TryParse(code, out _), Is.True);
        }

        // Helper method to get access to the private repository field
        private FakePasswordResetRepository GetPrivateRepositoryField()
        {
            var field = typeof(FakePasswordResetService).GetField("passwordResetRepository", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            return (FakePasswordResetRepository)field.GetValue(service);
        }
    }
} 