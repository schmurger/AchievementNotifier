using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AchievementNotifier.Parsers
{


    public abstract class AchievementParser
    {

        public abstract void DetectUnlockedAchievements(String fileContents);

        public abstract String GetStatsFile();

        public void CreateNotification(Achievement achievement)
        {
            Console.WriteLine($"Unlocked {achievement.name}");

            new ToastContentBuilder()
                 .AddArgument("action", "viewConversation")
                 .AddArgument("conversationId", achievement.id)
                 .AddText(achievement.name)
                 .AddText(achievement.description)
                 .AddAppLogoOverride(new Uri(achievement.icon))
                 .AddAudio(new Uri("ms-appx:///Assets/notification.wav"))
                 .Show();


        }
    }
}
