using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;


namespace AchievementNotifier.Parsers.Tenoke
{
    public class TenokeUserStatsParser : AchievementParser
    {
       

        private static Regex statsPattern = new Regex(@"\[STATS\]");
        private static Regex achievedPattern = new Regex(@"\[ACHIEVEMENTS\]");
        private static Regex achStatPattern = new Regex(@"""(.*)_Stat""\s+=\s+(.*)");
        private static Regex achievementStatLine = new Regex(@"""(.*)""\s*=\s*{unlocked\s*=\s*([a-z]+)\s*,\s*time\s*=\s*([0-9]+)", RegexOptions.IgnoreCase);
        private static int unlockTimeSecondsTolerance = 5;
        protected static String statsFilePattern = @"SteamData\user_stats.ini";

        public override String GetStatsFile()
        {
            return userStatsFile;
        }

        protected string buildStatsFilePath()
        {
            return Path.Combine(Path.GetDirectoryName(processFileName), statsFilePattern);
        }

        public void readUserStatsFile()
        {
            if (string.IsNullOrEmpty(userStatsFile)) return;

            string[] lines = FileOperations.readFile(userStatsFile);
            foreach (string line in lines)
            {
                readStatLine(line);
                readAchievedLine(line);
            }
        }

        private void readStatLine(string line)
        {

            Match achStatMatch = achStatPattern.Match(line);
            if (achStatMatch.Success)
            {
                String id = achStatMatch.Groups[1].Value;
                String value = achStatMatch.Groups[2].Value;
                float.TryParse(value, out achievements.GetValueOrDefault(id).progressMin);
                Console.WriteLine($"{id}, progress={value}");
            }

        }

        private void readAchievedLine(string line)
        {

            Match achievementStatMatch = achievementStatLine.Match(line);
            if (achievementStatMatch.Success)
            {
                String id = achievementStatMatch.Groups[1].Value;
                Achievement achievement = achievements.GetValueOrDefault(id);
                bool.TryParse(achievementStatMatch.Groups[2].Value, out achievement.achieved);
                long.TryParse(achievementStatMatch.Groups[3].Value, out achievement.timestamp);
                Debug.WriteLine($"{id}, unlocked={achievementStatMatch.Groups[2].Value}, timestamp={achievementStatMatch.Groups[3].Value}");
            }
        }

        public override void DetectUnlockedAchievements(String fileName)
        {
            List<Achievement> unlockedAchievements = new List<Achievement>();
            string[] lines = FileOperations.readFile(fileName);

            foreach (string line in lines)
            {
                Debug.WriteLine($"Checking stats line {line}");
                Match match = achievementStatLine.Match(line);

                if (match.Success)
                {
                    if (match.Groups.Count == 4)
                    {
                        String id = match.Groups[1].Value;
                        bool unlocked = false;
                        bool.TryParse(match.Groups[2].Value, out unlocked);
                        long achievedTime;
                        long.TryParse(match.Groups[3].Value, out achievedTime);
                        long diffTime = Math.Abs(DateTimeOffset.Now.ToUnixTimeSeconds() - achievedTime);

                        Debug.WriteLine($"{id}, Unlocked = {unlocked}, diffTime = {diffTime}");
                        if (unlocked && diffTime < unlockTimeSecondsTolerance)
                        {
                            Achievement achievement = achievements.GetValueOrDefault(id);
                            achievement.timestamp = achievedTime;
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

    }
}
