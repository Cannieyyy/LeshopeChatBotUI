using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeshopeChatBotUI
{
    public class MemoryManager
    {
        private readonly string memoryFilePath;

        public MemoryManager()
        {
            memoryFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\memory.txt");
            memoryFilePath = Path.GetFullPath(memoryFilePath); // Normalize path
        }

        public void SaveData(string key, string value)
        {
            var data = new Dictionary<string, string>();

            if (File.Exists(memoryFilePath))
            {
                foreach (var line in File.ReadAllLines(memoryFilePath))
                {
                    var parts = line.Split('=');
                    if (parts.Length == 2)
                        data[parts[0].Trim()] = parts[1].Trim();
                }
            }
            else
            {
                File.Create(memoryFilePath).Close();
            }

            data[key] = value;

            using (StreamWriter writer = new StreamWriter(memoryFilePath))
            {
                foreach (var pair in data)
                {
                    writer.WriteLine($"{pair.Key}={pair.Value}");
                }
            }
        }

        public string LoadData(string key)
        {
            if (!File.Exists(memoryFilePath))
                return null;

            foreach (var line in File.ReadAllLines(memoryFilePath))
            {
                var parts = line.Split('=');
                if (parts.Length == 2 && parts[0].Trim() == key)
                    return parts[1].Trim();
            }

            return null;
        }

        //a method to store topics in a list format
        public void AppendToList(string key, string item)
        {
            var current = LoadData(key);
            var items = new HashSet<string>((current ?? "").Split(','));//topics are split by a ,
            if (!string.IsNullOrWhiteSpace(item))
                items.Add(item);

            SaveData(key, string.Join(",", items));//saved as a joined topic
        }

        // a method that loads the list from a file
        public List<string> LoadList(string key)
        {
            var data = LoadData(key);
            if (data == null) return new List<string>();
            return new List<string>(data.Split(','));
        }

        //a method that clears the memory file
        public void ClearFile()
        {
            // Overwrite the file with an empty string
            File.WriteAllText(memoryFilePath, string.Empty);
        }

    }
}
