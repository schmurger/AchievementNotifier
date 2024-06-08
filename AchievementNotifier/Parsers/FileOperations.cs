using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AchievementNotifier.Parsers
{
    public class FileOperations
    {
        public static string[] readContents(String fileName)
        {
            if (File.Exists(fileName))
            {
                while (true)
                {
                    try
                    {
                        string fileContents;
                        using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8))
                        {
                            fileContents = streamReader.ReadToEnd();
                        }
                        return fileContents.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"File busy {ex}");
                    }
                    Thread.Sleep(500);
                }

            }
            return new string[] { };
        }

        public static string findFile(String gameDirectory, String file)
        {
            return Directory.GetFiles(gameDirectory, file, SearchOption.AllDirectories).FirstOrDefault();
        }


    }
}
