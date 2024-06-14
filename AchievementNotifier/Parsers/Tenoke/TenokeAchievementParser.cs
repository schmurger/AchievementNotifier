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
