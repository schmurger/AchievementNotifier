using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;

namespace AchievementNotifier.Parsers.Tenoke
{
    public class TenokeAchievementParser : TenokeConfigParser
    {

        private static string imagesZipFile = "icons.zip";
        private static string imagesDirectory = "icons";

        
        public TenokeAchievementParser(string processFileName, string gameDirectory, String configFile)
        {
            this.processFileName = processFileName;
            this.gameDirectory = gameDirectory;
            this.configFile = configFile;
            this.userStatsFile = buildStatsFilePath();
            this.extractedImagesDirectory = Path.Combine(Path.GetDirectoryName(configFile), imagesDirectory);

            
            readConfigFile();
            readUserStatsFile();
            extractAchievementIcons();
            populateAchievementsView();
            
            Debug.WriteLine("Initialized TenokeAchievementParser");
        }

        private GameItem getGameMenuItem()
        {
            GameItem gameItem = new GameItem();

            gameItem.id = processFileName;
            gameItem.name = new DirectoryInfo(gameDirectory).Name;
            gameItem.icon = Path.Combine(extractedImagesDirectory, $"{gameItem.name}.png");
            
            //Icon.ExtractAssociatedIcon(processFileName).ToBitmap().Save(gameItem.icon, System.Drawing.Imaging.ImageFormat.Png);
            return gameItem;
        }

        private void populateAchievementsView()
        {

            List<AchievementItem> achievementList = new List<AchievementItem>();
            foreach (Achievement achievement in achievements.Values)
            {
                AchievementItem AchievementItem = new AchievementItem();
                AchievementItem.id = achievement.id;
                AchievementItem.icon = achievement.achieved ? achievement.icon : achievement.iconGray;
                AchievementItem.name = achievement.name;
                AchievementItem.description = achievement.description;

                if (achievement.progressMax > 0 && achievement.progressMin > 0)
                {
                    AchievementItem.percentage = (int)(achievement.progressMax / achievement.progressMin * 100);
                    AchievementItem.percentageText = $"{AchievementItem.percentage}%";
                    AchievementItem.progress = $"{(int)achievement.progressMin}/{(int)achievement.progressMax}";
                    AchievementItem.progressVisible = true;
                }

                if (achievement.achieved)
                {
                    AchievementItem.achievedAt = DateTimeOffset.FromUnixTimeSeconds(achievement.timestamp).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                }
                achievementList.Add(AchievementItem);   
            }
            
            GameItem gameItem = getGameMenuItem();
            MainWindow.getInstance().Add(gameItem, achievementList);
        }

        

        private void extractAchievementIcons()
        {
            String zipFile = Path.Combine(Path.GetDirectoryName(configFile), imagesZipFile);
            try
            {
                ZipFile.ExtractToDirectory(zipFile, extractedImagesDirectory, false);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Unable to extract icons {zipFile} or icons already exist in {extractedImagesDirectory}");
            }
        }
    }
}
