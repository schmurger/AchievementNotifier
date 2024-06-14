using System;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using Microsoft.UI.Xaml.Controls;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO.Pipes;
using System.Threading.Tasks;

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

        private static string NOTIFICATION_SOUND = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\Assets\\notification.wav";
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
        private static ColorMatrix colorMatrix = new ColorMatrix(new float[][]
                        {
                         new float[] {.3f, .3f, .3f, 0, 0},
                         new float[] {.59f, .59f, .59f, 0, 0},
                         new float[] {.11f, .11f, .11f, 0, 0},
                         new float[] {0, 0, 0, 1, 0},
                         new float[] {0, 0, 0, 0, 1}
                        });

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
                handleGrayIcon(achievement);
                AchievementItem.icon = achievement.achieved ? achievement.icon : achievement.iconGray;
                AchievementItem.name = achievement.name;
                AchievementItem.description = achievement.description;

                if (achievement.progressMax > 0)
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

        public void handleGrayIcon(Achievement achievement)
        {   
            if(achievement.icon == achievement.iconGray)
            {
                string extension = Path.GetExtension(achievement.iconGray);
                achievement.iconGray = achievement.iconGray.Replace(extension, $"_gray.jpg");
                MakeGrayscale(achievement.icon, achievement.iconGray);
            }
        }

        public void MakeGrayscale(string fileName, string fileNameGray)
        {
            if (File.Exists(fileNameGray)) return;

            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                Bitmap original = new Bitmap(fileStream);
                Bitmap newBitmap = new Bitmap(original.Width, original.Height);

                using (Graphics g = Graphics.FromImage(newBitmap))
                using (ImageAttributes attributes = new ImageAttributes())
                {
                    attributes.SetColorMatrix(colorMatrix);
                    g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
                    newBitmap.Save(fileNameGray, ImageFormat.Jpeg);
                }
            }
        }
    }
}
