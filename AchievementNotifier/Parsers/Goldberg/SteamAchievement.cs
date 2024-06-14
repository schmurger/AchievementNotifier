using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchievementNotifier.Parsers.Goldberg
{
    public class SteamAchievement
    {
        public int hidden { get; set; }
        public Dictionary<string, string> displayName { get; set; }
        public Dictionary<string, string> description { get; set; }
        public string icon_gray { get; set; }
        public string icon { get; set; }
        public string name { get; set; }
        public SteamProgress progress { get; set; }
    }

    public class SteamProgress
    {
        public string min_val { get; set; }
        public string max_val { get; set; }
        public Dictionary<string, string> value { get; set; }
    }

    public class SteamUserProgress
    {
        public bool earned { get; set; }
        public long earned_time { get; set; }
    }
}
