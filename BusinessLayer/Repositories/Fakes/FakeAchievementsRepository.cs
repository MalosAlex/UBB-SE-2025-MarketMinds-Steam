using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Models;
using BusinessLayer.Exceptions;

public class FakeAchievementsRepository : IAchievementsRepository
{
    public bool ThrowOnInsertAchievements { get; set; }
    public bool ThrowOnGetAllAchievements { get; set; }
    public bool ThrowOnGetAchievementsWithStatus { get; set; } = false;
    public bool ThrowOnUpdateIconUrl { get; set; } = false;
    public bool ThrowOnGetNumberOfSoldGames { get; set; } = false;
    public bool ThrowOnRemoveAchievement { get; set; } = false;
    public bool ThrowOnGetUnlockedAchievements { get; set; } = false;
    public bool ThrowOnGetUnlockedData { get; set; }
    public bool ThrowOnIsUnlocked { get; set; }
    public bool InsertAchievementsCalled { get; private set; }
    public List<(int points, string url)> UpdatedIcons { get; } = new();
    public List<AchievementWithStatus> AchievementsWithStatusToReturn { get; set; } = new();
    public List<Achievement> AllAchievementsToReturn { get; set; } = new();
    public AchievementUnlockedData UnlockedDataToReturn { get; set; }

    public bool AchievementsTableIsEmpty { get; set; } = true;
    public int NumberOfOwnedGames { get; set; }
    public int NumberOfSoldGames { get; set; }
    public int NumberOfReviewsGiven { get; set; }
    public int NumberOfReviewsReceived { get; set; }
    public int NumberOfFriends { get; set; }
    public int NumberOfPosts { get; set; }
    public Dictionary<string, Dictionary<int, int?>> AchievementIds { get; set; } = new();
    public List<(int userId, int achievementId)> UnlockedAchievements { get; } = new();
    public List<Achievement> UnlockedAchievementsToReturn { get; set; } = new();
    public void SetAchievementId(string category, int count, int? id)
    {
        if (!AchievementIds.ContainsKey(category))
        {
            AchievementIds[category] = new Dictionary<int, int?>();
        }
        AchievementIds[category][count] = id;
    }
    public void InsertAchievements()
    {
        if (ThrowOnInsertAchievements)
        {
            throw new RepositoryException("Insert fail");
        }

        InsertAchievementsCalled = true;
    }

    public bool IsAchievementsTableEmpty() => AchievementsTableIsEmpty;
    public void UpdateAchievementIconUrl(int points, string iconUrl)
    {
        if (ThrowOnUpdateIconUrl)
        {
            throw new RepositoryException("Fake update icon failure");
        }

        UpdatedIcons.Add((points, iconUrl));
    }

    public List<Achievement> GetAllAchievements()
    {
        if (ThrowOnGetAllAchievements)
        {
            throw new RepositoryException("GetAll failed");
        }

        return AllAchievementsToReturn;
    }

    public List<Achievement> GetUnlockedAchievementsForUser(int userId)
    {
        if (ThrowOnGetUnlockedAchievements)
        {
            throw new RepositoryException("Simulated failure");
        }

        return UnlockedAchievementsToReturn;
    }

    public void UnlockAchievement(int userId, int achievementId)
    {
        UnlockedAchievements.Add((userId, achievementId));
    }

    public void RemoveAchievement(int userId, int achievementId)
    {
        if (ThrowOnRemoveAchievement)
        {
            throw new RepositoryException("Fake remove fail");
        }
    }
    public AchievementUnlockedData GetUnlockedDataForAchievement(int userId, int achievementId)
    {
        if (ThrowOnGetUnlockedData)
        {
            throw new RepositoryException("Fake exception");
        }

        return UnlockedDataToReturn;
    }
    public bool IsAchievementUnlocked(int userId, int achievementId)
    {
        if (ThrowOnIsUnlocked)
        {
            throw new RepositoryException("Simulated DB error");
        }

        return UnlockedAchievements.Any(a => a.userId == userId && a.achievementId == achievementId);
    }

    public List<AchievementWithStatus> GetAchievementsWithStatusForUser(int userId)
    {
        if (ThrowOnGetAchievementsWithStatus)
        {
            throw new RepositoryException("Simulated repo failure");
        }

        return AchievementsWithStatusToReturn;
    }

    public int GetNumberOfSoldGames(int userId)
    {
        if (ThrowOnGetNumberOfSoldGames)
        {
            throw new RepositoryException("Simulated failure");
        }

        return NumberOfSoldGames;
    }
    public int GetFriendshipCount(int userId) => NumberOfFriends;
    public int GetNumberOfOwnedGames(int userId) => NumberOfOwnedGames;
    public int GetNumberOfReviewsGiven(int userId) => NumberOfReviewsGiven;
    public int GetNumberOfReviewsReceived(int userId) => NumberOfReviewsReceived;
    public int GetNumberOfPosts(int userId) => NumberOfPosts;
    // public int GetYearsOfAcftivity(int userId) => YearsOfAcftivity;
    public Func<int, int> GetYearsOfAcftivity { get; set; } = _ => 0;
    int IAchievementsRepository.GetYearsOfAcftivity(int userId) => GetYearsOfAcftivity(userId);
    public Func<int, bool> IsUserDeveloper { get; set; } = _ => false;
    bool IAchievementsRepository.IsUserDeveloper(int userId) => IsUserDeveloper(userId);

    public int? GetAchievementIdByName(string name)
    {
        foreach (var category in AchievementIds)
        {
            foreach (var pair in category.Value)
            {
                var expectedName = category.Key switch
                {
                    "Friendships" => $"FRIENDSHIP{pair.Key}",
                    "Owned Games" => $"OWNEDGAMES{pair.Key}",
                    "Sold Games" => $"SOLDGAMES{pair.Key}",
                    "Number of Reviews Given" => $"REVIEW{pair.Key}",
                    "Number of Reviews Received" => $"REVIEWR{pair.Key}",
                    "Years of Activity" => $"ACTIVITY{pair.Key}",
                    "Number of Posts" => $"POSTS{pair.Key}",
                    "Developer" => "DEVELOPER",
                    _ => string.Empty
                };

                if (expectedName == name)
                {
                    return pair.Value;
                }
            }
        }
        return null;
    }
}
