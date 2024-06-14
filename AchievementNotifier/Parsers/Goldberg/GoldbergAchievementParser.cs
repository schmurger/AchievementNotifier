using AchievementNotifier.Parsers.Tenoke;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AchievementNotifier.Parsers.Goldberg
{
    internal class GoldbergAchievementParser : GoldbergConfigParser
    {
        private static string imagesDirectory = "img";
        private static string appIdFile = "steam_appid.txt";
        private static string steamSettings = "steam_settings";
        private static string GSE_FOLDER = "GSE Saves";
        private static string GOLDBERG_FOLDER = "Goldberg SteamEmu Saves";
        
        private string gameId;
        private string appIdFilePath;

        public GoldbergAchievementParser(string processFileName, string gameDirectory, String configFile)
        {

            this.processFileName = processFileName;
            this.gameDirectory = gameDirectory;
            this.configFile = configFile;
            this.appIdFilePath = getAppIdFilePath();
            this.gameId = readAppId();
            this.userStatsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), getEmuFolder(), gameId);
            this.userStatsFile = Path.Combine(userStatsFolder, "achievements.json");
            this.extractedImagesDirectory = Path.GetDirectoryName(configFile);

            readStatsFile();
            readConfigFile();
            populateAchievementsView();  
        }

        protected string readAppId()
        {
            return FileOperations.readString(appIdFilePath);
        }

        protected string getAppIdFilePath()
        {
            return FileOperations.findFile(gameDirectory, appIdFile);
        }
        protected string getEmuFolder()
        {
            if (appIdFilePath.Contains(steamSettings))
            {
                return GSE_FOLDER;
            }
            return GOLDBERG_FOLDER;
        }
    }
}
