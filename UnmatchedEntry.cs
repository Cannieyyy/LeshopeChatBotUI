using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LeshopeChatBotUI
{
    public class UnmatchedEntry
    {
        public string OriginalInput { get; set; }
        public string CleanedInput { get; set; }
        public string Timestamp { get; set; }
        public bool Responded { get; set; } = false;


        public static class InputCleaner
        {
            private static readonly HashSet<string> StopWords = new()
        {
            "what", "is", "the", "a", "an", "of", "and", "to", "in", "on", "for", "with", "by",
            "how", "do", "does", "can", "you", "me", "i", "am", "are", "we", "us", "they", "my"
        };

            public static string Clean(string input)
            {
                input = input.ToLower();
                input = Regex.Replace(input, @"[^\w\s]", ""); // remove punctuation
                var words = input.Split(' ')
                                 .Where(word => !StopWords.Contains(word) && !string.IsNullOrWhiteSpace(word));
                return string.Join(" ", words);
            }
        }

        public class UnmatchedLogger
        {
            private readonly string filePath;
            public event Action<UnmatchedEntry> OnUnmatchedLogged;

            public UnmatchedLogger(string jsonFilePath = "trained_data.json")
            {
                filePath = jsonFilePath;
            }

            public void Log(string originalInput)
            {
                File.AppendAllText("log.txt", $"[Debug] Log method called with input: {originalInput}\n");

                try
                {
                    string cleanedInput = InputCleaner.Clean(originalInput);

                    var newEntry = new UnmatchedEntry
                    {
                        OriginalInput = originalInput,
                        CleanedInput = cleanedInput,
                        Timestamp = DateTime.UtcNow.ToString("f"), // friendlier format
                        Responded = false
                    };

                    Dictionary<string, object> jsonContent = new();

                    if (File.Exists(filePath))
                    {
                        string raw = File.ReadAllText(filePath);
                        jsonContent = JsonSerializer.Deserialize<Dictionary<string, object>>(raw) ?? new();
                    }

                    List<UnmatchedEntry> unmatched = new();

                    if (jsonContent.ContainsKey("unmatched"))
                    {
                        var jsonElem = (JsonElement)jsonContent["unmatched"];
                        unmatched = JsonSerializer.Deserialize<List<UnmatchedEntry>>(jsonElem.GetRawText()) ?? new();
                    }

                    unmatched.Add(newEntry);
                    jsonContent["unmatched"] = unmatched;

                    OnUnmatchedLogged?.Invoke(newEntry); 

                    string updatedJson = JsonSerializer.Serialize(jsonContent, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(filePath, updatedJson);
                }
                catch (Exception ex)
                {
                    File.AppendAllText("log.txt", $"[UnmatchedLogger] Error: {ex.Message}\n");
                }
            }
        }
    }
}
