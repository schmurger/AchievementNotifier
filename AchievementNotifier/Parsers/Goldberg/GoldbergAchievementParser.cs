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
        private static string APP_DATA = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static string[] GOLDBER_FOLDERS = new string[] { "Goldberg SteamEmu Saves", "GSE Saves" };
        private string gameId;
        private string appIdFilePath;

        public GoldbergAchievementParser(string processFileName, string gameDirectory, string configFile)
        {

            this.processFileName = processFileName;
            this.gameDirectory = gameDirectory;
            this.configFile = configFile;
            this.appIdFilePath = getAppIdFilePath();
            this.gameId = readAppId();
            this.userStatsFolder = getAppDataFolder();
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

        protected string getAppDataFolder()
        {
            foreach( string folder in GOLDBER_FOLDERS){
                string appDataFolder = $"{APP_DATA}\\{folder}\\{gameId}";
                if (Directory.Exists(appDataFolder)){
                    return appDataFolder;
                }
            }
            return "";
        }
    }
}
