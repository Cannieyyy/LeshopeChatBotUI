using Microsoft.ML.Data;

namespace LeshopeChatBotUI
{
    //deinfining a class to initialize the userInput and the response of the trained data
    public class ChatData
    {
        //declaring a sting variable to store the userInput of train data
        public string UserInput { get; set; }

        //declaring a sting variable to store the response of train data
        public string Response { get; set; }


    }//end of ChatData class

    //class to initialize the response and the score of the prideiction
    public class ChatPrediction
    {
        [ColumnName("PredictedLabel")]

        public string Response { get; set; }

        // this is to store the ratings of prediction to find the closest match
        public float[] Score { get; set; }
    
    }//end of ChatPrediction class

}//end of name space
