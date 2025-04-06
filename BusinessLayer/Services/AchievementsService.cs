using BusinessLayer.Models;
using BusinessLayer.Repositories;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Exceptions;

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

        public GroupedAchievementsResult GetGroupedAchievementsForUser(int userId)
        {
            try
            {
                // Unlock logic (business rules)
                UnlockAchievementForUser(userId);

                // Get all achievements with status
                var all = achievementsRepository.GetAchievementsWithStatusForUser(userId);

                // Group by category
                return new GroupedAchievementsResult
                {
                    AllAchievements = all,
                    Friendships = FilterByCategory(all, "Friendships"),
                    OwnedGames = FilterByCategory(all, "Owned Games"),
                    SoldGames = FilterByCategory(all, "Sold Games"),
                    YearsOfActivity = FilterByCategory(all, "Years of Activity"),
                    NumberOfPosts = FilterByCategory(all, "Number of Posts"),
                    NumberOfReviewsGiven = FilterByCategory(all, "Number of Reviews Given"),
                    NumberOfReviewsReceived = FilterByCategory(all, "Number of Reviews Received"),
                    Developer = FilterByCategory(all, "Developer")
                };
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Error grouping achievements for user.", ex);
            }
        }

        private List<AchievementWithStatus> FilterByCategory(List<AchievementWithStatus> all, string category)
        {
            return all.Where(a => a.Achievement.AchievementType == category).ToList();
        }

        public AchievementsService(IAchievementsRepository achievementsRepository)
        {
            achievementsRepository = achievementsRepository ?? throw new ArgumentNullException(nameof(achievementsRepository));
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
                else
                {
                    System.Diagnostics.Debug.WriteLine("Achievements table is not empty. No need to insert achievements.");
                }
            }
            catch (RepositoryException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing achievements: {ex.Message}");
            }
        }

        private void UpdateAchievementIconUrls()
        {
            try
            {
                var iconUrls = new Dictionary<int, string>
                {
                    { 1, "https://t4.ftcdn.net/jpg/00/99/53/31/360_F_99533164_fpE2O6vEjnXgYhonMyYBGtGUFCLqfTWA.jpg" },
                    { 3, "https://png.pngtree.com/png-clipart/20200401/original/pngtree-gold-number-5-png-image_5330870.jpg" },
                    { 5, "https://t4.ftcdn.net/jpg/01/93/98/05/360_F_193980561_lymRkyDG6roPxmgA6x27fEaq3O3z3Mcf.jpg" },
                    { 10, "https://as1.ftcdn.net/v2/jpg/02/42/16/20/1000_F_242162042_Ve21lDSZQl3Ebb9laV1WAJrR0ls3RGAn.jpg" },
                    { 15, "https://t3.ftcdn.net/jpg/02/79/95/72/360_F_279957287_UsAVf2woGRBWekMX68LiiWpwrrVVy9bI.jpg" }
                };
                foreach (var iconUrl in iconUrls)
                {
                    System.Diagnostics.Debug.WriteLine($"Updating icon URL for points: {iconUrl.Key}, URL: {iconUrl.Value}");
                    achievementsRepository.UpdateAchievementIconUrl(iconUrl.Key, iconUrl.Value);
                }

                System.Diagnostics.Debug.WriteLine("Achievement icon URLs updated successfully.");
            }
            catch (RepositoryException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating achievement icon URLs: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
            }
        }

        public List<Achievement> GetAchievementsForUser(int userId)
        {
            try
            {
                return achievementsRepository.GetAllAchievements();
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Error retrieving achievements for user.", ex);
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

                if (numberOfFriends == 1 || numberOfFriends == 5 || numberOfFriends == 10 || numberOfFriends == 50 || numberOfFriends == 100)
                {
                    int? achievementId = GetAchievementIdByTypeAndCount("Friendships", numberOfFriends);
                    System.Diagnostics.Debug.WriteLine($"Achievement ID for friendships with count {numberOfFriends}: {achievementId}");
                    if (achievementId.HasValue && !achievementsRepository.IsAchievementUnlocked(userId, achievementId.Value))
                    {
                        System.Diagnostics.Debug.WriteLine($"Unlocking achievement ID {achievementId.Value} for user {userId}");
                        achievementsRepository.UnlockAchievement(userId, achievementId.Value);
                    }
                }

                if (numberOfOwnedGames == 1 || numberOfOwnedGames == 5 || numberOfOwnedGames == 10 || numberOfOwnedGames == 50)
                {
                    int? achievementId = GetAchievementIdByTypeAndCount("Owned Games", numberOfOwnedGames);
                    System.Diagnostics.Debug.WriteLine($"Achievement ID for owned games with count {numberOfOwnedGames}: {achievementId}");
                    if (achievementId.HasValue && !achievementsRepository.IsAchievementUnlocked(userId, achievementId.Value))
                    {
                        System.Diagnostics.Debug.WriteLine($"Unlocking achievement ID {achievementId.Value} for user {userId}");
                        achievementsRepository.UnlockAchievement(userId, achievementId.Value);
                    }
                }

                if (numberOfSoldGames == 1 || numberOfSoldGames == 5 || numberOfSoldGames == 10 || numberOfSoldGames == 50)
                {
                    int? achievementId = GetAchievementIdByTypeAndCount("Sold Games", numberOfSoldGames);
                    System.Diagnostics.Debug.WriteLine($"Achievement ID for sold games with count {numberOfSoldGames}: {achievementId}");
                    if (achievementId.HasValue && !achievementsRepository.IsAchievementUnlocked(userId, achievementId.Value))
                    {
                        System.Diagnostics.Debug.WriteLine($"Unlocking achievement ID {achievementId.Value} for user {userId}");
                        achievementsRepository.UnlockAchievement(userId, achievementId.Value);
                    }
                }

                if (numberOfReviewsGiven == 1 || numberOfReviewsGiven == 5 || numberOfReviewsGiven == 10 || numberOfReviewsGiven == 50)
                {
                    int? achievementId = GetAchievementIdByTypeAndCount("Number of Reviews Given", numberOfReviewsGiven);
                    System.Diagnostics.Debug.WriteLine($"Achievement ID for reviews given with count {numberOfReviewsGiven}: {achievementId}");
                    if (achievementId.HasValue && !achievementsRepository.IsAchievementUnlocked(userId, achievementId.Value))
                    {
                        System.Diagnostics.Debug.WriteLine($"Unlocking achievement ID {achievementId.Value} for user {userId}");
                        achievementsRepository.UnlockAchievement(userId, achievementId.Value);
                    }
                }

                if (numberOfReviewsReceived == 1 || numberOfReviewsReceived == 5 || numberOfReviewsReceived == 10 || numberOfReviewsReceived == 50)
                {
                    int? achievementId = GetAchievementIdByTypeAndCount("Number of Reviews Received", numberOfReviewsReceived);
                    System.Diagnostics.Debug.WriteLine($"Achievement ID for reviews received with count {numberOfReviewsReceived}: {achievementId}");
                    if (achievementId.HasValue && !achievementsRepository.IsAchievementUnlocked(userId, achievementId.Value))
                    {
                        System.Diagnostics.Debug.WriteLine($"Unlocking achievement ID {achievementId.Value} for user {userId}");
                        achievementsRepository.UnlockAchievement(userId, achievementId.Value);
                    }
                }

                if (numberOfPosts == 1 || numberOfPosts == 5 || numberOfPosts == 10 || numberOfPosts == 50)
                {
                    int? achievementId = GetAchievementIdByTypeAndCount("Number of Posts", numberOfPosts);
                    System.Diagnostics.Debug.WriteLine($"Achievement ID for posts with count {numberOfPosts}: {achievementId}");
                    if (achievementId.HasValue && !achievementsRepository.IsAchievementUnlocked(userId, achievementId.Value))
                    {
                        System.Diagnostics.Debug.WriteLine($"Unlocking achievement ID {achievementId.Value} for user {userId}");
                        achievementsRepository.UnlockAchievement(userId, achievementId.Value);
                    }
                }
                if (yearsOfActivity == 1 || yearsOfActivity == 2 || yearsOfActivity == 3 || yearsOfActivity == 4)
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
                    int? achievementId = GetAchievementIdByTypeAndCount("Developer", 1);
                    if (achievementId.HasValue && !achievementsRepository.IsAchievementUnlocked(userId, achievementId.Value))
                    {
                        achievementsRepository.UnlockAchievement(userId, achievementId.Value);
                    }
                }
            }
            catch (RepositoryException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        public void RemoveAchievement(int userId, int achievementId)
        {
            try
            {
                achievementsRepository.RemoveAchievement(userId, achievementId);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Error removing achievement.", ex);
            }
        }

        public List<Achievement> GetUnlockedAchievementsForUser(int userId)
        {
            try
            {
                return achievementsRepository.GetUnlockedAchievementsForUser(userId);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Error retrieving unlocked achievements for user.", ex);
            }
        }

        public List<Achievement> GetAllAchievements()
        {
            try
            {
                return achievementsRepository.GetAllAchievements();
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Error retrieving unlocked achievements for user.", ex);
            }
        }

        public AchievementUnlockedData GetUnlockedDataForAchievement(int userId, int achievementId)
        {
            try
            {
                return achievementsRepository.GetUnlockedDataForAchievement(userId, achievementId);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Error retrieving unlocked data for achievement.", ex);
            }
        }

        public List<AchievementWithStatus> GetAchievementsWithStatusForUser(int userId)
        {
            try
            {
                return achievementsRepository.GetAchievementsWithStatusForUser(userId);
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Error retrieving achievements with status for user.", ex);
            }
        }

        public int? GetAchievementIdByTypeAndCount(string type, int count)
        {
            if (type == "Friendships")
            {
                if (count == 1)
                {
                    return achievementsRepository.GetAchievementIdByName("FRIENDSHIP1");
                }
                else if (count == 5)
                {
                    return achievementsRepository.GetAchievementIdByName("FRIENDSHIP2");
                }
                else if (count == 10)
                {
                    return achievementsRepository.GetAchievementIdByName("FRIENDSHIP3");
                }
                else if (count == 50)
                {
                    return achievementsRepository.GetAchievementIdByName("FRIENDSHIP4");
                }
                else if (count == 100)
                {
                    return achievementsRepository.GetAchievementIdByName("FRIENDSHIP5");
                }
            }
            else if (type == "Owned Games")
            {
                if (count == 1)
                {
                    return achievementsRepository.GetAchievementIdByName("OWNEDGAMES1");
                }
                else if (count == 5)
                {
                    return achievementsRepository.GetAchievementIdByName("OWNEDGAMES2");
                }
                else if (count == 10)
                {
                    return achievementsRepository.GetAchievementIdByName("OWNEDGAMES3");
                }
                else if (count == 50)
                {
                    return achievementsRepository.GetAchievementIdByName("OWNEDGAMES4");
                }
            }
            else if (type == "Sold Games")
            {
                if (count == 1)
                {
                    return achievementsRepository.GetAchievementIdByName("SOLDGAMES1");
                }
                else if (count == 5)
                {
                    return achievementsRepository.GetAchievementIdByName("SOLDGAMES2");
                }
                else if (count == 10)
                {
                    return achievementsRepository.GetAchievementIdByName("SOLDGAMES3");
                }
                else if (count == 50)
                {
                    return achievementsRepository.GetAchievementIdByName("SOLDGAMES4");
                }
            }
            else if (type == "Number of Reviews Given")
            {
                if (count == 1)
                {
                    return achievementsRepository.GetAchievementIdByName("REVIEW1");
                }
                else if (count == 5)
                {
                    return achievementsRepository.GetAchievementIdByName("REVIEW2");
                }
                else if (count == 10)
                {
                    return achievementsRepository.GetAchievementIdByName("REVIEW3");
                }
                else if (count == 50)
                {
                    return achievementsRepository.GetAchievementIdByName("REVIEW4");
                }
            }
            else if (type == "Number of Reviews Received")
            {
                if (count == 1)
                {
                    return achievementsRepository.GetAchievementIdByName("REVIEWR1");
                }
                else if (count == 5)
                {
                    return achievementsRepository.GetAchievementIdByName("REVIEWR2");
                }
                else if (count == 10)
                {
                    return achievementsRepository.GetAchievementIdByName("REVIEWR3");
                }
                else if (count == 50)
                {
                    return achievementsRepository.GetAchievementIdByName("REVIEWR4");
                }
            }
            else if (type == "Years of Activity")
            {
                if (count == 1)
                {
                    return achievementsRepository.GetAchievementIdByName("ACTIVITY1");
                }
                else if (count == 2)
                {
                    return achievementsRepository.GetAchievementIdByName("ACTIVITY2");
                }
                else if (count == 3)
                {
                    return achievementsRepository.GetAchievementIdByName("ACTIVITY3");
                }
                else if (count == 4)
                {
                    return achievementsRepository.GetAchievementIdByName("ACTIVITY4");
                }
            }
            else if (type == "Number of Posts")
            {
                if (count == 1)
                {
                    return achievementsRepository.GetAchievementIdByName("POSTS1");
                }
                else if (count == 5)
                {
                    return achievementsRepository.GetAchievementIdByName("POSTS2");
                }
                else if (count == 10)
                {
                    return achievementsRepository.GetAchievementIdByName("POSTS3");
                }
                else if (count == 50)
                {
                    return achievementsRepository.GetAchievementIdByName("POSTS4");
                }
            }
            else if (type == "Developer")
            {
                if (count == 1)
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
                    var achievement = achievementsRepository.GetAllAchievements()
                        .FirstOrDefault(a => a.AchievementId == achievementId);
                    if (achievement != null)
                    {
                        return achievement.Points;
                    }
                }
                throw new ServiceException("Achievement is not unlocked or does not exist.");
            }
            catch (RepositoryException ex)
            {
                throw new ServiceException("Error retrieving points for unlocked achievement.", ex);
            }
        }
    }
}
