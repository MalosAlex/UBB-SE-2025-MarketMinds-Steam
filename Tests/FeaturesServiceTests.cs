using System.Collections.Generic;
using System.Linq;
using BusinessLayer.Exceptions;
using BusinessLayer.Models;
using BusinessLayer.Repositories.Fakes;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Services;
using BusinessLayer.Services.Fakes;
using BusinessLayer.Services.Interfaces;
using Moq;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class FeaturesServiceTests
    {
        private FeaturesService service;
        private Mock<IFeaturesRepository> mockRepository;
        private Mock<IUserService> mockUserService;

        [SetUp]
        public void Setup()
        {
            mockRepository = new Mock<IFeaturesRepository>();
            mockUserService = new Mock<IUserService>();
            service = new FeaturesService(mockRepository.Object, mockUserService.Object);
        }

        [Test]
        public void GetFeaturesByCategories_GroupsFeaturesByType()
        {
            // Arrange
            var user = new User { UserId = 1, Email = "test@example.com", Username = "testuser" };
            mockUserService.Setup(s => s.GetCurrentUser()).Returns(user);

            var features = new List<Feature>
            {
                new Feature { FeatureId = 1, Name = "Gold Frame", Type = "frame", Value = 100 },
                new Feature { FeatureId = 2, Name = "Silver Frame", Type = "frame", Value = 50 },
                new Feature { FeatureId = 3, Name = "Happy Emoji", Type = "emoji", Value = 30 },
                new Feature { FeatureId = 4, Name = "Sad Emoji", Type = "emoji", Value = 20 }
            };

            mockRepository.Setup(r => r.GetAllFeatures(user.UserId)).Returns(features);

            // Act
            var categories = service.GetFeaturesByCategories();

            // Assert
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
            int featureId = 2;
            mockRepository.Setup(r => r.IsFeaturePurchased(userId, featureId)).Returns(false);

            // Act
            bool result = service.EquipFeature(userId, featureId);

            // Assert
            Assert.That(result, Is.False);
            mockRepository.Verify(r => r.IsFeaturePurchased(userId, featureId), Times.Once);
            mockRepository.Verify(r => r.EquipFeature(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void EquipFeature_WithPurchasedNonFrameFeature_EquipsWithoutUnequipping()
        {
            // Arrange
            int userId = 1;
            int featureId = 3; // Non-frame feature
            mockRepository.Setup(r => r.IsFeaturePurchased(userId, featureId)).Returns(true);
            mockRepository.Setup(r => r.GetFeaturesByType("frame")).Returns(new List<Feature>());
            mockRepository.Setup(r => r.EquipFeature(userId, featureId)).Returns(true);

            // Act
            bool result = service.EquipFeature(userId, featureId);

            // Assert
            Assert.That(result, Is.True);
            mockRepository.Verify(r => r.IsFeaturePurchased(userId, featureId), Times.Once);
            mockRepository.Verify(r => r.UnequipFeaturesByType(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
            mockRepository.Verify(r => r.EquipFeature(userId, featureId), Times.Once);
        }

        [Test]
        public void EquipFeature_WithPurchasedFrameFeature_UnequipsOtherFramesFirst()
        {
            // Arrange
            int userId = 1;
            int featureId = 1; // Frame feature
            var frameFeatures = new List<Feature>
            {
                new Feature { FeatureId = 1, Name = "Gold Frame", Type = "frame" },
                new Feature { FeatureId = 2, Name = "Silver Frame", Type = "frame" }
            };

            mockRepository.Setup(r => r.IsFeaturePurchased(userId, featureId)).Returns(true);
            mockRepository.Setup(r => r.GetFeaturesByType("frame")).Returns(frameFeatures);
            mockRepository.Setup(r => r.UnequipFeaturesByType(userId, "frame")).Returns(true);
            mockRepository.Setup(r => r.EquipFeature(userId, featureId)).Returns(true);

            // Act
            bool result = service.EquipFeature(userId, featureId);

            // Assert
            Assert.That(result, Is.True);
            mockRepository.Verify(r => r.IsFeaturePurchased(userId, featureId), Times.Once);
            mockRepository.Verify(r => r.UnequipFeaturesByType(userId, "frame"), Times.Once);
            mockRepository.Verify(r => r.EquipFeature(userId, featureId), Times.Once);
        }

        [Test]
        public void UnequipFeature_WithNonPurchasedFeature_ReturnsFalseWithMessage()
        {
            // Arrange
            int userId = 1;
            int featureId = 2;
            mockRepository.Setup(r => r.IsFeaturePurchased(userId, featureId)).Returns(false);

            // Act
            var result = service.UnequipFeature(userId, featureId);

            // Assert
            Assert.That(result.Item1, Is.False);
            Assert.That(result.Item2, Is.EqualTo("Feature not purchased"));
            mockRepository.Verify(r => r.IsFeaturePurchased(userId, featureId), Times.Once);
            mockRepository.Verify(r => r.UnequipFeature(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Test]
        public void UnequipFeature_WithPurchasedFeatureSuccess_ReturnsTrueWithMessage()
        {
            // Arrange
            int userId = 1;
            int featureId = 1;
            mockRepository.Setup(r => r.IsFeaturePurchased(userId, featureId)).Returns(true);
            mockRepository.Setup(r => r.UnequipFeature(userId, featureId)).Returns(true);

            // Act
            var result = service.UnequipFeature(userId, featureId);

            // Assert
            Assert.That(result.Item1, Is.True);
            Assert.That(result.Item2, Is.EqualTo("Feature unequipped successfully"));
            mockRepository.Verify(r => r.IsFeaturePurchased(userId, featureId), Times.Once);
            mockRepository.Verify(r => r.UnequipFeature(userId, featureId), Times.Once);
        }

        [Test]
        public void UnequipFeature_WithPurchasedFeatureFailure_ReturnsFalseWithMessage()
        {
            // Arrange
            int userId = 1;
            int featureId = 1;
            mockRepository.Setup(r => r.IsFeaturePurchased(userId, featureId)).Returns(true);
            mockRepository.Setup(r => r.UnequipFeature(userId, featureId)).Returns(false);

            // Act
            var result = service.UnequipFeature(userId, featureId);

            // Assert
            Assert.That(result.Item1, Is.False);
            Assert.That(result.Item2, Is.EqualTo("Failed to unequip feature"));
            mockRepository.Verify(r => r.IsFeaturePurchased(userId, featureId), Times.Once);
            mockRepository.Verify(r => r.UnequipFeature(userId, featureId), Times.Once);
        }

        [Test]
        public void GetUserEquippedFeatures_ReturnsOnlyEquippedFeatures()
        {
            // Arrange
            int userId = 1;
            var userFeatures = new List<Feature>
            {
                new Feature { FeatureId = 1, Name = "Gold Frame", Type = "frame", Equipped = true },
                new Feature { FeatureId = 2, Name = "Silver Frame", Type = "frame", Equipped = false },
                new Feature { FeatureId = 3, Name = "Happy Emoji", Type = "emoji", Equipped = true }
            };

            mockRepository.Setup(r => r.GetUserFeatures(userId)).Returns(userFeatures);

            // Act
            var equippedFeatures = service.GetUserEquippedFeatures(userId);

            // Assert
            Assert.That(equippedFeatures.Count, Is.EqualTo(2));
            Assert.That(equippedFeatures.All(f => f.Equipped), Is.True);
            Assert.That(equippedFeatures.Any(f => f.FeatureId == 1), Is.True);
            Assert.That(equippedFeatures.Any(f => f.FeatureId == 3), Is.True);
            Assert.That(equippedFeatures.Any(f => f.FeatureId == 2), Is.False);
        }

        [Test]
        public void IsFeaturePurchased_DelegatesCallToRepository()
        {
            // Arrange
            int userId = 1;
            int featureId = 1;
            mockRepository.Setup(r => r.IsFeaturePurchased(userId, featureId)).Returns(true);

            // Act
            bool result = service.IsFeaturePurchased(userId, featureId);

            // Assert
            Assert.That(result, Is.True);
            mockRepository.Verify(r => r.IsFeaturePurchased(userId, featureId), Times.Once);
        }

        [Test]
        public void GetUserFeatures_WithValidFeatures_ReturnsFeatures()
        {
            // Arrange
            int userId = 1;
            var userFeatures = new List<Feature>
            {
                new Feature { FeatureId = 1, Name = "Gold Frame", Type = "frame", Value = 100 },
                new Feature { FeatureId = 3, Name = "Happy Emoji", Type = "emoji", Value = 30 }
            };

            mockRepository.Setup(r => r.GetUserFeatures(userId)).Returns(userFeatures);

            // Act
            var features = service.GetUserFeatures(userId);

            // Assert
            Assert.That(features.Count, Is.EqualTo(2));
            Assert.That(features, Is.EqualTo(userFeatures));
        }

        [Test]
        public void GetUserFeatures_WithInvalidFeature_ThrowsValidationException()
        {
            // Arrange
            int userId = 1;
            var userFeatures = new List<Feature>
            {
                new Feature { FeatureId = 1, Name = null, Type = "frame", Value = 100 } // Invalid feature (null name)
            };

            mockRepository.Setup(r => r.GetUserFeatures(userId)).Returns(userFeatures);

            // Act & Assert
            Assert.That(() => service.GetUserFeatures(userId), Throws.TypeOf<ValidationException>());
        }

        [Test]
        public void FeaturesService_WithFakeRepositoryAndUserService_WorksCorrectly()
        {
            // Arrange - Create a service with fake repository implementation
            var fakeRepository = new FakeFeaturesRepository();
            var fakeUserService = new FakeUserService();
            var serviceWithFakes = new FeaturesService(fakeRepository, fakeUserService);

            // Act - Test various operations
            var categories = serviceWithFakes.GetFeaturesByCategories();
            bool equip = serviceWithFakes.EquipFeature(1, 1); // Equip a feature the user has (Gold Frame)
            var equipped = serviceWithFakes.GetUserEquippedFeatures(1);
            var purchaseCheck = serviceWithFakes.IsFeaturePurchased(1, 2); // Check a feature the user doesn't have

            // Assert
            Assert.That(categories.Count, Is.GreaterThan(0));
            Assert.That(equip, Is.True);
            Assert.That(equipped, Is.Not.Empty);
            Assert.That(purchaseCheck, Is.False);
        }
    }
} 