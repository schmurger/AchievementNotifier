﻿using AchievementNotifier.Parsers.Tenoke;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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

    internal class EmuDetector
    {
        public static String UE_GAME_STRING = "-Shipping";

        private Dictionary<String, AchievementParser> detectedGames = new Dictionary<String, AchievementParser>();
        public static Dictionary<EMU_TYPE, String> configFiles = new Dictionary<EMU_TYPE, string>
        {
            { EMU_TYPE.GOLDBERG ,"steam_appid.txt" },
            { EMU_TYPE.GSE , "configs.app.ini"},
            { EMU_TYPE.TENOKE, "tenoke.ini" },
            { EMU_TYPE.NOT_FOUND, "nofile" }
        };

        public static Dictionary<EMU_TYPE, String> userStatsFiles = new Dictionary<EMU_TYPE, string>
        {
            { EMU_TYPE.GOLDBERG ,"steam_appid.txt" },
            { EMU_TYPE.GSE , "configs.app.ini"},
            { EMU_TYPE.TENOKE, "user_stats.ini" },
            { EMU_TYPE.NOT_FOUND, "nofile" }
        };

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
                    Debug.WriteLine($"Couldn't find emu for {gameDirectory}");
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
                        new FileWatcher(parser);
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine($"Couldn't find emu for {pid}");
            }
        }
    }
}