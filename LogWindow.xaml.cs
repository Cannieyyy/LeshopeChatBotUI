using System.Windows;
using System.Windows.Threading;
using static LeshopeChatBotUI.UnmatchedEntry;


namespace LeshopeChatBotUI
{
    /// <summary>
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window
    {       
        private string currentUser;
        private LogWindow logWindow;

        private List<QuizLogEntry> quizLogs = new List<QuizLogEntry>();


        public LogWindow(string userName)
        {
            InitializeComponent();
            currentUser = userName;//setting current user to userName

            


            //initialize the log display
            DisplayLogs();

            //initialize autoRefresh
            SetupAutoRefresh();

            
        }

        //method to display logs
        private void DisplayLogs()
        {
            UpdateLogDisplay();
        }

        //method to clear the log
        public void ClearUserLog()
        {
            DisplayLogs();
        }


        //method to auto-update the log during runtime
        private void SetupAutoRefresh()
        {
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            timer.Tick += (s, e) => DisplayLogs();
            timer.Start();
        }

        private List<string> unmatchedQuestionsLog = new List<string>();

        public void AddUnmatchedQuestionLog(UnmatchedEntry entry)
        {
            string formatted = $"==== FAILED RESPONSE RETURN ====\n" +
                       $"Question: {entry.OriginalInput}\n" +
                       $"Date: {entry.Timestamp}\n" +
                       $"===============================\n";

            unmatchedQuestionsLog.Add(formatted);

            // If using ListView:
            LogListView.Items.Add(formatted);

           
        }

        private void UpdateLogDisplay()
        {
            LogListView.Items.Clear();

            // User activity logs
            if (RuntimeLog.Logs.Count > 0)
            {
                LogListView.Items.Add("===== User Activity Logs =====");
                foreach (var log in RuntimeLog.Logs)
                {
                    LogListView.Items.Add(log);
                }
            }

            // Unmatched questions logs
            if (unmatchedQuestionsLog.Count > 0)
            {
                LogListView.Items.Add("===== Unmatched Questions Logs =====");
                foreach (var unmatched in unmatchedQuestionsLog)
                {
                    LogListView.Items.Add(unmatched);
                }
            }

            // Quiz logs
            if (quizLogs.Count > 0)
            {
                LogListView.Items.Add("===== Quiz Logs =====");
                foreach (var quiz in quizLogs)
                {
                    LogListView.Items.Add(quiz.ToString());
                }
            }
        }

        
        public void AddQuizLog(int score, int total)
        {

            var entry = new QuizLogEntry
            {
                Timestamp = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss"),
                Score = score,
                Total = total
            };

            quizLogs.Add(entry);
            UpdateLogDisplay(); // Refresh display
        }


        public void AddRuntimeLog(string entry)
        {
            LogListView.Items.Add(entry);
        }
    }
}
