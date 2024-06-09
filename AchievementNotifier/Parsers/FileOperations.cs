using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

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
            return Directory.GetFiles(gameDirectory, file, SearchOption.AllDirectories).FirstOrDefault();
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
        public static void SerializeToFile(string storageFile, Dictionary<String, GameView> Storage)
        {
            CreateFolders(storageFile);

            using (FileStream fs = File.Create(storageFile))
            {
                DataContractSerializer serializer = new DataContractSerializer(Storage.GetType());
                serializer.WriteObject(fs, Storage);
            }
        }

        public static Dictionary<String, GameView> DeserializeFromFile(string storageFile)
        {
            Dictionary <String, GameView> Storage = new Dictionary<String, GameView >();
            if (!File.Exists(storageFile)) return Storage;

            using (FileStream fs = new FileStream(storageFile, FileMode.Open))
            {
                DataContractSerializer serializer = new DataContractSerializer(Storage.GetType());
                return (Dictionary<String, GameView>)serializer.ReadObject(fs);
                

            }
        }
    }
}
