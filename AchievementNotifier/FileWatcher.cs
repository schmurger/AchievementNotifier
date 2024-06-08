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

        public FileWatcher(AchievementParser AchievementParser)
        {
            parser = AchievementParser;
            watcher = new FileSystemWatcher();
           
            String monitorFile = parser.GetStatsFile(); 
            String watchDirectory = System.IO.Path.GetDirectoryName(monitorFile);
            String watchFile = System.IO.Path.GetFileName(monitorFile);

            watcher.Path = watchDirectory;
            watcher.Filter = watchFile;
            watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.Size | NotifyFilters.LastWrite;
            watcher.Changed += new FileSystemEventHandler(this.OnChanged);


            Console.WriteLine($"Watching {watcher.Path}\\{watcher.Filter}");
            watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            Debug.WriteLine($"Detected change {e.FullPath}");
            parser.DetectUnlockedAchievements(e.FullPath);
        }
    }
}
