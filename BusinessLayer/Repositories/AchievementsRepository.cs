﻿using System.Data;
using System.Diagnostics;
using BusinessLayer.Data;
using BusinessLayer.Models;
using BusinessLayer.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using BusinessLayer.Exceptions;

namespace BusinessLayer.Repositories
{
    public class AchievementsRepository : IAchievementsRepository
    {
        private readonly IDataLink dataLink;

        public AchievementsRepository(IDataLink datalink)
        {
            dataLink = datalink ?? throw new ArgumentNullException(nameof(datalink));
        }

        public void InsertAchievements()
        {
            try
            {
                dataLink.ExecuteNonQuery("InsertAchievements");
                System.Diagnostics.Debug.WriteLine("InsertAchievements stored procedure executed successfully.");
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred while inserting achievements.", ex);
            }
        }

        public bool IsAchievementsTableEmpty()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Executing SQL query to check if achievements table is empty...");
                var result = dataLink.ExecuteScalar<int>("IsAchievementsTableEmpty");
                System.Diagnostics.Debug.WriteLine($"Number of achievements in table: {result}");
                return result == 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unexpected error while checking if achievements table is empty: {ex.Message}");
                throw new RepositoryException("An unexpected error occurred while checking if achievements table is empty.", ex);
            }
        }

        public void UpdateAchievementIconUrl(int points, string iconUrl)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@points", points),
                    new SqlParameter("@iconUrl", iconUrl)
                };
                dataLink.ExecuteNonQuery("UpdateAchievementIcon", parameters);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unexpected error while updating achievement icon URL: {ex.Message}");
                throw new RepositoryException("An unexpected error occurred while updating achievement icon URL.", ex);
            }
        }

        public List<Achievement> GetAllAchievements()
        {
            try
            {
                var dataTable = dataLink.ExecuteReader("GetAllAchievements");
                return MapDataTableToAchievements(dataTable);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving achievements.", ex);
            }
        }

        public List<Achievement> GetUnlockedAchievementsForUser(int userId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@userId", userId)
                };
                var dataTable = dataLink.ExecuteReader("GetUnlockedAchievements", parameters);
                return MapDataTableToAchievements(dataTable);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving unlocked achievements.", ex);
            }
        }

        public void UnlockAchievement(int userId, int achievementId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@userId", userId),
                    new SqlParameter("@achievementId", achievementId)
                };
                dataLink.ExecuteNonQuery("UnlockAchievement", parameters);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred while unlocking achievement.", ex);
            }
        }

        public void RemoveAchievement(int userId, int achievementId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@userId", userId),
                    new SqlParameter("@achievementId", achievementId)
                };
                dataLink.ExecuteNonQuery("RemoveAchievement", parameters);
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred while removing achievement.", ex);
            }
        }

        public AchievementUnlockedData GetUnlockedDataForAchievement(int userId, int achievementId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId),
                    new SqlParameter("@achievement_id", achievementId)
                };
                var dataTable = dataLink.ExecuteReader("GetUnlockedDataForAchievement", parameters);
                return dataTable.Rows.Count > 0 ? MapDataRowToAchievementUnlockedData(dataTable.Rows[0]) : null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw new RepositoryException("An unexpected error occurred while retrieving achievement data.", ex);
            }
        }

        public bool IsAchievementUnlocked(int userId, int achievementId)
        {
            try
            {
                var parameters = new SqlParameter[]
            {
                new SqlParameter("@user_id", userId),
                new SqlParameter("@achievement_id", achievementId)
            };

                int? result = dataLink.ExecuteScalar<int>("IsAchievementUnlocked", parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unexpected error during ExecuteScalar operation: {ex.Message}");
                throw new RepositoryException("Error checking if achievement is unlocked.", ex);
            }
        }

        public List<AchievementWithStatus> GetAchievementsWithStatusForUser(int userId)
        {
            try
            {
                var allAchievements = GetAllAchievements();
                var achievementsWithStatus = new List<AchievementWithStatus>();

                foreach (var achievement in allAchievements)
                {
                    var isUnlocked = IsAchievementUnlocked(userId, achievement.AchievementId);
                    var unlockedData = GetUnlockedDataForAchievement(userId, achievement.AchievementId);
                    achievementsWithStatus.Add(new AchievementWithStatus
                    {
                        Achievement = achievement,
                        IsUnlocked = isUnlocked,
                        UnlockedDate = unlockedData?.UnlockDate
                    });
                }

                return achievementsWithStatus;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw new RepositoryException("Error retrieving achievements with status for user.", ex);
            }
        }

        public int GetNumberOfSoldGames(int userId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                new SqlParameter("@user_id", userId)
                };

                var result = dataLink.ExecuteScalar<int>("GetNumberOfSoldGames", parameters);
                return result;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving number of sold games.", ex);
            }
        }

        public int GetFriendshipCount(int userId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@user_id", userId)
                };
                return dataLink.ExecuteScalar<int>("GetFriendshipCountForUser", parameters);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Unexpected Error: {ex.Message}");
                throw new RepositoryException("An unexpected error occurred while retrieving friendship count.", ex);
            }
        }

        public int GetNumberOfOwnedGames(int userId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                new SqlParameter("@user_id", userId)
                };

                var result = dataLink.ExecuteScalar<int>("GetNumberOfOwnedGames", parameters);
                return result;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving number of owned games.", ex);
            }
        }

        public int GetNumberOfReviewsGiven(int userId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                new SqlParameter("@user_id", userId)
                };

                var result = dataLink.ExecuteScalar<int>("GetNumberOfReviewsGiven", parameters);
                return result;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving number of reviews given.", ex);
            }
        }

        public int GetNumberOfReviewsReceived(int userId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                new SqlParameter("@user_id", userId)
                };

                var result = dataLink.ExecuteScalar<int>("GetNumberOfReviewsReceived", parameters);
                return result;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving number of reviews received.", ex);
            }
        }

        public int GetNumberOfPosts(int userId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                new SqlParameter("@user_id", userId)
                };

                var result = dataLink.ExecuteScalar<int>("GetNumberOfPosts", parameters);
                return result;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving number of posts.", ex);
            }
        }

        public int GetYearsOfAcftivity(int userId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
            new SqlParameter("@user_id", userId)
                };

                var createdAt = dataLink.ExecuteScalar<DateTime>("GetUserCreatedAt", parameters);
                var yearsOfActivity = DateTime.Now.Year - createdAt.Year;

                // Adjust for the case where the user hasn't completed the current year
                if (DateTime.Now.DayOfYear < createdAt.DayOfYear)
                {
                    yearsOfActivity--;
                }

                return yearsOfActivity;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving years of activity.", ex);
            }
        }

        public int? GetAchievementIdByName(string achievementName)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
            new SqlParameter("@achievementName", achievementName)
                };

                var result = dataLink.ExecuteScalar<int?>("GetAchievementIdByName", parameters);
                System.Diagnostics.Debug.WriteLine($"Achievement ID for name {achievementName}: {result}");
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unexpected error while retrieving achievement ID: {ex.Message}");
                throw new RepositoryException("An unexpected error occurred while retrieving achievement ID.", ex);
            }
        }

        public bool IsUserDeveloper(int userId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
            new SqlParameter("@user_id", userId)
                };

                var result = dataLink.ExecuteScalar<bool>("IsUserDeveloper", parameters);
                return result;
            }
            catch (Exception ex)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving developer status.", ex);
            }
        }
        private static List<Achievement> MapDataTableToAchievements(DataTable dataTable)
        {
            return dataTable.AsEnumerable().Select(MapDataRowToAchievement).ToList();
        }

        private static Achievement MapDataRowToAchievement(DataRow row)
        {
            return new Achievement
            {
                AchievementId = Convert.ToInt32(row["achievement_id"]),
                AchievementName = row["achievement_name"].ToString() ?? string.Empty,
                Description = row["description"].ToString() ?? string.Empty,
                AchievementType = row["achievement_type"].ToString() ?? string.Empty,
                Points = Convert.ToInt32(row["points"]),
                Icon = row["icon_url"].ToString()
            };
        }

        private static AchievementUnlockedData MapDataRowToAchievementUnlockedData(DataRow row)
        {
            return new AchievementUnlockedData
            {
                AchievementName = row["AchievementName"].ToString() ?? string.Empty,
                AchievementDescription = row["AchievementDescription"].ToString() ?? string.Empty,
                UnlockDate = row["UnlockDate"] != DBNull.Value ? Convert.ToDateTime(row["UnlockDate"]) : (DateTime?)null
            };
        }
    }
}
