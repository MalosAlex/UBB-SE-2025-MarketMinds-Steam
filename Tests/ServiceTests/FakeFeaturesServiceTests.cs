using System.Collections.Generic;
using System.Linq;
using BusinessLayer.Models;
using BusinessLayer.Repositories.Fakes;
using BusinessLayer.Services.Fakes;
using NUnit.Framework;

namespace Tests.ServiceTests
{
    [TestFixture]
    public class FakeFeaturesServiceTests
    {
        private FakeFeaturesService service;

        [SetUp]
        public void Setup()
        {
            service = new FakeFeaturesService();
        }

        [Test]
        public void Constructor_InitializesServiceWithFakeDependencies()
        {
            // Assert
            Assert.That(service, Is.Not.Null);
            Assert.That(service.UserService, Is.Not.Null);
            Assert.That(service.UserService, Is.InstanceOf<FakeUserService>());
        }

        [Test]
        public void GetFeaturesByCategories_GroupsFeaturesByType()
        {
            // Act
            var categories = service.GetFeaturesByCategories();

            // Assert
            Assert.That(categories, Is.Not.Null);
            Assert.That(categories.Count, Is.EqualTo(2));
            Assert.That(categories.ContainsKey("frame"), Is.True);
            Assert.That(categories.ContainsKey("emoji"), Is.True);
            Assert.That(categories["frame"].Count, Is.EqualTo(2));
            Assert.That(categories["emoji"].Count, Is.EqualTo(2));
        }

        [Test]
        public void EquipFeature_WithNonPurchasedFeature_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            int featureId = 2; // Silver Frame (not purchased)

            // Act
            bool result = service.EquipFeature(userId, featureId);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void EquipFeature_WithPurchasedFeature_ReturnsTrue()
        {
            // Arrange
            int userId = 1;
            int featureId = 1; // Gold Frame (purchased)

            // Act
            bool result = service.EquipFeature(userId, featureId);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void EquipFeature_WithFrameType_UnequipsOtherFrames()
        {
            // Arrange
            int userId = 1;
            int frameFeatureId = 1; // Gold Frame

            // Ensure it's equipped first
            service.EquipFeature(userId, frameFeatureId);
            
            // Now buy and equip another frame
            // We need to directly access the repository to add the new frame
            var featuresRepository = GetPrivateRepositoryField();
            featuresRepository.PurchaseFeature(userId, 2); // Silver Frame
            
            // Act
            bool result = service.EquipFeature(userId, 2); // Equip Silver Frame
            
            // Get user features to verify state
            var userFeatures = featuresRepository.GetUserFeatures(userId);
            var goldFrame = userFeatures.FirstOrDefault(f => f.FeatureId == 1);
            var silverFrame = userFeatures.FirstOrDefault(f => f.FeatureId == 2);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(goldFrame, Is.Not.Null);
            Assert.That(silverFrame, Is.Not.Null);
            Assert.That(goldFrame.Equipped, Is.False, "Gold Frame should be unequipped");
            Assert.That(silverFrame.Equipped, Is.True, "Silver Frame should be equipped");
        }

        [Test]
        public void UnequipFeature_WithNonPurchasedFeature_ReturnsFalseWithMessage()
        {
            // Arrange
            int userId = 1;
            int featureId = 2; // Silver Frame (not purchased)

            // Act
            var result = service.UnequipFeature(userId, featureId);

            // Assert
            Assert.That(result.Item1, Is.False);
            Assert.That(result.Item2, Is.EqualTo("Feature not purchased"));
        }

        [Test]
        public void UnequipFeature_WithPurchasedFeature_ReturnsTrueWithMessage()
        {
            // Arrange
            int userId = 1;
            int featureId = 1; // Gold Frame (purchased)
            service.EquipFeature(userId, featureId); // Make sure it's equipped

            // Act
            var result = service.UnequipFeature(userId, featureId);

            // Assert
            Assert.That(result.Item1, Is.True);
            Assert.That(result.Item2, Is.EqualTo("Feature unequipped successfully"));
        }

        [Test]
        public void GetUserEquippedFeatures_ReturnsOnlyEquippedFeatures()
        {
            // Arrange
            int userId = 1;
            service.EquipFeature(userId, 1); // Equip Gold Frame
            service.UnequipFeature(userId, 3); // Unequip Happy Emoji

            // Act
            var equippedFeatures = service.GetUserEquippedFeatures(userId);

            // Assert
            Assert.That(equippedFeatures, Is.Not.Null);
            Assert.That(equippedFeatures.Count, Is.EqualTo(1));
            Assert.That(equippedFeatures[0].FeatureId, Is.EqualTo(1)); // Only Gold Frame should be equipped
        }

        [Test]
        public void IsFeaturePurchased_WithPurchasedFeature_ReturnsTrue()
        {
            // Arrange
            int userId = 1;
            int featureId = 1; // Gold Frame (purchased)

            // Act
            bool result = service.IsFeaturePurchased(userId, featureId);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsFeaturePurchased_WithNonPurchasedFeature_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            int featureId = 2; // Silver Frame (not purchased)

            // Act
            bool result = service.IsFeaturePurchased(userId, featureId);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void GetUserFeatures_ReturnsCorrectFeatures()
        {
            // Arrange
            int userId = 1;

            // Act
            var features = service.GetUserFeatures(userId);

            // Assert
            Assert.That(features, Is.Not.Null);
            Assert.That(features.Count, Is.EqualTo(2));
            Assert.That(features.Any(f => f.Name == "Gold Frame"), Is.True);
            Assert.That(features.Any(f => f.Name == "Happy Emoji"), Is.True);
        }

        [Test]
        public void GetUserFeatures_WithNonExistentUser_ReturnsEmptyList()
        {
            // Arrange
            int userId = 999; // Non-existent user

            // Act
            var features = service.GetUserFeatures(userId);

            // Assert
            Assert.That(features, Is.Not.Null);
            Assert.That(features, Is.Empty);
        }

        // Helper method to get access to the private repository field
        private FakeFeaturesRepository GetPrivateRepositoryField()
        {
            var fieldInfo = typeof(FakeFeaturesService).GetField("featuresRepository", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return (FakeFeaturesRepository)fieldInfo.GetValue(service);
        }
    }
} 