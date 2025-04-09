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

namespace Tests.ServiceTests
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
            this.mockRepository = new Mock<IFeaturesRepository>();
            this.mockUserService = new Mock<IUserService>();
            this.service = new FeaturesService(this.mockRepository.Object, this.mockUserService.Object);
        }

        [Test]
        public void GetFeaturesByCategories_ReturnsCorrectNumberOfCategories()
        {
            // Arrange
            var user = new User { UserId = 1, Email = "test@example.com", Username = "testuser" };
            this.mockUserService.Setup(s => s.GetCurrentUser()).Returns(user);

            var features = new List<Feature>
            {
                new Feature { FeatureId = 1, Name = "Gold Frame", Type = "frame", Value = 100 },
                new Feature { FeatureId = 2, Name = "Silver Frame", Type = "frame", Value = 50 },
                new Feature { FeatureId = 3, Name = "Happy Emoji", Type = "emoji", Value = 30 },
                new Feature { FeatureId = 4, Name = "Sad Emoji", Type = "emoji", Value = 20 }
            };

            this.mockRepository.Setup(mockRepository => mockRepository.GetAllFeatures(user.UserId)).Returns(features);


            // Act
            var categories = this.service.GetFeaturesByCategories();

            // Assert
            Assert.That(categories.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetFeaturesByCategories_ContainsFrameCategory()
        {
            // Arrange
            var user = new User { UserId = 1, Email = "test@example.com", Username = "testuser" };
            this.mockUserService.Setup(s => s.GetCurrentUser()).Returns(user);

            var features = new List<Feature>
            {
                new Feature { FeatureId = 1, Name = "Gold Frame", Type = "frame", Value = 100 },
                new Feature { FeatureId = 2, Name = "Silver Frame", Type = "frame", Value = 50 }
            };

            this.mockRepository.Setup(mockRepository => mockRepository.GetAllFeatures(user.UserId)).Returns(features);


            // Act
            var categories = this.service.GetFeaturesByCategories();

            // Assert
            Assert.That(categories.ContainsKey("frame"), Is.True);
        }

        [Test]
        public void GetFeaturesByCategories_ContainsEmojiCategory()
        {
            // Arrange
            var user = new User { UserId = 1, Email = "test@example.com", Username = "testuser" };
            this.mockUserService.Setup(s => s.GetCurrentUser()).Returns(user);

            var features = new List<Feature>
            {
                new Feature { FeatureId = 3, Name = "Happy Emoji", Type = "emoji", Value = 30 },
                new Feature { FeatureId = 4, Name = "Sad Emoji", Type = "emoji", Value = 20 }
            };

            this.mockRepository.Setup(mockRepository => mockRepository.GetAllFeatures(user.UserId)).Returns(features);


            // Act
            var categories = this.service.GetFeaturesByCategories();

            // Assert
            Assert.That(categories.ContainsKey("emoji"), Is.True);
        }

        [Test]
        public void GetFeaturesByCategories_FrameCategoryHasCorrectCount()
        {
            // Arrange
            var user = new User { UserId = 1, Email = "test@example.com", Username = "testuser" };
            this.mockUserService.Setup(s => s.GetCurrentUser()).Returns(user);

            var features = new List<Feature>
            {
                new Feature { FeatureId = 1, Name = "Gold Frame", Type = "frame", Value = 100 },
                new Feature { FeatureId = 2, Name = "Silver Frame", Type = "frame", Value = 50 }
            };

            this.mockRepository.Setup(mockRepository => mockRepository.GetAllFeatures(user.UserId)).Returns(features);


            // Act
            var categories = this.service.GetFeaturesByCategories();

            // Assert
            Assert.That(categories["frame"].Count, Is.EqualTo(2));
        }

        [Test]
        public void GetFeaturesByCategories_EmojiCategoryHasCorrectCount()
        {
            // Arrange
            var user = new User { UserId = 1, Email = "test@example.com", Username = "testuser" };
            this.mockUserService.Setup(s => s.GetCurrentUser()).Returns(user);

            var features = new List<Feature>
            {
                new Feature { FeatureId = 3, Name = "Happy Emoji", Type = "emoji", Value = 30 },
                new Feature { FeatureId = 4, Name = "Sad Emoji", Type = "emoji", Value = 20 }
            };

            this.mockRepository.Setup(mockRepository => mockRepository.GetAllFeatures(user.UserId)).Returns(features);


            // Act
            var categories = this.service.GetFeaturesByCategories();

            // Assert
            Assert.That(categories["emoji"].Count, Is.EqualTo(2));
        }

        [Test]
        public void EquipFeature_WithNonPurchasedFeature_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            int featureId = 2;

            this.mockRepository.Setup(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId)).Returns(false);


            // Act
            bool result = this.service.EquipFeature(userId, featureId);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void EquipFeature_WithNonPurchasedFeature_VerifiesIsFeaturePurchasedCall()
        {
            // Arrange
            int userId = 1;
            int featureId = 2;
            this.mockRepository.Setup(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId)).Returns(false);


            // Act
            this.service.EquipFeature(userId, featureId);

            // Assert
            this.mockRepository.Verify(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId), Times.Once);

        }

        [Test]
        public void EquipFeature_WithNonPurchasedFeature_DoesNotCallEquipFeature()
        {
            // Arrange
            int userId = 1;
            int featureId = 2;
            this.mockRepository.Setup(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId)).Returns(false);

            // Act
            this.service.EquipFeature(userId, featureId);

            // Assert
            this.mockRepository.Verify(mockRepository => mockRepository.EquipFeature(It.IsAny<int>(), It.IsAny<int>()), Times.Never);

        }

        [Test]
        public void EquipFeature_WithPurchasedNonFrameFeature_ReturnsTrue()
        {
            // Arrange
            int userId = 1;
            int featureId = 3; // Non-frame feature
            this.mockRepository.Setup(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId)).Returns(true);
            this.mockRepository.Setup(mockRepository => mockRepository.GetFeaturesByType("frame")).Returns(new List<Feature>());
            this.mockRepository.Setup(mockRepository => mockRepository.EquipFeature(userId, featureId)).Returns(true);

            // Act
            bool result = this.service.EquipFeature(userId, featureId);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void EquipFeature_WithPurchasedNonFrameFeature_VerifiesIsFeaturePurchasedCall()
        {
            // Arrange
            int userId = 1;
            int featureId = 3; // Non-frame feature
            this.mockRepository.Setup(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId)).Returns(true);
            this.mockRepository.Setup(mockRepository => mockRepository.GetFeaturesByType("frame")).Returns(new List<Feature>());
            this.mockRepository.Setup(mockRepository => mockRepository.EquipFeature(userId, featureId)).Returns(true);

            // Act
            this.service.EquipFeature(userId, featureId);

            // Assert
            this.mockRepository.Verify(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId), Times.Once);

        }

        [Test]
        public void EquipFeature_WithPurchasedNonFrameFeature_DoesNotCallUnequipFeaturesByType()
        {
            // Arrange
            int userId = 1;
            int featureId = 3; // Non-frame feature
            this.mockRepository.Setup(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId)).Returns(true);
            this.mockRepository.Setup(mockRepository => mockRepository.GetFeaturesByType("frame")).Returns(new List<Feature>());
            this.mockRepository.Setup(mockRepository => mockRepository.EquipFeature(userId, featureId)).Returns(true);

            // Act
            this.service.EquipFeature(userId, featureId);

            // Assert
            this.mockRepository.Verify(mockRepository => mockRepository.UnequipFeaturesByType(It.IsAny<int>(), It.IsAny<string>()), Times.Never);

        }

        [Test]
        public void EquipFeature_WithPurchasedNonFrameFeature_VerifiesEquipFeatureCall()
        {
            // Arrange
            int userId = 1;
            int featureId = 3; // Non-frame feature
            this.mockRepository.Setup(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId)).Returns(true);
            this.mockRepository.Setup(mockRepository => mockRepository.GetFeaturesByType("frame")).Returns(new List<Feature>());
            this.mockRepository.Setup(mockRepository => mockRepository.EquipFeature(userId, featureId)).Returns(true);

            // Act
            this.service.EquipFeature(userId, featureId);

            // Assert
            this.mockRepository.Verify(mockRepository => mockRepository.EquipFeature(userId, featureId), Times.Once);

        }

        [Test]
        public void EquipFeature_WithPurchasedFrameFeature_ReturnsTrue()
        {
            // Arrange
            int userId = 1;
            int featureId = 1; // Frame feature
            var frameFeatures = new List<Feature>
            {
                new Feature { FeatureId = 1, Name = "Gold Frame", Type = "frame" },
                new Feature { FeatureId = 2, Name = "Silver Frame", Type = "frame" }
            };

            this.mockRepository.Setup(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId)).Returns(true);
            this.mockRepository.Setup(mockRepository => mockRepository.GetFeaturesByType("frame")).Returns(frameFeatures);
            this.mockRepository.Setup(mockRepository => mockRepository.UnequipFeaturesByType(userId, "frame")).Returns(true);
            this.mockRepository.Setup(mockRepository => mockRepository.EquipFeature(userId, featureId)).Returns(true);

            // Act
            bool result = this.service.EquipFeature(userId, featureId);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void EquipFeature_WithPurchasedFrameFeature_VerifiesIsFeaturePurchasedCall()
        {
            // Arrange
            int userId = 1;
            int featureId = 1; // Frame feature
            var frameFeatures = new List<Feature>
            {
                new Feature { FeatureId = 1, Name = "Gold Frame", Type = "frame" },
                new Feature { FeatureId = 2, Name = "Silver Frame", Type = "frame" }
            };

            this.mockRepository.Setup(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId)).Returns(true);
            this.mockRepository.Setup(mockRepository => mockRepository.GetFeaturesByType("frame")).Returns(frameFeatures);
            this.mockRepository.Setup(mockRepository => mockRepository.UnequipFeaturesByType(userId, "frame")).Returns(true);
            this.mockRepository.Setup(mockRepository => mockRepository.EquipFeature(userId, featureId)).Returns(true);

            // Act
            this.service.EquipFeature(userId, featureId);

            // Assert
            this.mockRepository.Verify(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId), Times.Once);

        }

        [Test]
        public void EquipFeature_WithPurchasedFrameFeature_VerifiesUnequipFeaturesByTypeCall()
        {
            // Arrange
            int userId = 1;
            int featureId = 1; // Frame feature
            var frameFeatures = new List<Feature>
            {
                new Feature { FeatureId = 1, Name = "Gold Frame", Type = "frame" },
                new Feature { FeatureId = 2, Name = "Silver Frame", Type = "frame" }
            };

            this.mockRepository.Setup(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId)).Returns(true);
            this.mockRepository.Setup(mockRepository => mockRepository.GetFeaturesByType("frame")).Returns(frameFeatures);
            this.mockRepository.Setup(mockRepository => mockRepository.UnequipFeaturesByType(userId, "frame")).Returns(true);
            this.mockRepository.Setup(mockRepository => mockRepository.EquipFeature(userId, featureId)).Returns(true);

            // Act
            this.service.EquipFeature(userId, featureId);

            // Assert
            this.mockRepository.Verify(mockRepository => mockRepository.UnequipFeaturesByType(userId, "frame"), Times.Once);

        }

        [Test]
        public void EquipFeature_WithPurchasedFrameFeature_VerifiesEquipFeatureCall()
        {
            // Arrange
            int userId = 1;
            int featureId = 1; // Frame feature
            var frameFeatures = new List<Feature>
            {
                new Feature { FeatureId = 1, Name = "Gold Frame", Type = "frame" },
                new Feature { FeatureId = 2, Name = "Silver Frame", Type = "frame" }
            };

            this.mockRepository.Setup(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId)).Returns(true);
            this.mockRepository.Setup(mockRepository => mockRepository.GetFeaturesByType("frame")).Returns(frameFeatures);
            this.mockRepository.Setup(mockRepository => mockRepository.UnequipFeaturesByType(userId, "frame")).Returns(true);
            this.mockRepository.Setup(mockRepository => mockRepository.EquipFeature(userId, featureId)).Returns(true);


            // Act
            this.service.EquipFeature(userId, featureId);

            // Assert
            this.mockRepository.Verify(mockRepository => mockRepository.EquipFeature(userId, featureId), Times.Once);

        }

        [Test]
        public void UnequipFeature_WithNonPurchasedFeature_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            int featureId = 2;
            this.mockRepository.Setup(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId)).Returns(false);


            // Act
            var result = this.service.UnequipFeature(userId, featureId);

            // Assert
            Assert.That(result.Item1, Is.False);
        }

        [Test]
        public void UnequipFeature_WithNonPurchasedFeature_ReturnsCorrectMessage()
        {
            // Arrange
            int userId = 1;
            int featureId = 2;
            this.mockRepository.Setup(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId)).Returns(false);

            // Act
            var result = this.service.UnequipFeature(userId, featureId);

            // Assert
            Assert.That(result.Item2, Is.EqualTo("Feature not purchased"));
        }

        [Test]
        public void UnequipFeature_WithNonPurchasedFeature_VerifiesIsFeaturePurchasedCall()
        {
            // Arrange
            int userId = 1;
            int featureId = 2;
            this.mockRepository.Setup(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId)).Returns(false);

            // Act
            this.service.UnequipFeature(userId, featureId);

            // Assert

            this.mockRepository.Verify(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId), Times.Once);

        }

        [Test]
        public void UnequipFeature_WithNonPurchasedFeature_DoesNotCallUnequipFeature()
        {
            // Arrange
            int userId = 1;
            int featureId = 2;

            this.mockRepository.Setup(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId)).Returns(false);


            // Act
            this.service.UnequipFeature(userId, featureId);

            // Assert

            this.mockRepository.Verify(mockRepository => mockRepository.UnequipFeature(It.IsAny<int>(), It.IsAny<int>()), Times.Never);

        }

        [Test]
        public void UnequipFeature_WithPurchasedFeatureSuccess_ReturnsTrue()
        {
            // Arrange
            int userId = 1;
            int featureId = 1;

            this.mockRepository.Setup(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId)).Returns(true);
            this.mockRepository.Setup(mockRepository => mockRepository.UnequipFeature(userId, featureId)).Returns(true);


            // Act
            var result = this.service.UnequipFeature(userId, featureId);

            // Assert
            Assert.That(result.Item1, Is.True);
        }

        [Test]
        public void UnequipFeature_WithPurchasedFeatureSuccess_ReturnsSuccessMessage()
        {
            // Arrange
            int userId = 1;
            int featureId = 1;

            this.mockRepository.Setup(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId)).Returns(true);
            this.mockRepository.Setup(mockRepository => mockRepository.UnequipFeature(userId, featureId)).Returns(true);


            // Act
            var result = this.service.UnequipFeature(userId, featureId);

            // Assert
            Assert.That(result.Item2, Is.EqualTo("Feature unequipped successfully"));
        }

        [Test]
        public void UnequipFeature_WithPurchasedFeatureSuccess_VerifiesIsFeaturePurchasedCall()
        {
            // Arrange
            int userId = 1;
            int featureId = 1;

            this.mockRepository.Setup(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId)).Returns(true);
            this.mockRepository.Setup(mockRepository => mockRepository.UnequipFeature(userId, featureId)).Returns(true);


            // Act
            this.service.UnequipFeature(userId, featureId);

            // Assert

            this.mockRepository.Verify(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId), Times.Once);

        }

        [Test]
        public void UnequipFeature_WithPurchasedFeatureSuccess_VerifiesUnequipFeatureCall()
        {
            // Arrange
            int userId = 1;
            int featureId = 1;
            this.mockRepository.Setup(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId)).Returns(true);
            this.mockRepository.Setup(mockRepository => mockRepository.UnequipFeature(userId, featureId)).Returns(true);


            // Act
            this.service.UnequipFeature(userId, featureId);

            // Assert
            this.mockRepository.Verify(mockRepository => mockRepository.UnequipFeature(userId, featureId), Times.Once);

        }

        [Test]
        public void UnequipFeature_WithPurchasedFeatureFailure_ReturnsFalse()
        {
            // Arrange
            int userId = 1;
            int featureId = 1;
            this.mockRepository.Setup(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId)).Returns(true);
            this.mockRepository.Setup(mockRepository => mockRepository.UnequipFeature(userId, featureId)).Returns(false);


            // Act
            var result = this.service.UnequipFeature(userId, featureId);

            // Assert
            Assert.That(result.Item1, Is.False);
        }

        [Test]
        public void UnequipFeature_WithPurchasedFeatureFailure_ReturnsFailureMessage()
        {
            // Arrange
            int userId = 1;
            int featureId = 1;

            this.mockRepository.Setup(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId)).Returns(true);
            this.mockRepository.Setup(mockRepository => mockRepository.UnequipFeature(userId, featureId)).Returns(false);


            // Act
            var result = this.service.UnequipFeature(userId, featureId);

            // Assert
            Assert.That(result.Item2, Is.EqualTo("Failed to unequip feature"));
        }

        [Test]
        public void UnequipFeature_WithPurchasedFeatureFailure_VerifiesIsFeaturePurchasedCall()
        {
            // Arrange
            int userId = 1;
            int featureId = 1;

            this.mockRepository.Setup(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId)).Returns(true);
            this.mockRepository.Setup(mockRepository => mockRepository.UnequipFeature(userId, featureId)).Returns(false);


            // Act
            this.service.UnequipFeature(userId, featureId);

            // Assert

            this.mockRepository.Verify(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId), Times.Once);

        }

        [Test]
        public void UnequipFeature_WithPurchasedFeatureFailure_VerifiesUnequipFeatureCall()
        {
            // Arrange
            int userId = 1;
            int featureId = 1;

            this.mockRepository.Setup(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId)).Returns(true);
            this.mockRepository.Setup(mockRepository => mockRepository.UnequipFeature(userId, featureId)).Returns(false);


            // Act
            this.service.UnequipFeature(userId, featureId);

            // Assert

            this.mockRepository.Verify(mockRepository => mockRepository.UnequipFeature(userId, featureId), Times.Once);

        }

        [Test]
        public void GetUserEquippedFeatures_ReturnsCorrectCount()
        {
            // Arrange
            int userId = 1;
            var userFeatures = new List<Feature>
            {
                new Feature { FeatureId = 1, Name = "Gold Frame", Type = "frame", Equipped = true },
                new Feature { FeatureId = 2, Name = "Silver Frame", Type = "frame", Equipped = false },
                new Feature { FeatureId = 3, Name = "Happy Emoji", Type = "emoji", Equipped = true }
            };


            this.mockRepository.Setup(mockRepository => mockRepository.GetUserFeatures(userId)).Returns(userFeatures);


            // Act
            var equippedFeatures = this.service.GetUserEquippedFeatures(userId);

            // Assert
            Assert.That(equippedFeatures.Count, Is.EqualTo(2));
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

            this.mockRepository.Setup(mockRepository => mockRepository.GetUserFeatures(userId)).Returns(userFeatures);


            // Act
            var equippedFeatures = this.service.GetUserEquippedFeatures(userId);

            // Assert
            Assert.That(equippedFeatures.All(f => f.Equipped), Is.True);
        }

        [Test]
        public void GetUserEquippedFeatures_ContainsGoldFrame()
        {
            // Arrange
            int userId = 1;
            var userFeatures = new List<Feature>
            {
                new Feature { FeatureId = 1, Name = "Gold Frame", Type = "frame", Equipped = true },
                new Feature { FeatureId = 2, Name = "Silver Frame", Type = "frame", Equipped = false },
                new Feature { FeatureId = 3, Name = "Happy Emoji", Type = "emoji", Equipped = true }
            };

            this.mockRepository.Setup(mockRepository => mockRepository.GetUserFeatures(userId)).Returns(userFeatures);

            // Act
            var equippedFeatures = this.service.GetUserEquippedFeatures(userId);

            // Assert
            Assert.That(equippedFeatures.Any(f => f.FeatureId == 1), Is.True);
        }

        [Test]
        public void GetUserEquippedFeatures_ContainsHappyEmoji()
        {
            // Arrange
            int userId = 1;
            var userFeatures = new List<Feature>
            {
                new Feature { FeatureId = 1, Name = "Gold Frame", Type = "frame", Equipped = true },
                new Feature { FeatureId = 2, Name = "Silver Frame", Type = "frame", Equipped = false },
                new Feature { FeatureId = 3, Name = "Happy Emoji", Type = "emoji", Equipped = true }
            };

            this.mockRepository.Setup(mockRepository => mockRepository.GetUserFeatures(userId)).Returns(userFeatures);


            // Act
            var equippedFeatures = this.service.GetUserEquippedFeatures(userId);

            // Assert
            Assert.That(equippedFeatures.Any(f => f.FeatureId == 3), Is.True);
        }

        [Test]
        public void GetUserEquippedFeatures_DoesNotContainSilverFrame()
        {
            // Arrange
            int userId = 1;
            var userFeatures = new List<Feature>
            {
                new Feature { FeatureId = 1, Name = "Gold Frame", Type = "frame", Equipped = true },
                new Feature { FeatureId = 2, Name = "Silver Frame", Type = "frame", Equipped = false },
                new Feature { FeatureId = 3, Name = "Happy Emoji", Type = "emoji", Equipped = true }
            };

            this.mockRepository.Setup(mockRepository => mockRepository.GetUserFeatures(userId)).Returns(userFeatures);


            // Act
            var equippedFeatures = this.service.GetUserEquippedFeatures(userId);

            // Assert
            Assert.That(equippedFeatures.Any(f => f.FeatureId == 2), Is.False);
        }

        [Test]
        public void IsFeaturePurchased_DelegatesCallToRepository()
        {
            // Arrange
            int userId = 1;
            int featureId = 1;

            this.mockRepository.Setup(mockRepository => mockRepository.IsFeaturePurchased(userId, featureId)).Returns(true);


            // Act
            bool result = this.service.IsFeaturePurchased(userId, featureId);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void GetUserFeatures_WithValidFeatures_ReturnsFeatures()
        {
            // Arrange
            int userId = 1;
            var features = new List<Feature>
            {
                new Feature { FeatureId = 1, Name = "Gold Frame", Type = "frame", Value = 100 },
                new Feature { FeatureId = 2, Name = "Silver Frame", Type = "frame", Value = 50 }
            };

            this.mockRepository.Setup(mockRepository => mockRepository.GetUserFeatures(userId)).Returns(features);

            // Act
            var result = this.service.GetUserFeatures(userId);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void GetUserFeatures_WithInvalidFeature_ThrowsValidationException()
        {
            // Arrange
            int userId = 1;
            var features = new List<Feature>
            {
                new Feature { FeatureId = 1, Name = "", Type = "frame", Value = 100 } // Invalid feature
            };

            this.mockRepository.Setup(mockRepository => mockRepository.GetUserFeatures(userId)).Returns(features);


            // Act & Assert
            Assert.That(() => this.service.GetUserFeatures(userId), Throws.TypeOf<ValidationException>());
        }
    }
}