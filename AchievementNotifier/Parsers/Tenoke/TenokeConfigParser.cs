using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AchievementNotifier.Parsers.Tenoke
{
    public class TenokeConfigParser : TenokeUserStatsParser
    {
        private static Regex achievementIdPattern = new Regex(@"\[ACHIEVEMENTS\.([a-zA-Z0-9_]+)\]", RegexOptions.IgnoreCase);
        private static Regex achievementNamePattern = new Regex(@"\[ACHIEVEMENTS\.([a-zA-Z0-9_]+)\.name\]", RegexOptions.IgnoreCase);
        private static Regex achievementDescriptionPattern = new Regex(@"\[ACHIEVEMENTS\.([a-zA-Z0-9_]+)\.desc\]", RegexOptions.IgnoreCase);
        private static Regex englishPattern = new Regex(@"english\s+=\s+""(.*)""");

        private static Dictionary<String, Regex> configFilePatterns;
        protected string extractedImagesDirectory;
        protected string configFile;

        static TenokeConfigParser()
        {
            configFilePatterns = new Dictionary<String, Regex>
                {
                { "icon",       new Regex("icon = (.*)",            RegexOptions.IgnoreCase) },
                { "iconGray",   new Regex("icon_gray = (.*)",       RegexOptions.IgnoreCase) },
                { "hidden",     new Regex("hidden = (.*)",          RegexOptions.IgnoreCase) },
                { "progressMin",new Regex("progress_min = (.*)",    RegexOptions.IgnoreCase) },
                { "progressMax",new Regex("progress_max = (.*)",    RegexOptions.IgnoreCase) }
                };
        }

        public void readConfigFile()
        {
            Console.WriteLine($"Start parsing {configFile}");
            string[] lines = FileOperations.readContents(configFile);

            Boolean idParseMode = false;
            Boolean nameParseMode = false;
            Boolean descParseMode = false;
            Achievement achievement = null;
            String currentAchievementId = null;

            foreach (string line in lines)
            {
                Match achievementIdMatch = achievementIdPattern.Match(line);
                if (achievementIdMatch.Success)
                {
                    idParseMode = true;
                    achievement = new Achievement();
                    setAchievementField("id", achievementIdMatch.Groups[1].Value, achievement);
                    continue;
                }

                if (idParseMode)
                {
                    Boolean anyMatch = false;
                    foreach (KeyValuePair<String, Regex> entry in configFilePatterns)
                    {
                        Match match = entry.Value.Match(line);
                        if (match.Success)
                        {
                            setAchievementField(entry.Key, match.Groups[1].Value, achievement);
                            anyMatch = true;
                            break;
                        }
                    }

                    if (!anyMatch)
                    {
                        achievements.Add(achievement.id, achievement);
                        idParseMode = false;
                    }
                }

                Match achievementNameMatch = achievementNamePattern.Match(line);
                if (achievementNameMatch.Success)
                {
                    nameParseMode = true;
                    currentAchievementId = achievementNameMatch.Groups[1].Value;
                    continue;
                }

                Match englishMatch = englishPattern.Match(line);
                if (nameParseMode && englishMatch.Success)
                {
                    setAchievementField("name", englishMatch.Groups[1].Value, achievements.GetValueOrDefault(currentAchievementId));
                    nameParseMode = false;
                    continue;
                }

                Match achievementDescriptionMatch = achievementDescriptionPattern.Match(line);
                if (achievementDescriptionMatch.Success)
                {
                    descParseMode = true;
                    currentAchievementId = achievementDescriptionMatch.Groups[1].Value;
                    continue;
                }

                if (descParseMode && englishMatch.Success)
                {
                    setAchievementField("description", englishMatch.Groups[1].Value, achievements.GetValueOrDefault(currentAchievementId));
                    descParseMode = false;
                    continue;
                }
            }
        }

        private void setAchievementField(String key, String value, Achievement achievement)
        {
            if (achievement == null) return;

            switch (key)
            {
                case "id":
                    achievement.id = value.Replace("\"", ""); break;
                case "name":
                    achievement.name = value.Replace("\"", ""); break;
                case "description":
                    achievement.description = value.Replace("\"", ""); break;
                case "icon":
                    achievement.icon = System.IO.Path.Combine(extractedImagesDirectory, value.Replace("\"", "")); break;
                case "iconGray":
                    achievement.iconGray = System.IO.Path.Combine(extractedImagesDirectory, value.Replace("\"", "")); break;
                case "hidden":
                    bool.TryParse(value, out achievement.hidden); break;
                case "progressMin":
                    float.TryParse(value, out achievement.progressMin); break;
                case "progressMax":
                    float.TryParse(value, out achievement.progressMax); break;
                default:
                    break;
            }
        }
    }
}
