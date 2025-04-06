using BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BusinessLayer.Services.AchievementsService;

namespace BusinessLayer.Services.Interfaces
{
    public interface IAchievementsService
    {
        void InitializeAchievements();
        GroupedAchievementsResult GetGroupedAchievementsForUser(int userId);
        List<Achievement> GetAchievementsForUser(int userId);
        void UnlockAchievementForUser(int userId);
        void RemoveAchievement(int userId, int achievementId);
        List<Achievement> GetUnlockedAchievementsForUser(int userId);
        List<Achievement> GetAllAchievements();
        AchievementUnlockedData GetUnlockedDataForAchievement(int userId, int achievementId);
        List<AchievementWithStatus> GetAchievementsWithStatusForUser(int userId);
        int GetPointsForUnlockedAchievement(int userId, int achievementId);
    }
}
