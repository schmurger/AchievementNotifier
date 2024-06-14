using AchievementNotifier.Parsers.Goldberg;
using AchievementNotifier.Parsers.Tenoke;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AchievementNotifier.Parsers
{
    public enum EMU_TYPE
    {
        GSE = 1,
        TENOKE = 2,
        GOLDBERG = 3,
        NOT_FOUND = 99
    }

    public class EmuDetector
    {
        public static String UE_GAME_STRING = "-Shipping";
        private static EmuDetector emuDetector = new EmuDetector();
        private Dictionary<String, AchievementParser> detectedGames = new Dictionary<String, AchievementParser>();
        public static Dictionary<EMU_TYPE, String> configFiles = new Dictionary<EMU_TYPE, string>
        {
            { EMU_TYPE.GOLDBERG ,"achievements.json" },
            { EMU_TYPE.TENOKE, "tenoke.ini" },
            { EMU_TYPE.NOT_FOUND, "nofile" }
        };

        private EmuDetector()
        {
            emuDetector = this;
        }

        public static EmuDetector getInstance()
        {
            return emuDetector;
        }

        public AchievementParser DetectEmu(String fileName)
        {
            String gameDirectory = findGameDirectory(fileName);

            foreach (KeyValuePair<EMU_TYPE, String> entry in configFiles)
            {
                try
                {
                    String configFile = FileOperations.findFile(gameDirectory, entry.Value);
                  
                    if (!string.IsNullOrEmpty(configFile))
                    {
                        return initParser(entry.Key, fileName, gameDirectory, configFile);
                        
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Couldn't find emu for {fileName}, ex={e.GetBaseException().Message}");
                }
            }

            return initParser(EMU_TYPE.NOT_FOUND, "","", "");
        }

        private bool isUEGame(String fileName)
        {
            return fileName != null && fileName.Contains(UE_GAME_STRING);    
        }

        

        private string findGameDirectory(String processFileName)
        {
            String gameDirectory = System.IO.Path.GetDirectoryName(processFileName);

            if (isUEGame(processFileName))
            {
                return System.IO.Path.GetFullPath(System.IO.Path.Combine(gameDirectory, @"..\..\..\"));
            }

            return gameDirectory;
        }

        private AchievementParser initParser(EMU_TYPE configType, String processFileName, String gameDirectory, String configFile)
        {
            switch (configType)
            {
                case EMU_TYPE.TENOKE:
                    return new TenokeAchievementParser(processFileName, gameDirectory, configFile);
                case EMU_TYPE.GOLDBERG:
                    return new GoldbergAchievementParser(processFileName, gameDirectory, configFile);
                default:
                    return null;
            }
        }

        public void checkProcessForEmu(int pid)
        {
            try
            {
                String processFileName = Process.GetProcessById(pid).MainModule.FileName;
                Console.WriteLine($"Checking process {processFileName}");
          
                if (processFileName != null && !detectedGames.ContainsKey(processFileName))
                {
                    AchievementParser parser = DetectEmu(processFileName);
                    if (parser != null)
                    {
                        detectedGames.Add(processFileName, parser);
                        Task.Run(() => new FileWatcher(parser).Start());
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine($"Couldn't find emu for {pid}");
            }
        }
    }
}
