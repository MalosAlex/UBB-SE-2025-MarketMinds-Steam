using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SteamProfile.Models
{
    public class Achievement
    {
        public int AchievementId { get; set; }

        public String AchievementName { get; set; } = string.Empty;
        public String Description { get; set; } = string.Empty;
        public String AchievementType { get; set; } = string.Empty;
        public int Points { get; set; }
        public string? Icon { get; set; }

        public bool IsValidIcon()
        {
            if (string.IsNullOrWhiteSpace(Icon)) return false;
            string pattern = @"\.(png|svg|jpg)$";
            return Regex.IsMatch(Icon, pattern, RegexOptions.IgnoreCase);
        }

        public void UpdateForm(Achievement other)
        {
            AchievementName = other.AchievementName;
            Description = other.Description;
            AchievementType = other.AchievementType;
            Points = other.Points;
            Icon = other.Icon;
        }
    }
}
