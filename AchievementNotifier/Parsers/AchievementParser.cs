using System;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using Microsoft.UI.Xaml.Controls;
using System.Drawing;

namespace AchievementNotifier.Parsers
{


    public abstract class AchievementParser
    {
        protected string userStatsFile;
        protected string processFileName;
        protected string gameDirectory;
        protected string extractedImagesDirectory;
        protected string configFile;
        protected Dictionary<String, Achievement> achievements = new Dictionary<String, Achievement>();
        protected HashSet<string> notifiedAchievements = new HashSet<string>();

        private static string NOTIFICATION_SOUND = $"{System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\Assets\\notification.wav";
        private static string NOTIFICATION_XML =
            @"<toast launch='conversationId=9813'>
                <visual>
                    <binding template='ToastImageAndText02'>
                        <image id='1' src='{0}'/>
                        <text id='1'>{1}</text>
                        <text id='2'>{2}</text>
                    </binding>
                </visual>
                <audio src='file:///{3}'/>
            </toast>";
       

        public abstract void DetectUnlockedAchievements(String filePath);

        public abstract String GetStatsFile();

      
        public void CreateNotification(Achievement achievement)
        {
            Console.WriteLine($"Unlocked {achievement.name}");
            XmlDocument tileXml = new XmlDocument();
            tileXml.LoadXml(string.Format(NOTIFICATION_XML, new string[] { achievement.icon, achievement.name, achievement.description, NOTIFICATION_SOUND }));
            var toast = new ToastNotification(tileXml);
            ToastNotificationManager.CreateToastNotifier("AchievementNotifier").Show(toast);
        }

        public void populateAchievementsView()
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
                    AchievementItem.percentage = (int)(achievement.progressMin / achievement.progressMax * 100);
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

        private GameItem getGameMenuItem()
        {
            GameItem gameItem = new GameItem();

            gameItem.id = processFileName;
            gameItem.name = new DirectoryInfo(gameDirectory).Name;
            gameItem.icon = Path.Combine(extractedImagesDirectory, $"{gameItem.name}.png");

            Icon.ExtractAssociatedIcon(processFileName).ToBitmap().Save(gameItem.icon, System.Drawing.Imaging.ImageFormat.Png);
            return gameItem;
        }
    }
}
