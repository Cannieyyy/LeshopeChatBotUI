using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace LeshopeChatBotUI
{
    /// <summary>
    /// Interaction logic for QuizWindow.xaml
    /// </summary>
    public partial class QuizWindow : Window
    {
        private List<QuizQuestion> allQuestions;
        private List<QuizQuestion> selectedQuestions;
        private int currentQuestionIndex = 0;
        private int score = 0;
        private static readonly Random random = new();

        private static readonly List<string> encouragements = new()
        {
            "You were close! Keep going!",
            "Almost had it!",
            "Don't give up!",
            "You're learning fast!",
            "Solid try!",
            "Keep pushing!",
            "Great effort!",
            "Stay sharp!",
            "Not bad!",
            "You're improving!"
        };

        private static readonly List<string> praises = new()
        {
            "Awesome!",
            "Well done!",
            "Brilliant!",
            "Nailed it!",
            "Perfect!",
            "You're a star!",
            "Excellent work!",
            "That's right!",
            "Great answer!",
            "Impressive!"
        };

        private static readonly List<string> finalPositive = new()
        {
            "Outstanding job!",
            "You're a cybersecurity pro!",
            "Fantastic performance!",
            "You crushed it!",
            "Well done — keep learning!"
        };

        private static readonly List<string> finalConstructive = new()
        {
            "Almost there — try again!",
            "Not bad, you’re getting there!",
            "Keep practicing!",
            "Good try, let’s improve more!",
            "Better luck next time!"
        };

        public QuizWindow()
        {
            InitializeComponent();
            LoadQuestions();
            selectedQuestions = allQuestions.OrderBy(_ => Guid.NewGuid()).Take(10).ToList();
            DisplayCurrentQuestion();
        }

        private void LoadQuestions()
        {
            allQuestions = new List<QuizQuestion>
            {
                new QuizQuestion { Question = "What does 'phishing' mean?", Options = new List<string> { "Network scanning", "Stealing info via trickery", "Encrypting malware", "Bypassing firewalls" }, CorrectIndex = 1 },
                new QuizQuestion { Question = "Which is a strong password?", Options = new List<string> { "123456", "qwerty", "P@55w0rd!", "abc123" }, CorrectIndex = 2 },
                new QuizQuestion { Question = "True or False: HTTPS is more secure than HTTP.", Options = new List<string> { "True", "False" }, CorrectIndex = 0 },
                new QuizQuestion { Question = "What is 2FA?", Options = new List<string> { "Two File Access", "Two-Factor Authentication", "Trusted Firewall Agreement", "Tool for Admins" }, CorrectIndex = 1 },
                new QuizQuestion { Question = "A firewall is used to...", Options = new List<string> { "Encrypt files", "Host websites", "Monitor and control traffic", "Scan for malware" }, CorrectIndex = 2 },
                new QuizQuestion { Question = "Which of these is NOT malware?", Options = new List<string> { "Virus", "Worm", "Firewall", "Trojan" }, CorrectIndex = 2 },
                new QuizQuestion { Question = "Which of the following is a social engineering attack?", Options = new List<string> { "Phishing", "DDoS", "Brute Force", "SQL Injection" }, CorrectIndex = 0 },
                new QuizQuestion { Question = "True or False: Antivirus software prevents phishing.", Options = new List<string> { "True", "False" }, CorrectIndex = 1 },
                new QuizQuestion { Question = "What should you do before clicking links in emails?", Options = new List<string> { "Click immediately", "Scan with antivirus", "Verify the source", "Forward to friends" }, CorrectIndex = 2 },
                new QuizQuestion { Question = "Which of these is a secure protocol?", Options = new List<string> { "FTP", "SMTP", "HTTP", "HTTPS" }, CorrectIndex = 3 },
                new QuizQuestion { Question = "Fill in the blank: The process of converting data into a coded form to prevent unauthorized access is called ______.", Options = new List<string> { "Encryption", "Phishing", "Firewall", "Scanning" }, CorrectIndex = 0 },
                new QuizQuestion { Question = "True or False: Shoulder surfing is a type of social engineering.", Options = new List<string> { "True", "False" }, CorrectIndex = 0 },
                new QuizQuestion { Question = "Which of these is NOT a form of two-factor authentication?", Options = new List<string> { "Password + OTP", "Fingerprint + PIN", "Password + Security Question", "Username + Password" }, CorrectIndex = 3 },
                new QuizQuestion { Question = "Fill in the blank: ______ is software designed to damage, disrupt, or gain unauthorized access to systems.", Options = new List<string> { "Malware", "Firewall", "Patch", "Backup" }, CorrectIndex = 0 },
                new QuizQuestion { Question = "True or False: Antivirus software guarantees 100% protection against malware.", Options = new List<string> { "True", "False" }, CorrectIndex = 1 },
                new QuizQuestion { Question = "Choose the INCORRECT statement about passwords.", Options = new List<string> { "They should be long and complex", "They should be shared with trusted friends", "They should be unique for each account", "They should include numbers and symbols" }, CorrectIndex = 1 },
                new QuizQuestion { Question = "Fill in the blank: A ______ attack overwhelms a system with traffic to make it unavailable.", Options = new List<string> { "DDoS", "Phishing", "Spyware", "Keylogger" }, CorrectIndex = 0 },
                new QuizQuestion { Question = "True or False: Keeping software updated reduces vulnerability risks.", Options = new List<string> { "True", "False" }, CorrectIndex = 0 },
                new QuizQuestion { Question = "Which of these is NOT a type of cyber attack?", Options = new List<string> { "Phishing", "Brute Force", "SQL Injection", "Scanning Printer" }, CorrectIndex = 3 },
                new QuizQuestion { Question = "Fill in the blank: ______ is the practice of protecting systems and networks from digital attacks.", Options = new List<string> { "Cybersecurity", "Networking", "Programming", "Penetration" }, CorrectIndex = 0 },
                new QuizQuestion { Question = "True or False: Using public Wi-Fi for banking is safe.", Options = new List<string> { "True", "False" }, CorrectIndex = 1 },
                new QuizQuestion { Question = "Choose the INCORRECT example of personal identifiable information (PII).", Options = new List<string> { "Social Security Number", "Bank Password", "Email Address", "Favorite Movie Genre" }, CorrectIndex = 3 },
                new QuizQuestion { Question = "Fill in the blank: ______ is used to scan for and remove malicious software.", Options = new List<string> { "Antivirus", "Firewall", "Spam Filter", "Router" }, CorrectIndex = 0 },
                new QuizQuestion { Question = "True or False: Hackers only target large businesses.", Options = new List<string> { "True", "False" }, CorrectIndex = 1 },
                new QuizQuestion { Question = "Which of these is NOT a secure practice online?", Options = new List<string> { "Using strong passwords", "Clicking unknown links", "Enabling 2FA", "Updating software regularly" }, CorrectIndex = 1 },
                new QuizQuestion { Question = "Fill in the blank: ______ is a scam where attackers pretend to be someone else to steal info.", Options = new List<string> { "Phishing", "Scanning", "Encrypting", "Firewalling" }, CorrectIndex = 0 },
                new QuizQuestion { Question = "True or False: You should install updates as soon as they are available.", Options = new List<string> { "True", "False" }, CorrectIndex = 0 },
                new QuizQuestion { Question = "Choose the INCORRECT statement about firewalls.", Options = new List<string> { "They block unauthorized access", "They encrypt data files", "They monitor network traffic", "They act as a security barrier" }, CorrectIndex = 1 },
                new QuizQuestion { Question = "Fill in the blank: ______ is software that records every keystroke made by a user.", Options = new List<string> { "Keylogger", "Firewall", "Patch", "VPN" }, CorrectIndex = 0 },
                new QuizQuestion { Question = "True or False: It is safe to reuse the same password on multiple accounts.", Options = new List<string> { "True", "False" }, CorrectIndex = 1 },

            };
        }

        private void DisplayCurrentQuestion()
        {
            var question = selectedQuestions[currentQuestionIndex];
            QuestionText.Text = $"Q{currentQuestionIndex + 1}: {question.Question}";
            AnswersPanel.Children.Clear();

            for (int i = 0; i < question.Options.Count; i++)
            {
                var option = new RadioButton
                {
                    Content = question.Options[i],
                    Tag = i,
                    GroupName = "AnswerGroup"
                };
                AnswersPanel.Children.Add(option);
            }
        }
        private void StartQuiz_Click(object sender, RoutedEventArgs e)
        {
            WelcomePanel.Visibility = Visibility.Collapsed;
            QuizPanel.Visibility = Visibility.Visible;
            DisplayCurrentQuestion();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (currentQuestionIndex >= selectedQuestions.Count)
                return;

            var selectedAnswer = AnswersPanel.Children
                .OfType<RadioButton>()
                .FirstOrDefault(rb => rb.IsChecked == true);

            if (selectedAnswer == null)
            {
                ResponseText.Text = "Please select an answer.";
                ResponseText.Foreground = Brushes.Orange;
                ResponseText.Visibility = Visibility.Visible;
                return;
            }

            int selectedIndex = AnswersPanel.Children.IndexOf(selectedAnswer);
            var correct = selectedIndex == selectedQuestions[currentQuestionIndex].CorrectIndex;

            // Show encouragement or praise
            string feedback;
            if (correct)
            {
                score++;
                ResponseText.Foreground = Brushes.LightGreen;
                feedback = praises[random.Next(praises.Count)];
            }
            else
            {
                ResponseText.Foreground = Brushes.Orange;
                feedback = encouragements[random.Next(encouragements.Count)];
            }
            ResponseText.Text = feedback;
            ResponseText.Visibility = Visibility.Visible;

            currentQuestionIndex++;

            // Wait and show next question
            Dispatcher.InvokeAsync(async () =>
            {
                await Task.Delay(1000);
                if (currentQuestionIndex < selectedQuestions.Count)
                {
                    DisplayCurrentQuestion();
                }
                else
                {
                    ShowFinalScore();
                }
            });

        }

        private void ShowFinalScore()
        {
            QuizPanel.Visibility = Visibility.Visible;
            QuestionText.Visibility = Visibility.Collapsed;
            AnswersPanel.Visibility = Visibility.Collapsed;
            ResponseText.Visibility = Visibility.Collapsed;

            string resultMsg;
            double scoreRatio = (double)score / selectedQuestions.Count;

            if (scoreRatio >= 0.7)
            {
                resultMsg = finalPositive[random.Next(finalPositive.Count)];
            }
            else
            {
                resultMsg = finalConstructive[random.Next(finalConstructive.Count)];
            }

            FinalScoreText.Text = $"Your Score: {score}/{selectedQuestions.Count}\n{resultMsg}";
            FinalScoreText.Visibility = Visibility.Visible;

            QuizLogger.LogScore(score, selectedQuestions.Count);

        }

    }
}
