using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace AchievementNotifier.Parsers
{
    public class FileOperations
    {
        public static string[] readFile(String fileName)
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
            string[] files = Directory.GetFiles(gameDirectory, file, SearchOption.AllDirectories);
            if (files.Length == 0) return null;

            return files[0];
        }

        public static void WriteToFile(List<String> lines, string file)
        {
            using (StreamWriter writetext = new StreamWriter(file))
            {
                lines.ForEach(line => { writetext.WriteLine(line); });
            }
        }
        public static void CreateFolders(String file)
        {
            if (!File.Exists(file))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(file));
            }
        }
    }
}
