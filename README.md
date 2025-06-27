# Cyber Security Awareness ChatBot
***********************************

## Overview

This project is a chatbot application designed to raise awareness about cyber security. Originally a C# console app, it is now enhanced with a **user interface (UI)** and advanced features such as quizzes, NLP data training, and spell checking. The chatbot provides information on safe online practices, passwords, phishing, encryption, and more. It engages with users conversationally, guiding them through various cyber security topics to build digital safety habits.

====================================================================================================================================

## Features

- **User Interaction:** Asks for user name and personalizes conversation flow.
- **Customizable Bot Name:** Users can rename the bot anytime.
- **Cyber Security Awareness Responses:** Covers topics like phishing, malware, encryption, identity theft, and general cybersecurity.
- **Spell Checker:** Integrated spelling correction algorithm to improve input accuracy and matching.
- **NLP Training Data:** Uses JSON-formatted datasets to store multiple keyword explanations and sentiment responses for richer dialogue.
- **Microsoft ML Integration:** Utilizes Microsoft ML libraries for future classification and intent detection enhancements.
- **Quiz Module:** Includes multiple choice, true/false, and fill-in-the-blank questions to assess user knowledge interactively.
- **UI Application:** Upgraded from console to a UI-based application for improved accessibility and professional deployment.

====================================================================================================================================

## Prerequisites

- .NET Framework or compatible .NET runtime for your C# version.
- **Responses File:** `responses.txt` with `keyword=response` pairs or updated JSON datasets for NLP-based responses.
- **Microsoft ML NuGet Packages:** For ML-based intent detection, classification, or text analysis modules.

====================================================================================================================================

## Setup and Installation

1. Clone this repository to your local machine.
2. Open the project in Visual Studio or your preferred C# IDE.
3. Ensure `responses.txt` or your JSON training files are placed in the project root directory.
4. Restore NuGet packages (including Microsoft ML) if using ML-based modules.
5. Build and run the project.

====================================================================================================================================

## How to Use

1. Launch the application.
2. The bot will greet you and ask for your name.
3. Choose to rename the bot if you wish.
4. Ask questions about cyber security topics (e.g., "What is encryption?" or "How do I avoid phishing scams?").
5. The bot will respond using keyword-based or NLP-trained explanations.
6. Try out the **Quiz module** for knowledge assessment, including multiple choice, true/false, and fill-in-the-blank questions.

====================================================================================================================================

## Code Components


- **LoadResponsesFromFile Method:** Loads responses from `responses.txt` or JSON datasets.
- **FindBestResponse Method:** Matches user input to keywords or trained intents for relevant replies.
- **Spell Checker Algorithm:** Corrects user typos before matching keywords or intents.
- **Quiz Modules:** Three categories (multiple choice, true/false, fill-in-the-blank) to reinforce cyber security learning.
- **UI Forms:** Modern user interface for better usability compared to the original console app.
- **Microsoft ML Components:** Integrated for classification, NLP enhancements, and potential chatbot training extensions.

====================================================================================================================================

## Example Interaction

