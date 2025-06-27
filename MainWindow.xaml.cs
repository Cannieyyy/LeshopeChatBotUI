using System.IO;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Media;
using static LeshopeChatBotUI.UnmatchedEntry;


namespace LeshopeChatBotUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        private CyberBot bot;
        private MemoryManager memory;
        private LogWindow logWindow;
        private StringWriter botOutput = new StringWriter();
        private UnmatchedLogger unmatchedLogger;

        //decalre a list to store the random questions
        private readonly List<string> questions = new List<string>
           {
                //stroring the random responses
                "What would you like to know about cyber security today",
                "How can I assist you with your security concerns",
                "What topic about online safety interests you",
                "Is there something you'd like to learn about passwords, phishing, or safe browsing",
                "Do you have any questions about protecting yourself online"
           };



        public MainWindow()
        {
            try
            {

                InitializeComponent();

                bot = new CyberBot();
                bot.TrainModel("chat_training_data.json");
                logWindow = new LogWindow(bot.userName);
                memory = new MemoryManager();

                

                unmatchedLogger = new UnmatchedLogger();

                unmatchedLogger.OnUnmatchedLogged += (logEntry) =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        logWindow.AddUnmatchedQuestionLog(logEntry);
                    });
                };


            }

            catch (Exception ex)
            {
                MessageBox.Show("Initialization failed: " + ex.Message);
                // Optionally: log ex.ToString() to a file for deeper analysis
            }

        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            //declare a variable to store the entered name on the textbox
            string enteredName = UsernameInput.Text.Trim();

            // Filter unwanted words from the entered name
            enteredName = bot.FilterUnwantedWords(enteredName);

            //set the entered name to the userName on CyberBot class
            bot.userName = enteredName;

            // Validate the username
            if (!bot.isValidName(bot.userName))
            {
                UsernameError.Text = $"This field cannot be empty, contain a number or characters! Please give me a name {bot.userName}!";
                UsernameError.Visibility = Visibility.Visible;
                return;
            }



            // Hide error if valid
            UsernameError.Visibility = Visibility.Hidden;


            // Log user login 
            RuntimeLog.Add("==== USER LOGIN ====");
            RuntimeLog.Add($"User: {bot.userName}");
            RuntimeLog.Add($"Login Time: {DateTime.Now:dddd, dd MMMM yyyy HH:mm:ss}");
            RuntimeLog.Add("====================");


            WelcomePanel.Visibility = Visibility.Hidden;
            LoadUser();
        }

        private void LoadUser()
        {
            string savedName = memory.LoadData("userName");
            string savedBotName = memory.LoadData("botName");



            if (!string.IsNullOrEmpty(savedName) && savedName.Equals(bot.userName, StringComparison.OrdinalIgnoreCase))
            {
                // Show ReturningUserPanel and hide WelcomePanel
                ReturningUserPanel.Visibility = Visibility.Visible;

                // Display welcome message
                ReturningUserWelcomeText.Text = $"Welcome back, {bot.userName}!";

                if (!string.IsNullOrEmpty(savedBotName))
                {
                    bot.botName = savedBotName;
                    BotNamePrompt.Text = $"Would you still like my name to be {savedBotName}, {bot.userName}?";

                }

                PreviousSentiment();
            }
            else
            {
                // Call welcome user method 
                WelcomeNewUser();
            }
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            // Keep saved bot name
            MessageBox.Show($"Fantastic! My name remains {bot.botName}");

            // Hide returning user panel
            ReturningUserPanel.Visibility = Visibility.Hidden;
            ChatPanel.Visibility = Visibility.Visible;

        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            ReturningUserPanel.Visibility = Visibility.Hidden;

            // Call RenameBot method
            RenameBot();



        }

        private void WelcomeNewUser()
        {
            WelcomePanel.Visibility = Visibility.Hidden;
            WelcomeNewUserPanel.Visibility = Visibility.Visible;



            // Personalized message
            NewUserGreeting.Text = $"Nice to meet you, {bot.userName}!";

            memory.ClearFile();//clear the file

            memory.SaveData("userName", bot.userName);

            PromptBotName.Text = $"Would you like to give me a name {bot.userName}?";

            logWindow.ClearUserLog();


            // Placeholder: save the name to memory
            memory.SaveData("userName", bot.userName);

            // Placeholder: proceed to bot name setup or another interaction
            // AskBotName();
        }

        private void Yes_GiveName(object sender, RoutedEventArgs e)
        {
            WelcomeNewUserPanel.Visibility = Visibility.Hidden;
            RenameBot();

        }

        private void No_GiveName(object sender, RoutedEventArgs e)
        {
            bot.botName = bot.botName;

            memory.SaveData("botname", bot.botName);
            MessageBox.Show($"Awesome my name will remain {bot.botName}");

            WelcomeNewUserPanel.Visibility = Visibility.Hidden;
            ChatPanel.Visibility = Visibility.Visible;

        }
        private void RenameBot()
        {
            RenameBotPanel.Visibility = Visibility.Visible;
            BotRenameInput.Clear();
            BotRenameError.Visibility = Visibility.Hidden;


            BotNameQuestion.Text = $"That's awesome! What would you like to call me {bot.userName}?";



        }//end of rename bot method

        private void ConfirmRename_Click(object sender, RoutedEventArgs e)
        {

            // Get the entered bot name from the textbox
            string enteredName = BotRenameInput.Text.Trim();

            // Filter unwanted words
            enteredName = bot.FilterUnwantedWords(enteredName);

            // Validate the new name
            if (!bot.isValidName(enteredName))
            {
                BotRenameError.Text = $"This field cannot be empty, contain a number or characters! Please give me a name {bot.userName}!";
                BotRenameError.Visibility = Visibility.Visible;
                return;
            }

            // Set and save the valid name
            bot.botName = enteredName;
            memory.SaveData("botName", bot.botName);

            MessageBox.Show($"{bot.botName} is a wonderful name {bot.userName}! I will definitely remember this name!");

            // Hide the RenameBotPanel
            RenameBotPanel.Visibility = Visibility.Hidden;

            //Display the chatpanel
            ChatPanel.Visibility = Visibility.Visible;



        }

        private void AddChatBubble(string message, bool isUser)
        {
            var bubbleText = new TextBlock
            {
                Text = message,
                TextWrapping = TextWrapping.Wrap,
                Foreground = Brushes.Black,
                MaxWidth = 400,
                Padding = new Thickness(10)
            };

            var bubbleContainer = new Border
            {
                Background = isUser ? Brushes.LightBlue : Brushes.LightGray,
                CornerRadius = new CornerRadius(15),
                Padding = new Thickness(5),
                Margin = new Thickness(5),
                Child = bubbleText,
                HorizontalAlignment = isUser ? HorizontalAlignment.Right : HorizontalAlignment.Left
            };

            ChatStack.Children.Add(bubbleContainer);
        }

        private async void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            string userMessage = UserMessageInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(userMessage)) return;

            AddChatBubble(userMessage, true);
            UserMessageInput.Text = "";

            var responses = bot.LoadResponsesFromFile("responses.txt");

            //Try to find response without correction
            bool gotResponse = await bot.FindBestResponse(responses, userMessage, response =>
            {
                AddChatBubble(response, false);
            });

            if (!gotResponse)
            {
                // 🪄 2. Try correcting the sentence only if no result was found
                string correctedMessage = SpellCheckerHelper.CorrectSentence(userMessage);

                // Avoid infinite loops: skip if same as input
                if (!string.Equals(userMessage, correctedMessage, StringComparison.OrdinalIgnoreCase))
                {
                    AddChatBubble($"[Auto-corrected to]: {correctedMessage}", false);

                    gotResponse = await bot.FindBestResponse(responses, correctedMessage, response =>
                    {
                        AddChatBubble(response, false);
                    });
                }
            }

            if (!gotResponse)
            {
                unmatchedLogger.Log(userMessage);

                string[] fallbackResponses =
                {
                    $"I'm not sure I understood that, {bot.userName}.",
                    $"Could you please rephrase that?",
                    $"Hmm... I didn't quite catch that, {bot.userName}. Want to try again?"
                };

                Random random = new Random();
                AddChatBubble($"{bot.botName}: {fallbackResponses[random.Next(fallbackResponses.Length)]}", false);
            }
        }

        private void PreviousSentiment()
        {
            string previousSentiment = memory.LoadData($"sentiment_{bot.userName}");

            if (!string.IsNullOrEmpty(previousSentiment))
            {
                string sentimentGreeting = $"{bot.botName}: Hey {bot.userName}, last time you felt {previousSentiment}. How are you feeling today?";
                AddChatBubble(sentimentGreeting, false);
            }
            else
            {
                AskRandomInitialQuestion();
            }
        }

        private void AskRandomInitialQuestion()
        {
            Random random = new Random();
            string question = questions[random.Next(questions.Count)];

            string greeting = $"{bot.botName}: {question}, {bot.userName}?";
            AddChatBubble(greeting, false);
        }

        private void Tasks_Click(object sender, RoutedEventArgs e)
        {
            TaskManagerWindow taskManager = new TaskManagerWindow();
            taskManager.Show();
        }

        private void Quiz_Click(object sender, RoutedEventArgs e)
        {
            QuizWindow quizWindow = new QuizWindow();
            quizWindow.Show();
        }
        private void Logs_Click(object sender, RoutedEventArgs e)
        {
            if (!logWindow.IsVisible)
            {
                logWindow = new LogWindow(bot.userName);
                logWindow.Show();
            }
            else
            {
                logWindow.Activate();
            }
        }


    }
}