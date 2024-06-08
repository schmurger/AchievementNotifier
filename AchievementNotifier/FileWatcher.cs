using AchievementNotifier.Parsers;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace AchievementNotifier
{
    internal class FileWatcher
    {
        private AchievementParser parser;
        private FileSystemWatcher watcher;
        private BlockingCollection<string> queue;
        private long lastWriteTime = 0;
        private long writeInterval = 500;
        
        public FileWatcher(AchievementParser AchievementParser)
        {
            parser = AchievementParser;
            queue = new BlockingCollection<string>();   

            String monitorFile = parser.GetStatsFile(); 
            String watchDirectory = System.IO.Path.GetDirectoryName(monitorFile);
            String watchFile = System.IO.Path.GetFileName(monitorFile);

            watcher = new FileSystemWatcher(watchDirectory, watchFile);
            watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.Size | NotifyFilters.LastWrite;
            watcher.Changed += (_, e) => queue.Add(e.FullPath);
            watcher.Created += (_, e) => queue.Add(e.FullPath);
        }

        public void Start()
        {
            watcher.EnableRaisingEvents = true;
            Console.WriteLine($"Watching {watcher.Path}\\{watcher.Filter}");
            
            while (!queue.IsCompleted)
            {
                string filePath = queue.Take();
                Debug.WriteLine($"Detected change {filePath}");
               
                long fileWriteTime = new FileInfo(filePath).LastWriteTimeUtc.Ticks;
                if (fileWriteTime - lastWriteTime < writeInterval) continue;
                lastWriteTime = fileWriteTime;

                parser.DetectUnlockedAchievements(filePath);                
            }
        }
    }
}
