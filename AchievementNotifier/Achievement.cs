using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Identity.Core;

namespace AchievementNotifier
{
    public class Achievement
    {
        public String id;
        public String name;
        public String description;
        public String icon;
        public String iconGray;
        public Boolean hidden = false;
        public Boolean achieved = false;
        public long timestamp = 0;
        public float progressMin = 0;
        public float progressMax = 0;

        public Achievement() { }

        public Achievement(string name, string description, string icon)
        {
            this.name = name;
            this.description = description;
            this.icon = icon;
        }

        public Achievement(string id, Boolean achieved, long timestamp)
        {
            this.id = id;   
            this.achieved = achieved;
            this.timestamp = timestamp;
        }
    }

    [DataContract]
    public class AchievementItem
    {
        [DataMember]
        public String id;
        [DataMember]
        public String name;
        [DataMember]
        public String description;
        [DataMember]
        public String icon;
        [DataMember]
        public String achievedAt = "";
        [DataMember]
        public Boolean progressVisible = false;
        [DataMember]
        public String progress = "";
        [DataMember]
        public int percentage = 0;
        [DataMember]
        public String percentageText = "";
        public AchievementItem() { }
    }

    public class GameItem
    {
        [DataMember]
        public string id;
        [DataMember]
        public string name;
        [DataMember]
        public string icon;
        public GameItem() { }
    }

    [DataContract]
    public class GameView
    {
        [DataMember]
        public GameItem gameItem;
        [DataMember]
        public List<AchievementItem> achievementItems;
        public GameView() { }
        public GameView(GameItem gameItem, List<AchievementItem> achievementItems)
        {
            this.gameItem = gameItem;
            this.achievementItems = achievementItems;
        }
    }
}
