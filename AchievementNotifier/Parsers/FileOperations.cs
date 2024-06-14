using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace AchievementNotifier.Parsers
{
    public class FileOperations
    {
        private static int RETRIES = 3;


        public static string[] readFile(String fileName)
        {
            string fileContents = readString(fileName);
            if(fileContents != null)
            {
                return fileContents.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            }
            return new string[] { };
        }

        public static T Read<T>(string fileName)
        {
            try
            {
                string fileContents = readString(fileName);
                return JsonSerializer.Deserialize<T>(fileContents);
            }
            catch (Exception e)
            {
                return (T)Activator.CreateInstance(typeof(T));
            }
        }

        public static int readInt(string fileName)
        {
            if (!File.Exists(fileName)) return 0;
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] bytes = new byte[4];
                fileStream.Read(bytes, 0, bytes.Length);
                int value =  BitConverter.ToInt32(bytes, 0);
                return value;
            }
        }

        public static string readString(String fileName)
        {
            if (!File.Exists(fileName)) return null;
            int retries = 0;
            while (retries < RETRIES)
            {
                try
                {
                    using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (StreamReader streamReader = new StreamReader(fileStream, Encoding.UTF8))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"File busy {ex.Message}");
                }
                retries++;
                Thread.Sleep(500);
            }

            return null;
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
