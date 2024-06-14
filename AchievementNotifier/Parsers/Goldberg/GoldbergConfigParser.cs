using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AchievementNotifier.Parsers.Goldberg
{
    internal class GoldbergConfigParser : GoldbergUserStatsParser
    {

        protected void readConfigFile()
        {
           
            if (string.IsNullOrEmpty(configFile)) return;

            List<SteamAchievement> steamAchievements = FileOperations.Read<List<SteamAchievement>>(configFile);
            foreach(SteamAchievement steamAchievement in steamAchievements)
            {
                Achievement achievement = new Achievement();
                achievement.id = steamAchievement.name;
                achievement.name = getDisplayName(steamAchievement);
                achievement.description = getDescription(steamAchievement);
                achievement.icon = Path.Combine(extractedImagesDirectory, steamAchievement.icon);
                achievement.iconGray = Path.Combine(extractedImagesDirectory, steamAchievement.icon_gray);

                if(steamAchievement.progress != null)
                {
                    float.TryParse(steamAchievement.progress.min_val, out achievement.progressMin);
                    float.TryParse(steamAchievement.progress.max_val, out achievement.progressMax);
                }

                SteamUserProgress progress = steamUserProgress.GetValueOrDefault(achievement.id);
                if (progress != null)
                {
                    achievement.achieved = progress.earned;
                    achievement.timestamp = progress.earned_time;
                    achievement.progressMin = FileOperations.readInt(GetAchievementStatsFile(steamAchievement));
                }
               
                achievements.Add(achievement.id, achievement);
            }
        }

       private string getDisplayName(SteamAchievement steamAchievement)
        {
            if (steamAchievement.displayName.ValueKind.ToString() == "String")
            {
                return steamAchievement.displayName.ToString();

            }
            else {
                return (JsonSerializer.Deserialize<Dictionary<string, string>>(steamAchievement.displayName.ToString())).GetValueOrDefault("english");

            }
           
        }

        private string getDescription(SteamAchievement steamAchievement)
        {
            if (steamAchievement.description.ValueKind.ToString() == "String")
            {
                return steamAchievement.description.ToString();
            }
            else {
                return (JsonSerializer.Deserialize<Dictionary<string, string>>(steamAchievement.description.ToString())).GetValueOrDefault("english");
            }
        }
    }
}
