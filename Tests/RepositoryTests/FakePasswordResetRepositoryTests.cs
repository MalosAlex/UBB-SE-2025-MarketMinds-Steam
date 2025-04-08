using System;
using System.Reflection;
using BusinessLayer.Repositories.Fakes;
using NUnit.Framework;

namespace Tests.RepositoryTests
{
    [TestFixture]
    public class FakePasswordResetRepositoryTests
    {
        private FakePasswordResetRepository repository;

        [SetUp]
        public void Setup()
        {
            repository = new FakePasswordResetRepository();
        }

        [Test]
        public void StoreResetCode_WithValidUserId_StoresCodeSuccessfully()
        {
            // Arrange
            int userId = 1;
            string code = "123456";
            DateTime expiryTime = DateTime.Now.AddMinutes(30);

            // Act
            repository.StoreResetCode(userId, code, expiryTime);
            string email = GetUserEmail(userId);
            bool isVerified = repository.VerifyResetCode(email, code);

            // Assert
            Assert.That(isVerified, Is.True);
        }

        [Test]
        public void StoreResetCode_WithInvalidUserId_DoesNotStoreCode()
        {
            // Arrange
            int userId = 999; // Non-existent user
            string code = "123456";
            DateTime expiryTime = DateTime.Now.AddMinutes(30);

            // Act
            repository.StoreResetCode(userId, code, expiryTime);
            
            // Since we can't verify directly for a non-existent user, we'll check that it doesn't affect other users
            string email = GetUserEmail(1); // Existing user
            bool isVerified = repository.VerifyResetCode(email, code);

            // Assert
            Assert.That(isVerified, Is.False);
        }

        [Test]
        public void StoreResetCode_OverwritesExistingCode()
        {
            // Arrange
            int userId = 1;
            string firstCode = "111111";
            string secondCode = "222222";
            DateTime expiryTime = DateTime.Now.AddMinutes(30);

            // Act
            repository.StoreResetCode(userId, firstCode, expiryTime);
            repository.StoreResetCode(userId, secondCode, expiryTime);
            string email = GetUserEmail(userId);
            
            bool isFirstVerified = repository.VerifyResetCode(email, firstCode);
            bool isSecondVerified = repository.VerifyResetCode(email, secondCode);

            // Assert
            Assert.That(isFirstVerified, Is.False);
            Assert.That(isSecondVerified, Is.True);
        }

        [Test]
        public void VerifyResetCode_WithValidCode_ReturnsTrue()
        {
            // Arrange
            int userId = 1;
            string code = "123456";
            string email = GetUserEmail(userId);
            DateTime expiryTime = DateTime.Now.AddMinutes(30);
            repository.StoreResetCode(userId, code, expiryTime);

            // Act
            bool result = repository.VerifyResetCode(email, code);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void VerifyResetCode_WithInvalidCode_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            string validCode = "123456";
            string invalidCode = "654321";
            string email = GetUserEmail(userId);
            DateTime expiryTime = DateTime.Now.AddMinutes(30);
            repository.StoreResetCode(userId, validCode, expiryTime);

            // Act
            bool result = repository.VerifyResetCode(email, invalidCode);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void VerifyResetCode_WithExpiredCode_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            string code = "123456";
            string email = GetUserEmail(userId);
            DateTime expiryTime = DateTime.Now.AddMinutes(-5); // Expired 5 minutes ago
            repository.StoreResetCode(userId, code, expiryTime);

            // Act
            bool result = repository.VerifyResetCode(email, code);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void VerifyResetCode_WithNonExistentEmail_ReturnsFalse()
        {
            // Arrange
            string email = "nonexistent@example.com";
            string code = "123456";

            // Act
            bool result = repository.VerifyResetCode(email, code);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ResetPassword_WithValidCodeAndEmail_ReturnsTrue()
        {
            // Arrange
            int userId = 1;
            string code = "123456";
            string email = GetUserEmail(userId);
            string newPassword = "NewPassword123!";
            DateTime expiryTime = DateTime.Now.AddMinutes(30);
            repository.StoreResetCode(userId, code, expiryTime);

            // Act
            bool result = repository.ResetPassword(email, code, newPassword);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void ResetPassword_WithInvalidCode_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            string validCode = "123456";
            string invalidCode = "654321";
            string email = GetUserEmail(userId);
            string newPassword = "NewPassword123!";
            DateTime expiryTime = DateTime.Now.AddMinutes(30);
            repository.StoreResetCode(userId, validCode, expiryTime);

            // Act
            bool result = repository.ResetPassword(email, invalidCode, newPassword);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void ResetPassword_WithValidCode_DeletesResetCode()
        {
            // Arrange
            int userId = 1;
            string code = "123456";
            string email = GetUserEmail(userId);
            string newPassword = "NewPassword123!";
            DateTime expiryTime = DateTime.Now.AddMinutes(30);
            repository.StoreResetCode(userId, code, expiryTime);

            // Act
            bool resetResult = repository.ResetPassword(email, code, newPassword);
            bool verifyResult = repository.VerifyResetCode(email, code);

            // Assert
            Assert.That(resetResult, Is.True);
            Assert.That(verifyResult, Is.False, "Code should be deleted after password reset");
        }

        [Test]
        public void CleanupExpiredCodes_RemovesExpiredCodes()
        {
            // Arrange
            int userId = 1;
            string code = "123456";
            string email = GetUserEmail(userId);
            DateTime expiryTime = DateTime.Now.AddMinutes(-5); // Expired 5 minutes ago
            repository.StoreResetCode(userId, code, expiryTime);
            
            // Verify code exists before cleanup (even though it's expired)
            bool codeExistsBeforeCleanup = CodeExistsInRepository(email, code);
            Assert.That(codeExistsBeforeCleanup, Is.True, "Setup failed: Code should exist before cleanup");

            // Act
            repository.CleanupExpiredCodes();
            bool codeExistsAfterCleanup = CodeExistsInRepository(email, code);

            // Assert
            Assert.That(codeExistsAfterCleanup, Is.False, "Code should be removed after cleanup");
        }

        [Test]
        public void CleanupExpiredCodes_KeepsValidCodes()
        {
            // Arrange
            int userId = 1;
            string code = "123456";
            string email = GetUserEmail(userId);
            DateTime expiryTime = DateTime.Now.AddMinutes(30); // Valid for 30 more minutes
            repository.StoreResetCode(userId, code, expiryTime);

            // Act
            repository.CleanupExpiredCodes();
            bool isVerified = repository.VerifyResetCode(email, code);

            // Assert
            Assert.That(isVerified, Is.True, "Valid code should still be verifiable after cleanup");
        }

        // Helper method to get the email for a userId using reflection
        // since the repository uses a private dictionary for this mapping
        private string GetUserEmail(int userId)
        {
            // First, access the private field
            Type type = typeof(FakePasswordResetRepository);
            FieldInfo fieldInfo = type.GetField("userEmails", BindingFlags.NonPublic | BindingFlags.Instance);
            var userEmails = fieldInfo.GetValue(repository) as Dictionary<int, string>;
            
            // Then get the email from the dictionary if it exists
            if (userEmails.TryGetValue(userId, out string email))
            {
                return email;
            }
            
            return $"user{userId}@example.com"; // Default for testing if not found
        }

        // Helper method to check if a code exists in the repository
        private bool CodeExistsInRepository(string email, string code)
        {
            // Access the private resetCodes field using reflection to check if the code exists
            // regardless of its expiration status
            Type type = typeof(FakePasswordResetRepository);
            FieldInfo fieldInfo = type.GetField("resetCodes", BindingFlags.NonPublic | BindingFlags.Instance);
            var resetCodes = fieldInfo.GetValue(repository);
            
            // Get the type of the inner class ResetCode
            Type resetCodeType = type.GetNestedType("ResetCode", BindingFlags.NonPublic);
            
            // Use reflection to get the list's Count property
            int count = (int)resetCodes.GetType().GetProperty("Count").GetValue(resetCodes);
            
            // Iterate through the list using reflection
            for (int i = 0; i < count; i++)
            {
                // Get item at index i
                var item = resetCodes.GetType().GetProperty("Item").GetValue(resetCodes, new object[] { i });
                
                // Get Email and Code properties from the ResetCode object
                string codeEmail = (string)resetCodeType.GetProperty("Email").GetValue(item);
                string resetCodeValue = (string)resetCodeType.GetProperty("Code").GetValue(item);
                
                if (codeEmail == email && resetCodeValue == code)
                {
                    return true;
                }
            }
            
            return false;
        }
    }
} 