using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeshopeChatBotUI
{
    public class QuizQuestion
    {
        public string Question { get; set; }
        public List<string> Options { get; set; }
        public int CorrectIndex { get; set; }

        
        
    }

    public class QuizLogEntry
    {
        public string Timestamp { get; set; }
        public int Score { get; set; }
        public int Total { get; set; }

        public override string ToString()
        {
            return $"=== QUIZ PLAYED ===\n" +
                   $"Time: {Timestamp}\n" +
                   $"Score: {Score}/{Total}\n" +
                   $"====================";
        }
    }
}
