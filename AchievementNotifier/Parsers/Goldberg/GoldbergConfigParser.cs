using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
                achievement.name = steamAchievement.displayName.GetValueOrDefault("english");
                achievement.description = steamAchievement.description.GetValueOrDefault("english");
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

       
    }
}
