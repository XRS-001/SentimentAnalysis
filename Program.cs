using System;
using Microsoft.ML;
using System.Net;
using HtmlAgilityPack;
namespace SentimentAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1: Train model");
            Console.WriteLine("2: Run sentiment analysis on Steam game reviews");
            Console.WriteLine("3: Predict sentiment for a sentence");
            int choice = int.Parse(Console.ReadLine());

            if (choice == 1)
            {
                ModelTrainer.TrainModelAndSave();
            }
            else if (choice == 2)
            {
                PredictionRunner.LoadModelAndPredict();
            }
            else if (choice == 3)
            {
                PredictSentenceSentiment();
            }
        }

        static void PredictSentenceSentiment()
        {
            while (true)
            {
                var mlContext = new MLContext();
                var model = mlContext.Model.Load("SentimentModel.zip", out var modelSchema);
                var predictionEngine = mlContext.Model.CreatePredictionEngine<Review, SentimentPrediction>(model);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Enter a sentence or press enter to exit:");
                Console.ForegroundColor = ConsoleColor.White;
                string sentence = Console.ReadLine();
                if (sentence == "")
                {
                    break;
                }

                var prediction = predictionEngine.Predict(new Review { Text = sentence });

                string sentiment = prediction.Prediction ? "Positive" : "Negative";
                Console.WriteLine($"Predicted Sentiment: {sentiment}");
                Console.WriteLine($"Certainty: {Math.Min(Math.Abs(0.5f - ((prediction.Score + 1) / 2)) * 200, 100):F2}%");
            }
        }
    }
}



