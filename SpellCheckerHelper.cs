using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeshopeChatBotUI
{
    public static class SpellCheckerHelper
    {
        private static readonly HashSet<string> wordList = new();
        private static bool initialized = false;

        static SpellCheckerHelper()
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string wordFile = Path.Combine(baseDir, "words.txt");

                if (File.Exists(wordFile))
                {
                    foreach (var word in File.ReadAllLines(wordFile))
                    {
                        if (!string.IsNullOrWhiteSpace(word))
                            wordList.Add(word.Trim().ToLower());
                    }

                    initialized = true;
                }
            }
            catch
            {
                initialized = false;
            }
        }

        public static string CorrectWord(string word)
        {
            if (!initialized || string.IsNullOrWhiteSpace(word))
                return word;

            string lowerWord = word.ToLower();
            if (wordList.Contains(lowerWord))
                return word;

            // Find the word in the list with the smallest Levenshtein distance
            string closest = wordList
                .OrderBy(w => LevenshteinDistance(w, lowerWord))
                .FirstOrDefault();

            return string.IsNullOrEmpty(closest) ? word : closest;
        }

        public static string CorrectSentence(string sentence)
        {
            var words = sentence.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var corrected = words.Select(CorrectWord);
            return string.Join(" ", corrected);
        }

        private static int LevenshteinDistance(string s, string t)
        {
            if (s == t) return 0;
            if (string.IsNullOrEmpty(s)) return t.Length;
            if (string.IsNullOrEmpty(t)) return s.Length;

            int[,] d = new int[s.Length + 1, t.Length + 1];

            for (int i = 0; i <= s.Length; i++) d[i, 0] = i;
            for (int j = 0; j <= t.Length; j++) d[0, j] = j;

            for (int i = 1; i <= s.Length; i++)
            {
                for (int j = 1; j <= t.Length; j++)
                {
                    int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;

                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            return d[s.Length, t.Length];
        }
    }
}
