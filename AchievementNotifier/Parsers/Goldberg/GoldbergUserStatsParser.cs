using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchievementNotifier.Parsers.Goldberg
{
    internal class GoldbergUserStatsParser : AchievementParser
    {
        protected Dictionary<string, SteamUserProgress> steamUserProgress = new Dictionary<string, SteamUserProgress>();

        private static string statsFolder = "stats";
        private static string statOperand = "operand1";
        private static string statValue = "operation";
        private static int unlockTimeSecondsTolerance = 5;

        protected string userStatsFolder;

        public override string GetStatsFile()
        {
            return userStatsFile;
        }

        public void readStatsFile()
        {
            if (File.Exists(userStatsFile))
            { 
                steamUserProgress = FileOperations.Read<Dictionary<string, SteamUserProgress>>(userStatsFile);
            }
        }

        public string GetAchievementStatsFile(SteamAchievement steamAchievement)
        {
            if (steamAchievement.progress != null && steamAchievement.progress.value != null && steamAchievement.progress.value.GetValueOrDefault(statValue) != null)
            {
                return Path.Combine(userStatsFolder, statsFolder, steamAchievement.progress.value.GetValueOrDefault(statOperand).ToLower());
            }
            return null;
        }

        public override void DetectUnlockedAchievements(String fileName) {
            Dictionary<string, SteamUserProgress> steamUserProgress = FileOperations.Read<Dictionary<string, SteamUserProgress>>(fileName);

            foreach (KeyValuePair<string, SteamUserProgress> progress in steamUserProgress)
            {
                bool unlocked = progress.Value.earned;
                long diffTime = Math.Abs(DateTimeOffset.Now.ToUnixTimeSeconds() - progress.Value.earned_time);

                if (unlocked && diffTime < unlockTimeSecondsTolerance)
                {
                    Achievement achievement = achievements.GetValueOrDefault(progress.Key);
                    achievement.timestamp = progress.Value.earned_time;
                    achievement.achieved = unlocked;

                    if (!notifiedAchievements.Contains(achievement.id))
                    {
                        CreateNotification(achievement);
                        MainWindow.getInstance().UpdateAchievement(processFileName, achievement);
                    }
                    else
                    {
                        notifiedAchievements.Add(achievement.id);
                    }

                }
            }
        }
    }
}
