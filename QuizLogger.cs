using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeshopeChatBotUI
{
    public static class QuizLogger
    {
        public static LogWindow logWindow; // Should be assigned on main window load

        public static void LogScore(int score, int total)
        {
            logWindow?.AddQuizLog(score, total);

        }
    }
}
