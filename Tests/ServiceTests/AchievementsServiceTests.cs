using NUnit.Framework;
using BusinessLayer.Services;
using BusinessLayer.Models;
using System.Collections.Generic;
using BusinessLayer.Exceptions;
using System.Reflection;
using BusinessLayer.Repositories.Interfaces;
using Moq;

namespace Tests.ServiceTests
{
    [TestFixture]
    public class AchievementsServiceTests
    {
        private Mock<IAchievementsRepository> _mockRepo;
        private AchievementsService _service;

        [SetUp]
        public void SetUp()
        {
            _mockRepo = new Mock<IAchievementsRepository>();
            _service = new AchievementsService(_mockRepo.Object);
        }

        [Test]
        public void Constructor_WithNullRepository_ThrowsArgumentNullException()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() =>
            {
                var service = new AchievementsService(null);
            });

            Assert.That(ex.ParamName, Is.EqualTo("achievementsRepository"));
        }

        [Test]
        public void GetGroupedAchievementsForUser_WhenSuccess_ReturnsGroupedResult()
        {
            // Arrange
            var achievements = new List<AchievementWithStatus>
    {
        new AchievementWithStatus { Achievement = new Achievement { AchievementType = "Friendships" } },
        new AchievementWithStatus { Achievement = new Achievement { AchievementType = "Owned Games" } },
        new AchievementWithStatus { Achievement = new Achievement { AchievementType = "Friendships" } },
        new AchievementWithStatus { Achievement = new Achievement { AchievementType = "Developer" } },
    };

            _mockRepo.Setup(r => r.GetAchievementsWithStatusForUser(It.IsAny<int>()))
                     .Returns(achievements);

            var service = new AchievementsService(_mockRepo.Object);

            // Act
            var result = service.GetGroupedAchievementsForUser(1);

            // Assert
            Assert.That(result.AllAchievements.Count, Is.EqualTo(4));
            Assert.That(result.Friendships.Count, Is.EqualTo(2));
            Assert.That(result.OwnedGames.Count, Is.EqualTo(1));
            Assert.That(result.Developer.Count, Is.EqualTo(1));
            Assert.That(result.SoldGames, Is.Empty); // No sold games added
        }

        [Test]
        public void GetGroupedAchievementsForUser_WhenRepoThrows_ThrowsServiceException()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetAchievementsWithStatusForUser(It.IsAny<int>()))
                     .Throws(new RepositoryException("DB error"));

            var service = new AchievementsService(_mockRepo.Object);

            // Act & Assert
            var ex = Assert.Throws<ServiceException>(() => service.GetGroupedAchievementsForUser(42));
            Assert.That(ex.Message, Does.Contain("grouping achievements"));
            Assert.That(ex.InnerException, Is.TypeOf<RepositoryException>());
        }
        [Test]
        public void InitializeAchievements_WhenTableIsEmpty_CallsInsertAndUpdatesIcons()
        {
            var fakeRepo = new FakeAchievementsRepository
            {
                AchievementsTableIsEmpty = true
            };

            var service = new AchievementsService(fakeRepo);

            service.InitializeAchievements();

            Assert.That(fakeRepo.InsertAchievementsCalled, Is.True);
            Assert.That(fakeRepo.UpdatedIcons.Count, Is.EqualTo(5));
        }
        [Test]
        public void InitializeAchievements_WhenRepoThrows_DoesNotCrash()
        {
            var fakeRepo = new FakeAchievementsRepository
            {
                AchievementsTableIsEmpty = true,
                ThrowOnInsertAchievements = true
            };

            var service = new AchievementsService(fakeRepo);

            // should not crash
            Assert.DoesNotThrow(() => service.InitializeAchievements());
        }

        [Test]
        public void GetAchievementsForUser_WhenCalled_ReturnsAchievements()
        {
            var fakeRepo = new FakeAchievementsRepository();
            fakeRepo.AllAchievementsToReturn = new List<Achievement>
    {
        new Achievement { AchievementId = 1 },
        new Achievement { AchievementId = 2 }
    };

            var service = new AchievementsService(fakeRepo);
            var result = service.GetAchievementsForUser(5);

            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].AchievementId, Is.EqualTo(1));
        }

        [Test]
        public void GetAchievementsForUser_WhenRepoThrows_ThrowsServiceException()
        {
            var fakeRepo = new FakeAchievementsRepository { ThrowOnGetAllAchievements = true };
            var service = new AchievementsService(fakeRepo);

            var ex = Assert.Throws<ServiceException>(() => service.GetAchievementsForUser(123));

            Assert.That(ex.Message, Does.Contain("retrieving achievements"));
        }

        [Test]
        public void UpdateAchievementIconUrls_WhenRepositoryThrows_DoesNotCrash()
        {
            var fakeRepo = new FakeAchievementsRepository
            {
                AchievementsTableIsEmpty = true,
                ThrowOnUpdateIconUrl = true
            };

            var service = new AchievementsService(fakeRepo);

            // Act + Assert: Method should catch exception and not throw it
            Assert.DoesNotThrow(() => service.InitializeAchievements());
        }

        [Test]
        public void UnlockAchievementForUser_WithValidFriendshipCount_UnlocksAchievement()
        {
            var fakeRepo = new FakeAchievementsRepository();
            var service = new AchievementsService(fakeRepo);

            int userId = 1;
            int friendCount = 5;
            int expectedAchievementId = 123;

            fakeRepo.NumberOfFriends = friendCount;

            // Only set up the achievement ID mapping
            fakeRepo.AchievementIds["Friendships"] = new Dictionary<int, int?>
    {
        { 2, expectedAchievementId } // 5 friends => FRIENDSHIP2 => index 2
    };

            // Do NOT mark it as already unlocked:
            // fakeRepo.UnlockedAchievements.Add((userId, expectedAchievementId)); ❌ REMOVE THIS if it's there

            // Act
            service.UnlockAchievementForUser(userId);

            // Assert
            Assert.That(fakeRepo.UnlockedAchievements.Any(x =>
                x.userId == userId && x.achievementId == expectedAchievementId), Is.True,
                "The achievement should be unlocked for the valid friendship count.");
        }

        [Test]
        public void UnlockAchievementForUser_WithNoMatchingAchievementId_DoesNotUnlockAnything()
        {
            var fakeRepo = new FakeAchievementsRepository();
            var service = new AchievementsService(fakeRepo);

            int userId = 1;
            int friendCount = 5;

            fakeRepo.NumberOfFriends = friendCount;

            // Act
            service.UnlockAchievementForUser(userId);

            // Assert
            Assert.That(fakeRepo.UnlockedAchievements, Is.Empty,
                "No achievements should be unlocked if the achievement ID is null.");
        }



        [Test]
        public void UnlockAchievementForUser_WithValidOwnedGamesCount_UnlocksAchievement()
        {
            // Arrange
            var fakeRepo = new FakeAchievementsRepository();
            var service = new AchievementsService(fakeRepo);

            int userId = 1;
            int ownedGames = 10;
            int expectedAchievementId = 456;

            fakeRepo.NumberOfOwnedGames = ownedGames;

            // Match what GetAchievementIdByTypeAndCount will internally call: OWNEDGAMES3
            fakeRepo.AchievementIds["Owned Games"] = new Dictionary<int, int?> { { 3, expectedAchievementId } };

            // Act
            service.UnlockAchievementForUser(userId);

            // Assert
            Assert.That(fakeRepo.UnlockedAchievements.Any(x =>
                x.userId == userId && x.achievementId == expectedAchievementId), Is.True,
                "The achievement should be unlocked for the valid owned games count.");
        }

        [Test]
        public void UnlockAchievementForUser_WithOwnedGamesAchievementIdNull_DoesNotUnlock()
        {
            var fakeRepo = new FakeAchievementsRepository();
            var service = new AchievementsService(fakeRepo);
            int userId = 1;
            fakeRepo.NumberOfOwnedGames = 10;

            // Do NOT set fakeRepo.AchievementIds mapping = returns null

            service.UnlockAchievementForUser(userId);

            Assert.That(fakeRepo.UnlockedAchievements, Is.Empty);
        }


        [Test]
        public void UnlockAchievementForUser_UnlocksAchievement_ForValidSoldGames()
        {
            // Arrange
            var userId = 1;
            var expectedAchievementId = 123;

            var fakeRepo = new FakeAchievementsRepository
            {
                NumberOfSoldGames = 10 // Will trigger the unlock logic
            };

            // Setup: maps 10 to "SOLDGAMES3"
            fakeRepo.SetAchievementId("Sold Games", 3, expectedAchievementId);

            var service = new AchievementsService(fakeRepo);

            // Act
            service.UnlockAchievementForUser(userId);

            // Debug output (optional)
            foreach (var a in fakeRepo.UnlockedAchievements)
            {
                Console.WriteLine($"Unlocked: userId={a.userId}, achievementId={a.achievementId}");
            }

            // Assert
            Assert.That(fakeRepo.UnlockedAchievements.Any(x =>
                x.userId == userId && x.achievementId == expectedAchievementId), Is.True,
                "The achievement should be unlocked for valid sold games count.");
        }

        [Test]
        public void UnlockAchievementForUser_WithSoldGamesAchievementIdNull_DoesNotUnlock()
        {
            var fakeRepo = new FakeAchievementsRepository();
            var service = new AchievementsService(fakeRepo);
            int userId = 1;
            fakeRepo.NumberOfSoldGames = 5;

            // No mapping setup → achievementId will be null

            service.UnlockAchievementForUser(userId);

            Assert.That(fakeRepo.UnlockedAchievements, Is.Empty);
        }


        [Test]
        public void UnlockAchievementForUser_WithValidReviewsGivenCount_UnlocksAchievement()
        {
            // Arrange
            var userId = 1;
            var expectedAchievementId = 3;
            var reviewsGiven = 10;

            var fakeRepo = new FakeAchievementsRepository
            {
                NumberOfReviewsGiven = reviewsGiven
            };

            // Must match what GetAchievementIdByTypeAndCount expects:
            fakeRepo.SetAchievementId("Number of Reviews Given", 3, expectedAchievementId);

            var service = new AchievementsService(fakeRepo);

            // Act
            service.UnlockAchievementForUser(userId);

            // Assert
            Assert.That(fakeRepo.UnlockedAchievements.Any(x =>
                x.userId == userId && x.achievementId == expectedAchievementId), Is.True,
                "The achievement should be unlocked for the valid number of reviews given.");
        }

        [Test]
        public void UnlockAchievementForUser_WithReviewsGivenAchievementIdNull_DoesNotUnlock()
        {
            var fakeRepo = new FakeAchievementsRepository();
            var service = new AchievementsService(fakeRepo);
            int userId = 1;
            fakeRepo.NumberOfReviewsGiven = 1;

            service.UnlockAchievementForUser(userId);

            Assert.That(fakeRepo.UnlockedAchievements, Is.Empty);
        }


        [Test]
        public void UnlockAchievementForUser_WithValidReviewsReceivedCount_UnlocksAchievement()
        {
            // Arrange
            var userId = 1;
            var expectedAchievementId = 303;

            var fakeRepo = new FakeAchievementsRepository
            {
                NumberOfReviewsReceived = 10 // This is a valid count to trigger unlock logic
            };

            // This sets up: "Number of Reviews Received" + 10 → "REVIEWR3" → 303
            fakeRepo.SetAchievementId("Number of Reviews Received", 3, expectedAchievementId);

            var service = new AchievementsService(fakeRepo);

            // Act
            service.UnlockAchievementForUser(userId);

            // Assert
            Assert.That(fakeRepo.UnlockedAchievements.Any(x =>
                x.userId == userId && x.achievementId == expectedAchievementId),
                Is.True, "The achievement should be unlocked for the valid number of reviews received.");
        }

        [Test]
        public void UnlockAchievementForUser_WithReviewsReceivedAchievementIdNull_DoesNotUnlock()
        {
            var fakeRepo = new FakeAchievementsRepository();
            var service = new AchievementsService(fakeRepo);
            int userId = 1;
            fakeRepo.NumberOfReviewsReceived = 10;

            service.UnlockAchievementForUser(userId);

            Assert.That(fakeRepo.UnlockedAchievements, Is.Empty);
        }


        [Test]
        public void UnlockAchievementForUser_UnlocksAchievement_ForValidNumberOfPosts()
        {
            // Arrange
            var userId = 1;
            var numberOfPosts = 10;
            var expectedAchievementId = 888; // Arbitrary ID you expect to be unlocked

            var fakeRepo = new FakeAchievementsRepository
            {
                NumberOfPosts = numberOfPosts
            };

            // Set up the mapping: "POSTS3" is expected for 10 posts
            fakeRepo.SetAchievementId("Number of Posts", 3, expectedAchievementId);

            var service = new AchievementsService(fakeRepo);

            // Act
            service.UnlockAchievementForUser(userId);

            // Assert
            Assert.That(fakeRepo.UnlockedAchievements.Any(x =>
                x.userId == userId && x.achievementId == expectedAchievementId),
                Is.True,
                "The achievement should be unlocked for the valid number of posts.");
        }

        [Test]
        public void UnlockAchievementForUser_WithPostsAchievementIdNull_DoesNotUnlock()
        {
            var fakeRepo = new FakeAchievementsRepository();
            var service = new AchievementsService(fakeRepo);
            int userId = 1;
            fakeRepo.NumberOfPosts = 50;

            service.UnlockAchievementForUser(userId);

            Assert.That(fakeRepo.UnlockedAchievements, Is.Empty);
        }


        [Test]
        public void UnlockAchievementForUser_UnlocksAchievement_ForValidYearsOfActivity()
        {
            // Arrange
            var userId = 1;
            var yearsOfActivity = 3;
            var expectedAchievementId = 333; // Simulated ID for ACTIVITY3

            var fakeRepo = new FakeAchievementsRepository
            {
                // Simulate that user has 3 years of activity
                GetYearsOfAcftivity = (id) => yearsOfActivity
            };

            // Set the expected achievement ID for 3 years of activity
            fakeRepo.SetAchievementId("Years of Activity", 3, expectedAchievementId);

            var service = new AchievementsService(fakeRepo);

            // Act
            service.UnlockAchievementForUser(userId);

            // Assert
            Assert.That(fakeRepo.UnlockedAchievements.Any(x =>
                x.userId == userId && x.achievementId == expectedAchievementId),
                Is.True,
                "The achievement should be unlocked for 3 years of activity.");
        }

        [Test]
        public void UnlockAchievementForUser_WithYearsOfActivityAchievementIdNull_DoesNotUnlock()
        {
            var fakeRepo = new FakeAchievementsRepository();
            var service = new AchievementsService(fakeRepo);
            int userId = 1;

            // Simulate 3 years of activity (matches the if condition)
            fakeRepo.GetYearsOfAcftivity = _ => 3;

            // DO NOT define achievement ID → achievementId will be null
            service.UnlockAchievementForUser(userId);

            Assert.That(fakeRepo.UnlockedAchievements, Is.Empty);
        }



        [Test]
        public void UnlockAchievementForUser_UnlocksAchievement_ForDeveloper()
        {
            // Arrange
            var userId = 1;
            var expectedAchievementId = 777; // Arbitrary ID for developer achievement

            var fakeRepo = new FakeAchievementsRepository
            {
                // Simulate the user is a developer
                IsUserDeveloper = (id) => true
            };

            // Map the developer category (count = 1) to the expected ID
            fakeRepo.SetAchievementId("Developer", 1, expectedAchievementId);

            var service = new AchievementsService(fakeRepo);

            // Act
            service.UnlockAchievementForUser(userId);

            // Assert
            Assert.That(fakeRepo.UnlockedAchievements.Any(x =>
                x.userId == userId && x.achievementId == expectedAchievementId),
                Is.True, "The achievement should be unlocked for developer.");
        }

        [Test]
        public void UnlockAchievementForUser_WithDeveloperAchievementIdNull_DoesNotUnlock()
        {
            var fakeRepo = new FakeAchievementsRepository();
            var service = new AchievementsService(fakeRepo);
            int userId = 1;

            // Simulate user is a developer
            fakeRepo.IsUserDeveloper = _ => true;

            // Do NOT define a developer achievement ID
            service.UnlockAchievementForUser(userId);

            Assert.That(fakeRepo.UnlockedAchievements, Is.Empty);
        }



        [Test]
        public void UnlockAchievementForUser_WhenRepositoryThrowsException_IsHandledGracefully()
        {
            // Arrange
            var userId = 1;

            var fakeRepo = new FakeAchievementsRepository
            {
                // Simulate failure on one of the repo methods
                ThrowOnGetNumberOfSoldGames = true // You'll need to add support for this flag
            };

            var service = new AchievementsService(fakeRepo);

            // Act & Assert: The method should NOT throw
            Assert.DoesNotThrow(() => service.UnlockAchievementForUser(userId));
        }


        [Test]
        public void RemoveAchievement_CallsRepositoryWithoutException()
        {
            var fakeRepo = new FakeAchievementsRepository();
            var service = new AchievementsService(fakeRepo);

            // Act
            Assert.DoesNotThrow(() => service.RemoveAchievement(1, 123));
        }

        [Test]
        public void RemoveAchievement_WhenRepositoryThrows_ThrowsServiceException()
        {
            var fakeRepo = new FakeAchievementsRepository
            {
                ThrowOnRemoveAchievement = true // you'll add this
            };

            var service = new AchievementsService(fakeRepo);

            var ex = Assert.Throws<ServiceException>(() => service.RemoveAchievement(1, 123));
            Assert.That(ex.Message, Is.EqualTo("Error removing achievement."));
        }

        [Test]
        public void GetUnlockedAchievementsForUser_ReturnsCorrectList()
        {
            var fakeRepo = new FakeAchievementsRepository();
            var expected = new List<Achievement>
    {
        new Achievement { AchievementId = 1, AchievementName = "Test" }
    };
            fakeRepo.UnlockedAchievementsToReturn = expected; // you'll add this

            var service = new AchievementsService(fakeRepo);

            var result = service.GetUnlockedAchievementsForUser(1);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetUnlockedAchievementsForUser_WhenRepositoryThrows_ThrowsServiceException()
        {
            var fakeRepo = new FakeAchievementsRepository
            {
                ThrowOnGetUnlockedAchievements = true
            };
            var service = new AchievementsService(fakeRepo);

            var ex = Assert.Throws<ServiceException>(() => service.GetUnlockedAchievementsForUser(1));
            Assert.That(ex.Message, Is.EqualTo("Error retrieving unlocked achievements for user."));
        }

        [Test]
        public void GetAllAchievements_ReturnsAllAchievements()
        {
            var fakeRepo = new FakeAchievementsRepository();
            var expected = new List<Achievement>
    {
        new Achievement { AchievementId = 1, AchievementName = "Test Achievement" }
    };
            fakeRepo.AllAchievementsToReturn = expected;

            var service = new AchievementsService(fakeRepo);

            var result = service.GetAllAchievements();

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetAllAchievements_WhenRepositoryThrows_ThrowsServiceException()
        {
            var fakeRepo = new FakeAchievementsRepository
            {
                ThrowOnGetAllAchievements = true
            };
            var service = new AchievementsService(fakeRepo);

            var ex = Assert.Throws<ServiceException>(() => service.GetAllAchievements());
            Assert.That(ex.Message, Is.EqualTo("Error retrieving unlocked achievements for user."));
        }

        [Test]
        public void GetUnlockedDataForAchievement_ReturnsData()
        {
            var fakeRepo = new FakeAchievementsRepository();
            var expected = new AchievementUnlockedData
            {
                AchievementName = "Test",
                UnlockDate = DateTime.Now
            };

            fakeRepo.UnlockedDataToReturn = expected;

            var service = new AchievementsService(fakeRepo);

            var result = service.GetUnlockedDataForAchievement(1, 2);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetUnlockedDataForAchievement_WhenRepositoryThrows_ThrowsServiceException()
        {
            var fakeRepo = new FakeAchievementsRepository
            {
                ThrowOnGetUnlockedData = true
            };

            var service = new AchievementsService(fakeRepo);

            var ex = Assert.Throws<ServiceException>(() => service.GetUnlockedDataForAchievement(1, 1));
            Assert.That(ex.Message, Is.EqualTo("Error retrieving unlocked data for achievement."));
        }

        [Test]
        public void GetAchievementsWithStatusForUser_ReturnsCorrectData()
        {
            var fakeRepo = new FakeAchievementsRepository();
            var expected = new List<AchievementWithStatus>
    {
        new AchievementWithStatus
        {
            Achievement = new Achievement { AchievementId = 1, AchievementName = "Test" },
            IsUnlocked = true,
            UnlockedDate = DateTime.Now
        }
    };
            fakeRepo.AchievementsWithStatusToReturn = expected;

            var service = new AchievementsService(fakeRepo);

            var result = service.GetAchievementsWithStatusForUser(1);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetAchievementsWithStatusForUser_WhenRepositoryThrows_ThrowsServiceException()
        {
            var fakeRepo = new FakeAchievementsRepository
            {
                ThrowOnGetAchievementsWithStatus = true
            };

            var service = new AchievementsService(fakeRepo);

            var ex = Assert.Throws<ServiceException>(() => service.GetAchievementsWithStatusForUser(1));
            Assert.That(ex.Message, Is.EqualTo("Error retrieving achievements with status for user."));
        }
        [Test]
        public void GetPointsForUnlockedAchievement_ReturnsPoints_WhenAchievementIsUnlocked()
        {
            // Arrange
            var userId = 1;
            var achievementId = 42;
            var expectedPoints = 100;

            var fakeRepo = new FakeAchievementsRepository();
            fakeRepo.UnlockAchievement(userId, achievementId); // Mark it as unlocked
            fakeRepo.AllAchievementsToReturn = new List<Achievement>
    {
        new Achievement { AchievementId = achievementId, Points = expectedPoints }
    };

            var service = new AchievementsService(fakeRepo);

            // Act
            var result = service.GetPointsForUnlockedAchievement(userId, achievementId);

            // Assert
            Assert.That(result, Is.EqualTo(expectedPoints));
        }

        [Test]
        public void GetPointsForUnlockedAchievement_Throws_WhenAchievementNotUnlocked()
        {
            var userId = 1;
            var achievementId = 42;

            var fakeRepo = new FakeAchievementsRepository(); // Not unlocking it
            fakeRepo.AllAchievementsToReturn = new List<Achievement>
    {
        new Achievement { AchievementId = achievementId, Points = 50 }
    };

            var service = new AchievementsService(fakeRepo);

            var ex = Assert.Throws<ServiceException>(() =>
                service.GetPointsForUnlockedAchievement(userId, achievementId));

            Assert.That(ex.Message, Is.EqualTo("Achievement is not unlocked or does not exist."));
        }

        [Test]
        public void GetPointsForUnlockedAchievement_Throws_WhenAchievementNotFoundInList()
        {
            var userId = 1;
            var achievementId = 42;

            var fakeRepo = new FakeAchievementsRepository();
            fakeRepo.UnlockAchievement(userId, achievementId); // It's unlocked
            fakeRepo.AllAchievementsToReturn = new List<Achievement>(); // But not in the list

            var service = new AchievementsService(fakeRepo);

            var ex = Assert.Throws<ServiceException>(() =>
                service.GetPointsForUnlockedAchievement(userId, achievementId));

            Assert.That(ex.Message, Is.EqualTo("Achievement is not unlocked or does not exist."));
        }

        [Test]
        public void GetPointsForUnlockedAchievement_ThrowsServiceException_WhenRepositoryFails()
        {
            var fakeRepo = new FakeAchievementsRepository
            {
                ThrowOnIsUnlocked = true
            };

            var service = new AchievementsService(fakeRepo);

            var ex = Assert.Throws<ServiceException>(() =>
                service.GetPointsForUnlockedAchievement(1, 1));

            Assert.That(ex.Message, Is.EqualTo("Error retrieving points for unlocked achievement."));
        }
        [Test]
        public void GetAchievementIdByTypeAndCount_WithUnknownFriendshipCount_ReturnsNull()
        {
            var result = _service.GetAchievementIdByTypeAndCount("Friendships", 999);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetAchievementIdByTypeAndCount_WithUnknownOwnedGamesCount_ReturnsNull()
        {
            var result = _service.GetAchievementIdByTypeAndCount("Owned Games", 999);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetAchievementIdByTypeAndCount_WithUnknownSoldGamesCount_ReturnsNull()
        {
            var result = _service.GetAchievementIdByTypeAndCount("Sold Games", 999);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetAchievementIdByTypeAndCount_WithUnknownReviewsGivenCount_ReturnsNull()
        {
            var result = _service.GetAchievementIdByTypeAndCount("Number of Reviews Given", 999);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetAchievementIdByTypeAndCount_WithUnknownReviewsReceivedCount_ReturnsNull()
        {
            var result = _service.GetAchievementIdByTypeAndCount("Number of Reviews Received", 999);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetAchievementIdByTypeAndCount_WithUnknownYearsOfActivityCount_ReturnsNull()
        {
            var result = _service.GetAchievementIdByTypeAndCount("Years of Activity", 999);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetAchievementIdByTypeAndCount_WithUnknownPostsCount_ReturnsNull()
        {
            var result = _service.GetAchievementIdByTypeAndCount("Number of Posts", 999);
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetAchievementIdByTypeAndCount_WithInvalidDeveloperCount_ReturnsNull()
        {
            var result = _service.GetAchievementIdByTypeAndCount("Developer", 999); // Developer only accepts 1
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetAchievementIdByTypeAndCount_WithUnknownCategory_ReturnsNull()
        {
            var result = _service.GetAchievementIdByTypeAndCount("Unknown Type", 1);
            Assert.That(result, Is.Null);
        }

    }

    [TestFixture]
    public class AchievementServiceTests_GetAchievementIdByTypeAndCount
    {
        [TestCase("Friendships", 1, "FRIENDSHIP1")]
        [TestCase("Friendships", 5, "FRIENDSHIP2")]
        [TestCase("Friendships", 10, "FRIENDSHIP3")]
        [TestCase("Friendships", 50, "FRIENDSHIP4")]
        [TestCase("Friendships", 100, "FRIENDSHIP5")]

        [TestCase("Owned Games", 1, "OWNEDGAMES1")]
        [TestCase("Owned Games", 5, "OWNEDGAMES2")]
        [TestCase("Owned Games", 10, "OWNEDGAMES3")]
        [TestCase("Owned Games", 50, "OWNEDGAMES4")]

        [TestCase("Sold Games", 1, "SOLDGAMES1")]
        [TestCase("Sold Games", 5, "SOLDGAMES2")]
        [TestCase("Sold Games", 10, "SOLDGAMES3")]
        [TestCase("Sold Games", 50, "SOLDGAMES4")]

        [TestCase("Number of Reviews Given", 1, "REVIEW1")]
        [TestCase("Number of Reviews Given", 5, "REVIEW2")]
        [TestCase("Number of Reviews Given", 10, "REVIEW3")]
        [TestCase("Number of Reviews Given", 50, "REVIEW4")]

        [TestCase("Number of Reviews Received", 1, "REVIEWR1")]
        [TestCase("Number of Reviews Received", 5, "REVIEWR2")]
        [TestCase("Number of Reviews Received", 10, "REVIEWR3")]
        [TestCase("Number of Reviews Received", 50, "REVIEWR4")]

        [TestCase("Years of Activity", 1, "ACTIVITY1")]
        [TestCase("Years of Activity", 2, "ACTIVITY2")]
        [TestCase("Years of Activity", 3, "ACTIVITY3")]
        [TestCase("Years of Activity", 4, "ACTIVITY4")]

        [TestCase("Number of Posts", 1, "POSTS1")]
        [TestCase("Number of Posts", 5, "POSTS2")]
        [TestCase("Number of Posts", 10, "POSTS3")]
        [TestCase("Number of Posts", 50, "POSTS4")]

        [TestCase("Developer", 1, "DEVELOPER")]

        public void GetAchievementIdByTypeAndCount_ReturnsExpectedId(string type, int count, string expectedName)
        {
            // Arrange
            var mockRepo = new Mock<IAchievementsRepository>();
            mockRepo.Setup(r => r.GetAchievementIdByName(expectedName)).Returns(123);

            var service = new AchievementsService(mockRepo.Object);

            // Act
            var result = service.GetAchievementIdByTypeAndCount(type, count);

            // Assert
            Assert.That(result, Is.EqualTo(123));
        }


    }




}

