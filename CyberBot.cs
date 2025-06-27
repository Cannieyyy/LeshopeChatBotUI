using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.ML;
using static LeshopeChatBotUI.UnmatchedEntry;

namespace LeshopeChatBotUI
{
    public class CyberBot
    {
        //an instance for the Unmatched logger class
        private UnmatchedLogger unmatchedLogger = new UnmatchedLogger();
        
        //instance of the MLContext class - from the built-in package(Microsoft.ML)
        private MLContext mlContext;
        private ITransformer model;
        private PredictionEngine<ChatData, ChatPrediction> predictionEngine;


        //cretaing an instance of a class memory manager to access class methods
        private MemoryManager fileManager = new MemoryManager();

        //declaring a dictionary to temporarly store response and keyword
        private Dictionary<string, string> memory;

        //declaring and initialzing global variables to store userName and botName
        public string botName { get; set; } = "CyberBot";// this initialises the botName to a specific name before the user changes it
        public string userName { get; set; } = "You";
        string userInput; //this declaration is for the input of the user
        string pattern = "^[a-zA-Z ]+$";// this is the declaration of a pattern to validate input


        // Method to remove unnecessary words from a string
        public string FilterUnwantedWords(string input)
        {
            //declaring an array to store all the unwanted words
            string[] wordsToRemove = { "my", "name", "is", "i", "am", "call", "me", "the", "would", "like", ".", ",", "to", "your", "you" };
            List<string> words = new List<string>(input.Split(' '));

            // Remove unwanted words
            words.RemoveAll(word => wordsToRemove.Contains(word.ToLower()));

            // Return the first remaining word or "Unknown" if empty
            return words.Count > 0 ? words[0].ToString() : "Unknown";
        }


        //a method that validates is a username contains letters only
        public bool isValidName(string name)
        {
            return (Regex.IsMatch(name, pattern) && !string.IsNullOrWhiteSpace(name));
        }

        //method to train the model
        public void TrainModel(string jsonFilePath)
        {
            try
            {
                if (!File.Exists(jsonFilePath))
                {
                    Console.WriteLine("Training JSON file not found.");
                    return;
                }

                var json = File.ReadAllText(jsonFilePath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var chatDataList = JsonSerializer.Deserialize<List<ChatData>>(json, options);

                if (chatDataList == null || !chatDataList.Any())
                {
                    Console.WriteLine("No data in training JSON.");
                    return;
                }

                mlContext = new MLContext();

                var trainingData = mlContext.Data.LoadFromEnumerable(chatDataList);

                var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(ChatData.UserInput))
                    .Append(mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(ChatData.Response)))
                    .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
                    .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

                model = pipeline.Fit(trainingData);
                predictionEngine = mlContext.Model.CreatePredictionEngine<ChatData, ChatPrediction>(model);

                Console.WriteLine("Training complete.");
            }
            catch (Exception ex)
            {
                File.WriteAllText("ml_error.txt", $"Exception: {ex.Message}\n{ex.StackTrace}");
            }
        }

        //creating a method for making predictions
        public string GetMLResponse(string userInput)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userInput) || predictionEngine == null)
                {
                    return null;
                }

                var prediction = predictionEngine.Predict(new ChatData { UserInput = userInput });

                // Get confidence score
                float[] scores = prediction.Score;
                float maxScore = scores.Max();
                float confidenceThreshold = 0.6f; // You can tune this

                // Reject low-confidence responses
                if (maxScore < confidenceThreshold)
                {
                    File.AppendAllText("log.txt", $"[ML DROPPED] Low confidence: {maxScore}\n");
                    return null;
                }

                // Reject poor or default responses
                if (string.IsNullOrWhiteSpace(prediction.Response) ||
                    prediction.Response.Length < 5 ||
                    prediction.Response.ToLower().Contains("default") ||
                    prediction.Response.ToLower().Contains("unknown") ||
                    prediction.Response.ToLower().Contains("i don't know"))
                {
                    File.AppendAllText("log.txt", $"[ML DROPPED] Suspicious/empty response: {prediction.Response}\n");
                    return null;
                }

                File.AppendAllText("log.txt", $"[ML ACCEPTED] Confidence: {maxScore}, Response: {prediction.Response}\n");
                return prediction.Response;
            }
            catch (Exception ex)
            {
                File.AppendAllText("log.txt", $"[ERROR] ML Prediction Failed: {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }

        //Creating a method LoadResponsesfromFile to load the responses from a file.
        public List<(string Keyword, List<string> Response)> LoadResponsesFromFile(string filePath)
        {
            //Declaring an list object called responseList
            var responseList = new List<(string, List<string>)>();

            if (!File.Exists(filePath))//check if file exists
            {
                Console.WriteLine("File does not exist");
                return responseList;

            }

            foreach (var line in File.ReadAllLines(filePath))//loops through each line of the array
            {
                var parts = line.Split('=');//each line is going to be separated in to two parts, the "=" is where the line is going to split

                if (parts.Length == 2)//error handling for if the line does not have an equal sign
                {
                    var keyword = parts[0].ToLower();
                    var responses = parts[1].Split('|').Select(r => r.Trim()).ToList();
                    responseList.Add((keyword, responses));//a new string array is created and stored into the array list
                }
            }

            return responseList; //returns and arraylist responseList
        }//end of LoadResponsesFromFile


        private string lastTopicAsked = null;


        //creating a method FindBestResponse to look for a suitable response  
        public async Task<bool> FindBestResponse(List<(string Keyword, List<string> Response)> responseList, string userInput, Action<string> uiCallback)//this method passes an arraylist and a string as parameters
        {
            try
            {

                //check if user input is not empty or contain number and charaecters
                if (!isValidName(userInput)) return false;


                userInput = userInput.ToLower();
                File.AppendAllText("log.txt", $"[INPUT] User: {userInput}\n");
                string sentiment = DetectSentiment(userInput);//assign sentiment variable to detect sentiment method

                //if sentiment found, store to file
                if (sentiment != null) fileManager.SaveData("lastSentiment", sentiment);

                //if a follow up keyword is detected - parsing methods
                if (IsFollowUp(userInput))
                {
                    if (await HandleFollowUpResponse(responseList, uiCallback))
                        return true;
                    else
                        lastTopicAsked = null; // Clear bad topic reference
                }
                //assigning matches topics amd matching responses to matches Topics method
                var (matchedTopics, matchingResponses) = MatchesTopics(responseList, userInput);



                bool foundKeywordMatch = await DisplayResponses(matchedTopics, matchingResponses, sentiment, uiCallback);

                if (foundKeywordMatch) return true;

                //if prediction is not matched, try the ML.Net prediction
                string predicted = GetMLResponse(userInput);
                File.AppendAllText("log.txt", $"[ML PREDICTED] {predicted}\n");

                if (!string.IsNullOrEmpty(predicted))
                {
                    await TypingEffect($"{botName}: {predicted}", ConsoleColor.Green, uiCallback);
                    return true;
                }
                else
                {
                    unmatchedLogger.Log(userInput);

                    return false;
                }
            }

            catch (Exception ex)
            {
                string errorMsg = $"[ERROR] in FindBestResponse: {ex.Message}\n{ex.StackTrace}";
                File.AppendAllText("log.txt", errorMsg);
                await TypingEffect($"{botName}: I'm experiencing a technical issue. Please try again later.", ConsoleColor.Red, uiCallback);
            }

            return false;

        }//end of find best response method


        //method to detect follow up keywords
        private bool IsFollowUp(string userInput)
        {
            //return follow up response if user input contains one keyword
            if (string.IsNullOrEmpty(lastTopicAsked)) return false;

            var followUpPhrases = new List<string>
            {
                "tell me more",
                "more info",
                "i don't understand",
                "can you elaborate",
                "what else",
                "please explain further",
                "help me understand",
                "go deeper",
                "break it down",
                "simplify it",
                "i’m still confused",
                "give me more detail",
                "what does that mean",
                "say more about that",
                "i need a clearer explanation"
            };

            userInput = userInput.ToLower();
            return followUpPhrases.Any(phrase => userInput.Contains(phrase));

        }//end of IsFollowUp method

        //method to handle follow up question
        private async Task<bool> HandleFollowUpResponse(List<(string Keyword, List<string> Response)> responseList, Action<string> uiCallback)
        {
            //find first keyword in the response list that matches lastTopicAsked
            var followUp = responseList.FirstOrDefault(r => r.Keyword == lastTopicAsked);
            if (followUp.Response != null && followUp.Response.Count > 0)
            {
                //generate a random follow up response
                string response = followUp.Response[new Random().Next(followUp.Response.Count)];
                await TypingEffect($"{botName}: Sure! {response}", ConsoleColor.Green, uiCallback);
                return true;
            }
            return false;
        }//end of handleFollowUpResponse

        //method to find matched topics
        private (List<string> matchedTopics, List<string> matchingResponses) MatchesTopics(
           List<(string Keyword, List<string> Response)> responseList, string userInput)
        {
            var matchedTopics = new List<string>();
            var matchingResponses = new List<string>();

            //store sentiment keywords
            var sentimentKeywords = new List<string> { "worried", "frustrated", "curious", "anxious", "confused", "sad", "stressed", "interested" };

            //detect intent - what the user wants to know, either a defination, or explination
            //check if question is defination 
            var wantsDefinition = Regex.IsMatch(userInput, @"\b(what is|define|meaning of)\b", RegexOptions.IgnoreCase);
            var wantsExplanation = Regex.IsMatch(userInput, @"\b(explain|how does|describe)\b", RegexOptions.IgnoreCase);

            //foreach loop that loops through the list in pairs
            foreach (var (keyword, responses) in responseList)
            {
                if (sentimentKeywords.Contains(keyword)) continue;

                string baseKeyword = keyword;
                string intent = "tip";

                if (keyword.StartsWith("define_")) { baseKeyword = keyword.Substring(7); intent = "define"; }
                else if (keyword.StartsWith("explain_")) { baseKeyword = keyword.Substring(8); intent = "explain"; }

                string pattern = $"\\b{Regex.Escape(baseKeyword)}\\b";

                if (baseKeyword.Length < 3) continue;

                if (!Regex.IsMatch(userInput, pattern, RegexOptions.IgnoreCase)) continue;

                // Extra check: only match if at least one intent word is found (for tips, definitions, etc.)
                if ((intent == "define" && !wantsDefinition) ||
                    (intent == "explain" && !wantsExplanation) ||
                    (intent == "tip" && (wantsDefinition || wantsExplanation)))
                {
                    continue;
                }


                if (!matchedTopics.Contains(keyword)) matchedTopics.Add(keyword);

                string response = responses[new Random().Next(responses.Count)];
                var previousTopics = fileManager.LoadList($"favouriteTopics_{userName}");
                bool isRemembered = previousTopics.Contains(baseKeyword, StringComparer.OrdinalIgnoreCase);




                fileManager.AppendToList($"favouriteTopics_{userName}", baseKeyword);

                string formattedResponse = "";

                if (intent == "define")
                    formattedResponse = $"- Definition: {response}";
                else if (intent == "explain")
                    formattedResponse = $"- Explanation: {response}";
                else
                    formattedResponse = $"- Tip: {response}";

                if (isRemembered)
                    matchingResponses.Add($"{botName}: I remember we discussed {baseKeyword} earlier.\n{formattedResponse}");
                else
                    matchingResponses.Add($"{botName}: Here's what I found about {baseKeyword}:\n{formattedResponse}");


                fileManager.SaveData("lastTopic", baseKeyword);
                lastTopicAsked = baseKeyword;
            }
            return (matchedTopics, matchingResponses);
        }//end of find matching response method

        //method to display reponse
        private async Task<bool> DisplayResponses(List<string> matchedTopics, List<string> matchingResponses, string sentiment, Action<string> uiCallback)
        {
            if (matchedTopics.Count == 0) return false; //if no matched topic

            if (!string.IsNullOrEmpty(sentiment))//if sentiment id found
            {
                string comfort = $"{botName}: I'm sorry you're feeling {sentiment}. Here are some tips that can help you.";
                string tips = string.Join("\n", matchingResponses.Select(r => $"{r}"));
                await TypingEffect($"{comfort}\n{tips}", ConsoleColor.Yellow, uiCallback);

            }
            else
            {
                var groupedByTopic = matchingResponses
                .GroupBy(r => r.Split(':')[1].Split('\n')[0].Trim()) // Group by topic line
                .ToList();

                foreach (var group in groupedByTopic)
                {
                    string heading = group.Key;
                    await TypingEffect($"{botName}:{heading}", ConsoleColor.Green, uiCallback);

                    foreach (var response in group)
                    {
                        var lines = response.Split('\n').Skip(1); // Skip heading line
                        foreach (var line in lines)
                        {
                            await TypingEffect(line, ConsoleColor.Green, uiCallback);
                        }
                    }
                }
            }

            return true;

        }//Diplay reposnes method

        //a method that detects sentiment
        private string DetectSentiment(string input)
        {
            input = input.ToLower();
            if (input.Contains("worried") || input.Contains("anxious")) return "worried";
            if (input.Contains("curious") || input.Contains("interested")) return "curious";
            if (input.Contains("frustrated") || input.Contains("confused") || input.Contains("sad") || input.Contains("stressed")) return "frustrated";
            return null;
        }

        // a methods that adds a typing effect to the chatbot's response
        public async Task TypingEffect(string message, ConsoleColor color, Action<string> outputCallback)// this method parses a string, a colour and int as a parameter
        {
            int speed = 20;
            Console.ForegroundColor = color;

            string typed = "";
            foreach (char c in message)
            {
                typed += c;
                await Task.Delay(speed);  // Delay but don't update UI yet
            }

            outputCallback?.Invoke(typed); // Show entire message at once in a single bubble
            Console.ResetColor();
        }//end of typing effect

    }//end of class
}

