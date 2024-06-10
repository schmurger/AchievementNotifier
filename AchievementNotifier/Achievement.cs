using System;
using System.Collections.Generic;

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


    public class AchievementItem
    {

        public String id;
        public String name;
        public String description;
        public String icon;
        public String achievedAt = "";
        public Boolean progressVisible = false;
        public String progress = "";
        public int percentage = 0;
        public String percentageText = "";
        public AchievementItem() { }
    }

    public class GameItem
    {

        public string id;

        public string name;

        public string icon;
        public GameItem() { }
    }


    public class GameView
    {

        public GameItem gameItem;

        public List<AchievementItem> achievementItems;
        public GameView() { }
        public GameView(GameItem gameItem, List<AchievementItem> achievementItems)
        {
            this.gameItem = gameItem;
            this.achievementItems = achievementItems;
        }
    }
}
