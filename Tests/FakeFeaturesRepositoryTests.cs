using System.Collections.Generic;
using System.Linq;
using BusinessLayer.Models;
using BusinessLayer.Repositories.Fakes;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class FakeFeaturesRepositoryTests
    {
        private FakeFeaturesRepository repository;

        [SetUp]
        public void Setup()
        {
            repository = new FakeFeaturesRepository();
        }

        [Test]
        public void GetAllFeatures_ReturnsAllFeatures()
        {
            // Act
            var features = repository.GetAllFeatures(1);

            // Assert
            Assert.That(features, Is.Not.Null);
            Assert.That(features.Count, Is.EqualTo(4));
            Assert.That(features.Any(f => f.Name == "Gold Frame"), Is.True);
            Assert.That(features.Any(f => f.Name == "Silver Frame"), Is.True);
            Assert.That(features.Any(f => f.Name == "Happy Emoji"), Is.True);
            Assert.That(features.Any(f => f.Name == "Sad Emoji"), Is.True);
        }

        [Test]
        public void GetFeaturesByType_WithFrameType_ReturnsFrameFeatures()
        {
            // Act
            var features = repository.GetFeaturesByType("frame");

            // Assert
            Assert.That(features, Is.Not.Null);
            Assert.That(features.Count, Is.EqualTo(2));
            Assert.That(features.All(f => f.Type == "frame"), Is.True);
            Assert.That(features.Any(f => f.Name == "Gold Frame"), Is.True);
            Assert.That(features.Any(f => f.Name == "Silver Frame"), Is.True);
        }

        [Test]
        public void GetFeaturesByType_WithEmojiType_ReturnsEmojiFeatures()
        {
            // Act
            var features = repository.GetFeaturesByType("emoji");

            // Assert
            Assert.That(features, Is.Not.Null);
            Assert.That(features.Count, Is.EqualTo(2));
            Assert.That(features.All(f => f.Type == "emoji"), Is.True);
            Assert.That(features.Any(f => f.Name == "Happy Emoji"), Is.True);
            Assert.That(features.Any(f => f.Name == "Sad Emoji"), Is.True);
        }

        [Test]
        public void GetFeaturesByType_WithNonExistentType_ReturnsEmptyList()
        {
            // Act
            var features = repository.GetFeaturesByType("nonexistent");

            // Assert
            Assert.That(features, Is.Not.Null);
            Assert.That(features, Is.Empty);
        }

        [Test]
        public void GetUserFeatures_ForExistingUser_ReturnsUserFeatures()
        {
            // Act
            var features = repository.GetUserFeatures(1);

            // Assert
            Assert.That(features, Is.Not.Null);
            Assert.That(features.Count, Is.EqualTo(2));
            Assert.That(features.Any(f => f.Name == "Gold Frame" && f.Equipped), Is.True);
            Assert.That(features.Any(f => f.Name == "Happy Emoji" && !f.Equipped), Is.True);
        }

        [Test]
        public void GetUserFeatures_ForNonExistingUser_ReturnsEmptyList()
        {
            // Act
            var features = repository.GetUserFeatures(999);

            // Assert
            Assert.That(features, Is.Not.Null);
            Assert.That(features, Is.Empty);
        }

        [Test]
        public void IsFeaturePurchased_WithPurchasedFeature_ReturnsTrue()
        {
            // Act
            bool isPurchased = repository.IsFeaturePurchased(1, 1); // Gold Frame for User 1

            // Assert
            Assert.That(isPurchased, Is.True);
        }

        [Test]
        public void IsFeaturePurchased_WithNonPurchasedFeature_ReturnsFalse()
        {
            // Act
            bool isPurchased = repository.IsFeaturePurchased(1, 2); // Silver Frame for User 1

            // Assert
            Assert.That(isPurchased, Is.False);
        }

        [Test]
        public void IsFeaturePurchased_WithNonExistingUser_ReturnsFalse()
        {
            // Act
            bool isPurchased = repository.IsFeaturePurchased(999, 1);

            // Assert
            Assert.That(isPurchased, Is.False);
        }

        [Test]
        public void EquipFeature_WithPurchasedFeature_ReturnsTrue()
        {
            // Arrange
            // Unequip first to make sure it's not already equipped
            repository.UnequipFeature(1, 3); // Happy Emoji

            // Act
            bool result = repository.EquipFeature(1, 3);
            var userFeatures = repository.GetUserFeatures(1);
            var happyEmoji = userFeatures.FirstOrDefault(f => f.FeatureId == 3);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(happyEmoji, Is.Not.Null);
            Assert.That(happyEmoji.Equipped, Is.True);
        }

        [Test]
        public void EquipFeature_WithNonPurchasedFeature_ReturnsFalse()
        {
            // Act
            bool result = repository.EquipFeature(1, 2); // Silver Frame (not purchased)

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void EquipFeature_WithNonExistingUser_ReturnsFalse()
        {
            // Act
            bool result = repository.EquipFeature(999, 1);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void UnequipFeature_WithEquippedFeature_ReturnsTrue()
        {
            // Arrange
            repository.EquipFeature(1, 1); // Gold Frame

            // Act
            bool result = repository.UnequipFeature(1, 1);
            var userFeatures = repository.GetUserFeatures(1);
            var goldFrame = userFeatures.FirstOrDefault(f => f.FeatureId == 1);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(goldFrame, Is.Not.Null);
            Assert.That(goldFrame.Equipped, Is.False);
        }

        [Test]
        public void UnequipFeature_WithNonPurchasedFeature_ReturnsFalse()
        {
            // Act
            bool result = repository.UnequipFeature(1, 2); // Silver Frame (not purchased)

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void UnequipFeature_WithNonExistingUser_ReturnsFalse()
        {
            // Act
            bool result = repository.UnequipFeature(999, 1);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void UnequipFeaturesByType_WithExistingTypeAndUser_UnequipsFeatures()
        {
            // Arrange
            repository.EquipFeature(1, 1); // Gold Frame (frame type)

            // Act
            bool result = repository.UnequipFeaturesByType(1, "frame");
            var userFeatures = repository.GetUserFeatures(1);
            var frameFeatures = userFeatures.Where(f => f.Type == "frame");

            // Assert
            Assert.That(result, Is.True);
            Assert.That(frameFeatures.All(f => !f.Equipped), Is.True);
        }

        [Test]
        public void PurchaseFeature_ForNewFeature_AddsFeatureToUserFeatures()
        {
            // Arrange
            // Verify user doesn't have Silver Frame
            Assert.That(repository.IsFeaturePurchased(1, 2), Is.False);

            // Act
            repository.PurchaseFeature(1, 2); // Silver Frame
            var userFeatures = repository.GetUserFeatures(1);

            // Assert
            Assert.That(repository.IsFeaturePurchased(1, 2), Is.True);
            Assert.That(userFeatures.Count, Is.EqualTo(3)); // Now user has 3 features
            Assert.That(userFeatures.Any(f => f.FeatureId == 2 && !f.Equipped), Is.True);
        }

        [Test]
        public void PurchaseFeature_ForAlreadyPurchasedFeature_DoesNotDuplicate()
        {
            // Arrange
            // Verify user has Gold Frame
            Assert.That(repository.IsFeaturePurchased(1, 1), Is.True);
            var initialFeatureCount = repository.GetUserFeatures(1).Count;

            // Act
            repository.PurchaseFeature(1, 1); // Gold Frame (already purchased)
            var finalFeatureCount = repository.GetUserFeatures(1).Count;

            // Assert
            Assert.That(finalFeatureCount, Is.EqualTo(initialFeatureCount));
        }

        [Test]
        public void PurchaseFeature_ForNonExistingFeature_DoesNothing()
        {
            // Arrange
            var initialFeatureCount = repository.GetUserFeatures(1).Count;

            // Act
            repository.PurchaseFeature(1, 999); // Non-existent feature
            var finalFeatureCount = repository.GetUserFeatures(1).Count;

            // Assert
            Assert.That(finalFeatureCount, Is.EqualTo(initialFeatureCount));
            Assert.That(repository.IsFeaturePurchased(1, 999), Is.False);
        }

        [Test]
        public void AddFeature_AddsNewFeatureToAvailableFeatures()
        {
            // Arrange
            var initialFeatureCount = repository.GetAllFeatures(1).Count;
            var newFeature = new Feature 
            { 
                FeatureId = 5,
                Name = "Test Feature",
                Type = "test",
                Value = 50,
                Description = "A test feature"
            };

            // Act
            repository.AddFeature(newFeature);
            var features = repository.GetAllFeatures(1);

            // Assert
            Assert.That(features.Count, Is.EqualTo(initialFeatureCount + 1));
            Assert.That(features.Any(f => f.FeatureId == 5 && f.Name == "Test Feature"), Is.True);
        }
    }
} 