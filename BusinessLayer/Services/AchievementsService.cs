using System;
using BusinessLayer.Exceptions;
using BusinessLayer.Models;
using BusinessLayer.Repositories;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Services.Interfaces;

namespace BusinessLayer.Services
{
    public class AchievementsService : IAchievementsService
    {
        private readonly IAchievementsRepository achievementsRepository;

        public static class Categories
        {
            public const string Friendships = "Friendships";
            public const string OwnedGames = "Owned Games";
            public const string SoldGames = "Sold Games";
            public const string YearsOfActivity = "Years of Activity";
            public const string NumberOfPosts = "Number of Posts";
            public const string NumberOfReviewsGiven = "Number of Reviews Given";
            public const string NumberOfReviewsReceived = "Number of Reviews Received";
            public const string Developer = "Developer";
        }

        public class GroupedAchievementsResult
        {
            public List<AchievementWithStatus> AllAchievements { get; set; }
            public List<AchievementWithStatus> Friendships { get; set; }
            public List<AchievementWithStatus> OwnedGames { get; set; }
            public List<AchievementWithStatus> SoldGames { get; set; }
            public List<AchievementWithStatus> YearsOfActivity { get; set; }
            public List<AchievementWithStatus> NumberOfPosts { get; set; }
            public List<AchievementWithStatus> NumberOfReviewsGiven { get; set; }
            public List<AchievementWithStatus> NumberOfReviewsReceived { get; set; }
            public List<AchievementWithStatus> Developer { get; set; }
        }

        // Achievement Points Levels
        private const int PointsLevelBronze = 1;
        private const int PointsLevelSilver = 3;
        private const int PointsLevelGold = 5;
        private const int PointsLevelPlatinum = 10;
        private const int PointsLevelDiamond = 15;

        // Milestones for Achievements
        private const int MilestoneLevel1 = 1;
        private const int MilestoneLevel2 = 2;
        private const int MilestoneLevel3 = 3;
        private const int MilestoneLevel4 = 4;
        private const int MilestoneLevel5 = 5;
        private const int MilestoneLevel10 = 10;
        private const int MilestoneLevel50 = 50;
        private const int MilestoneLevel100 = 100;

        public GroupedAchievementsResult GetGroupedAchievementsForUser(int userId)
        {
            try
            {
                // Unlock logic (business rules)
                UnlockAchievementForUser(userId);

                // Get all achievements with status
                var achievements = achievementsRepository.GetAchievementsWithStatusForUser(userId);

                // Group by category
                return new GroupedAchievementsResult
                {
                    AllAchievements = achievements,
                    Friendships = FilterByCategory(achievements, "Friendships"),
                    OwnedGames = FilterByCategory(achievements, "Owned Games"),
                    SoldGames = FilterByCategory(achievements, "Sold Games"),
                    YearsOfActivity = FilterByCategory(achievements, "Years of Activity"),
                    NumberOfPosts = FilterByCategory(achievements, "Number of Posts"),
                    NumberOfReviewsGiven = FilterByCategory(achievements, "Number of Reviews Given"),
                    NumberOfReviewsReceived = FilterByCategory(achievements, "Number of Reviews Received"),
                    Developer = FilterByCategory(achievements, "Developer")
                };
            }
            catch (RepositoryException exception)
            {
                throw new ServiceException("Error grouping achievements for user.", exception);
            }
        }

        private List<AchievementWithStatus> FilterByCategory(List<AchievementWithStatus> allAchievements, string category)
        {
            var filteredAchievementsList = new List<AchievementWithStatus>();

            foreach (var achievement in allAchievements)
            {
                if (achievement.Achievement.AchievementType == category)
                {
                    filteredAchievementsList.Add(achievement);
                }
            }
            return filteredAchievementsList;
        }
        public AchievementsService(IAchievementsRepository achievementsRepository)
        {
            if (achievementsRepository == null)
            {
                throw new ArgumentNullException(nameof(achievementsRepository));
            }

            this.achievementsRepository = achievementsRepository;
        }

        public void InitializeAchievements()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Checking if achievements table is empty...");
                if (achievementsRepository.IsAchievementsTableEmpty())
                {
                    System.Diagnostics.Debug.WriteLine("Achievements table is empty. Inserting achievements...");
                    achievementsRepository.InsertAchievements();
                    System.Diagnostics.Debug.WriteLine("Achievements inserted successfully.");
                    UpdateAchievementIconUrls();
                }
            }
            catch (RepositoryException exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing achievements: {exception.Message}");
            }
        }

        private void UpdateAchievementIconUrls()
        {
            try
            {
                var iconUrls = new Dictionary<int, string>
                {
                    { PointsLevelBronze, "https://t4.ftcdn.net/jpg/00/99/53/31/360_F_99533164_fpE2O6vEjnXgYhonMyYBGtGUFCLqfTWA.jpg" },
                    { PointsLevelSilver, "https://png.pngtree.com/png-clipart/20200401/original/pngtree-gold-number-5-png-image_5330870.jpg" },
                    { PointsLevelGold, "https://t4.ftcdn.net/jpg/01/93/98/05/360_F_193980561_lymRkyDG6roPxmgA6x27fEaq3O3z3Mcf.jpg" },
                    { PointsLevelPlatinum, "https://as1.ftcdn.net/v2/jpg/02/42/16/20/1000_F_242162042_Ve21lDSZQl3Ebb9laV1WAJrR0ls3RGAn.jpg" },
                    { PointsLevelDiamond, "https://t3.ftcdn.net/jpg/02/79/95/72/360_F_279957287_UsAVf2woGRBWekMX68LiiWpwrrVVy9bI.jpg" }
                };
                foreach (var iconUrl in iconUrls)
                {
                    System.Diagnostics.Debug.WriteLine($"Updating icon URL for points: {iconUrl.Key}, URL: {iconUrl.Value}");
                    achievementsRepository.UpdateAchievementIconUrl(iconUrl.Key, iconUrl.Value);
                }

                System.Diagnostics.Debug.WriteLine("Achievement icon URLs updated successfully.");
            }
            catch (RepositoryException exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating achievement icon URLs: {exception.Message}");
            }
        }

        public List<Achievement> GetAchievementsForUser(int userId)
        {
            try
            {
                return achievementsRepository.GetAllAchievements();
            }
            catch (RepositoryException exception)
            {
                throw new ServiceException("Error retrieving achievements for user.", exception);
            }
        }

        public void UnlockAchievementForUser(int userId)
        {
            try
            {
                int numberOfSoldGames = achievementsRepository.GetNumberOfSoldGames(userId);
                int numberOfFriends = achievementsRepository.GetFriendshipCount(userId);
                int numberOfOwnedGames = achievementsRepository.GetNumberOfOwnedGames(userId);
                int numberOfReviewsGiven = achievementsRepository.GetNumberOfReviewsGiven(userId);
                int numberOfReviewsReceived = achievementsRepository.GetNumberOfReviewsReceived(userId);
                int numberOfPosts = achievementsRepository.GetNumberOfPosts(userId);
                int yearsOfActivity = achievementsRepository.GetYearsOfAcftivity(userId);
                bool isDeveloper = achievementsRepository.IsUserDeveloper(userId);

                if (numberOfFriends == MilestoneLevel1 || numberOfFriends == MilestoneLevel5 || numberOfFriends == MilestoneLevel10 || numberOfFriends == MilestoneLevel50 || numberOfFriends == MilestoneLevel100)
                {
                    int? achievementId = GetAchievementIdByTypeAndCount("Friendships", numberOfFriends);
                    System.Diagnostics.Debug.WriteLine($"Achievement ID for friendships with count {numberOfFriends}: {achievementId}");
                    if (achievementId.HasValue && !achievementsRepository.IsAchievementUnlocked(userId, achievementId.Value))
                    {
                        System.Diagnostics.Debug.WriteLine($"Unlocking achievement ID {achievementId.Value} for user {userId}");
                        achievementsRepository.UnlockAchievement(userId, achievementId.Value);
                    }
                }

                if (numberOfOwnedGames == MilestoneLevel1 || numberOfOwnedGames == MilestoneLevel5 || numberOfOwnedGames == MilestoneLevel10 || numberOfOwnedGames == MilestoneLevel50)
                {
                    int? achievementId = GetAchievementIdByTypeAndCount("Owned Games", numberOfOwnedGames);
                    System.Diagnostics.Debug.WriteLine($"Achievement ID for owned games with count {numberOfOwnedGames}: {achievementId}");
                    if (achievementId.HasValue && !achievementsRepository.IsAchievementUnlocked(userId, achievementId.Value))
                    {
                        System.Diagnostics.Debug.WriteLine($"Unlocking achievement ID {achievementId.Value} for user {userId}");
                        achievementsRepository.UnlockAchievement(userId, achievementId.Value);
                    }
                }

                if (numberOfSoldGames == MilestoneLevel1 || numberOfSoldGames == MilestoneLevel5 || numberOfSoldGames == MilestoneLevel10 || numberOfSoldGames == MilestoneLevel50)
                {
                    int? achievementId = GetAchievementIdByTypeAndCount("Sold Games", numberOfSoldGames);
                    System.Diagnostics.Debug.WriteLine($"Achievement ID for sold games with count {numberOfSoldGames}: {achievementId}");
                    if (achievementId.HasValue && !achievementsRepository.IsAchievementUnlocked(userId, achievementId.Value))
                    {
                        System.Diagnostics.Debug.WriteLine($"Unlocking achievement ID {achievementId.Value} for user {userId}");
                        achievementsRepository.UnlockAchievement(userId, achievementId.Value);
                    }
                }

                if (numberOfReviewsGiven == MilestoneLevel1 || numberOfReviewsGiven == MilestoneLevel5 || numberOfReviewsGiven == MilestoneLevel10 || numberOfReviewsGiven == MilestoneLevel50)
                {
                    int? achievementId = GetAchievementIdByTypeAndCount("Number of Reviews Given", numberOfReviewsGiven);
                    System.Diagnostics.Debug.WriteLine($"Achievement ID for reviews given with count {numberOfReviewsGiven}: {achievementId}");
                    if (achievementId.HasValue && !achievementsRepository.IsAchievementUnlocked(userId, achievementId.Value))
                    {
                        System.Diagnostics.Debug.WriteLine($"Unlocking achievement ID {achievementId.Value} for user {userId}");
                        achievementsRepository.UnlockAchievement(userId, achievementId.Value);
                    }
                }

                if (numberOfReviewsReceived == MilestoneLevel1 || numberOfReviewsReceived == MilestoneLevel5 || numberOfReviewsReceived == MilestoneLevel10 || numberOfReviewsReceived == MilestoneLevel50)
                {
                    int? achievementId = GetAchievementIdByTypeAndCount("Number of Reviews Received", numberOfReviewsReceived);
                    System.Diagnostics.Debug.WriteLine($"Achievement ID for reviews received with count {numberOfReviewsReceived}: {achievementId}");
                    if (achievementId.HasValue && !achievementsRepository.IsAchievementUnlocked(userId, achievementId.Value))
                    {
                        System.Diagnostics.Debug.WriteLine($"Unlocking achievement ID {achievementId.Value} for user {userId}");
                        achievementsRepository.UnlockAchievement(userId, achievementId.Value);
                    }
                }

                if (numberOfPosts == MilestoneLevel1 || numberOfPosts == MilestoneLevel5 || numberOfPosts == MilestoneLevel10 || numberOfPosts == MilestoneLevel50)
                {
                    int? achievementId = GetAchievementIdByTypeAndCount("Number of Posts", numberOfPosts);
                    System.Diagnostics.Debug.WriteLine($"Achievement ID for posts with count {numberOfPosts}: {achievementId}");
                    if (achievementId.HasValue && !achievementsRepository.IsAchievementUnlocked(userId, achievementId.Value))
                    {
                        System.Diagnostics.Debug.WriteLine($"Unlocking achievement ID {achievementId.Value} for user {userId}");
                        achievementsRepository.UnlockAchievement(userId, achievementId.Value);
                    }
                }
                if (yearsOfActivity == MilestoneLevel1 || yearsOfActivity == MilestoneLevel2 || yearsOfActivity == MilestoneLevel3 || yearsOfActivity == MilestoneLevel4)
                {
                    int? achievementId = GetAchievementIdByTypeAndCount("Years of Activity", yearsOfActivity);
                    System.Diagnostics.Debug.WriteLine($"Achievement ID for years of activity with count {yearsOfActivity}: {achievementId}");
                    if (achievementId.HasValue && !achievementsRepository.IsAchievementUnlocked(userId, achievementId.Value))
                    {
                        System.Diagnostics.Debug.WriteLine($"Unlocking achievement ID {achievementId.Value} for user {userId}");
                        achievementsRepository.UnlockAchievement(userId, achievementId.Value);
                    }
                }

                if (isDeveloper)
                {
                    int? achievementId = GetAchievementIdByTypeAndCount("Developer", MilestoneLevel1);
                    if (achievementId.HasValue && !achievementsRepository.IsAchievementUnlocked(userId, achievementId.Value))
                    {
                        achievementsRepository.UnlockAchievement(userId, achievementId.Value);
                    }
                }
            }
            catch (RepositoryException exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {exception.Message}");
            }
        }

        public void RemoveAchievement(int userId, int achievementId)
        {
            try
            {
                achievementsRepository.RemoveAchievement(userId, achievementId);
            }
            catch (RepositoryException exception)
            {
                throw new ServiceException("Error removing achievement.", exception);
            }
        }

        public List<Achievement> GetUnlockedAchievementsForUser(int userId)
        {
            try
            {
                return achievementsRepository.GetUnlockedAchievementsForUser(userId);
            }
            catch (RepositoryException exception)
            {
                throw new ServiceException("Error retrieving unlocked achievements for user.", exception);
            }
        }

        public List<Achievement> GetAllAchievements()
        {
            try
            {
                return achievementsRepository.GetAllAchievements();
            }
            catch (RepositoryException exception)
            {
                throw new ServiceException("Error retrieving unlocked achievements for user.", exception);
            }
        }

        public AchievementUnlockedData GetUnlockedDataForAchievement(int userId, int achievementId)
        {
            try
            {
                return achievementsRepository.GetUnlockedDataForAchievement(userId, achievementId);
            }
            catch (RepositoryException exception)
            {
                throw new ServiceException("Error retrieving unlocked data for achievement.", exception);
            }
        }

        public List<AchievementWithStatus> GetAchievementsWithStatusForUser(int userId)
        {
            try
            {
                return achievementsRepository.GetAchievementsWithStatusForUser(userId);
            }
            catch (RepositoryException exception)
            {
                throw new ServiceException("Error retrieving achievements with status for user.", exception);
            }
        }

        public int? GetAchievementIdByTypeAndCount(string type, int count)
        {
            if (type == "Friendships")
            {
                if (count == MilestoneLevel1)
                {
                    return achievementsRepository.GetAchievementIdByName("FRIENDSHIP1");
                }
                else if (count == MilestoneLevel5)
                {
                    return achievementsRepository.GetAchievementIdByName("FRIENDSHIP2");
                }
                else if (count == MilestoneLevel10)
                {
                    return achievementsRepository.GetAchievementIdByName("FRIENDSHIP3");
                }
                else if (count == MilestoneLevel50)
                {
                    return achievementsRepository.GetAchievementIdByName("FRIENDSHIP4");
                }
                else if (count == MilestoneLevel100)
                {
                    return achievementsRepository.GetAchievementIdByName("FRIENDSHIP5");
                }
            }
            else if (type == "Owned Games")
            {
                if (count == MilestoneLevel1)
                {
                    return achievementsRepository.GetAchievementIdByName("OWNEDGAMES1");
                }
                else if (count == MilestoneLevel5)
                {
                    return achievementsRepository.GetAchievementIdByName("OWNEDGAMES2");
                }
                else if (count == MilestoneLevel10)
                {
                    return achievementsRepository.GetAchievementIdByName("OWNEDGAMES3");
                }
                else if (count == MilestoneLevel50)
                {
                    return achievementsRepository.GetAchievementIdByName("OWNEDGAMES4");
                }
            }
            else if (type == "Sold Games")
            {
                if (count == MilestoneLevel1)
                {
                    return achievementsRepository.GetAchievementIdByName("SOLDGAMES1");
                }
                else if (count == MilestoneLevel5)
                {
                    return achievementsRepository.GetAchievementIdByName("SOLDGAMES2");
                }
                else if (count == MilestoneLevel10)
                {
                    return achievementsRepository.GetAchievementIdByName("SOLDGAMES3");
                }
                else if (count == MilestoneLevel50)
                {
                    return achievementsRepository.GetAchievementIdByName("SOLDGAMES4");
                }
            }
            else if (type == "Number of Reviews Given")
            {
                if (count == MilestoneLevel1)
                {
                    return achievementsRepository.GetAchievementIdByName("REVIEW1");
                }
                else if (count == MilestoneLevel5)
                {
                    return achievementsRepository.GetAchievementIdByName("REVIEW2");
                }
                else if (count == MilestoneLevel10)
                {
                    return achievementsRepository.GetAchievementIdByName("REVIEW3");
                }
                else if (count == MilestoneLevel50)
                {
                    return achievementsRepository.GetAchievementIdByName("REVIEW4");
                }
            }
            else if (type == "Number of Reviews Received")
            {
                if (count == MilestoneLevel1)
                {
                    return achievementsRepository.GetAchievementIdByName("REVIEWR1");
                }
                else if (count == MilestoneLevel5)
                {
                    return achievementsRepository.GetAchievementIdByName("REVIEWR2");
                }
                else if (count == MilestoneLevel10)
                {
                    return achievementsRepository.GetAchievementIdByName("REVIEWR3");
                }
                else if (count == MilestoneLevel50)
                {
                    return achievementsRepository.GetAchievementIdByName("REVIEWR4");
                }
            }
            else if (type == "Years of Activity")
            {
                if (count == MilestoneLevel1)
                {
                    return achievementsRepository.GetAchievementIdByName("ACTIVITY1");
                }
                else if (count == MilestoneLevel2)
                {
                    return achievementsRepository.GetAchievementIdByName("ACTIVITY2");
                }
                else if (count == MilestoneLevel3)
                {
                    return achievementsRepository.GetAchievementIdByName("ACTIVITY3");
                }
                else if (count == MilestoneLevel4)
                {
                    return achievementsRepository.GetAchievementIdByName("ACTIVITY4");
                }
            }
            else if (type == "Number of Posts")
            {
                if (count == MilestoneLevel1)
                {
                    return achievementsRepository.GetAchievementIdByName("POSTS1");
                }
                else if (count == MilestoneLevel5)
                {
                    return achievementsRepository.GetAchievementIdByName("POSTS2");
                }
                else if (count == MilestoneLevel10)
                {
                    return achievementsRepository.GetAchievementIdByName("POSTS3");
                }
                else if (count == MilestoneLevel50)
                {
                    return achievementsRepository.GetAchievementIdByName("POSTS4");
                }
            }
            else if (type == "Developer")
            {
                if (count == MilestoneLevel1)
                {
                    return achievementsRepository.GetAchievementIdByName("DEVELOPER");
                }
            }

            return null;
        }

        public int GetPointsForUnlockedAchievement(int userId, int achievementId)
        {
            try
            {
                if (achievementsRepository.IsAchievementUnlocked(userId, achievementId))
                {
                    List<Achievement> achievements = achievementsRepository.GetAllAchievements();
                    Achievement foundAchievement = null;

                    foreach (var achievement in achievements)
                    {
                        if (achievement.AchievementId == achievementId)
                        {
                            foundAchievement = achievement;
                            break; // stop loop once found
                        }
                    }

                    if (foundAchievement != null)
                    {
                        return foundAchievement.Points;
                    }
                }

                throw new ServiceException("Achievement is not unlocked or does not exist.");
            }
            catch (RepositoryException exception)
            {
                throw new ServiceException("Error retrieving points for unlocked achievement.", exception);
            }
        }
   }
}
