using System;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using System.Reflection;

namespace AchievementNotifier.Parsers
{


    public abstract class AchievementParser
    {
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
       

        public abstract void DetectUnlockedAchievements(String fileContents);

        public abstract String GetStatsFile();

      
        public void CreateNotification(Achievement achievement)
        {
            Console.WriteLine($"Unlocked {achievement.name}");
            XmlDocument tileXml = new XmlDocument();
            tileXml.LoadXml(string.Format(NOTIFICATION_XML, new string[] { achievement.icon, achievement.name, achievement.description, NOTIFICATION_SOUND }));
            var toast = new ToastNotification(tileXml);
            ToastNotificationManager.CreateToastNotifier("AchievementNotifier").Show(toast);
        }
    }
}
