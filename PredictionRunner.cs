using System;
using Microsoft.ML;
using Microsoft.ML.Data;

public class PredictionRunner
{
    public static void LoadModelAndPredict()
    {
        var mlContext = new MLContext();
        var model = mlContext.Model.Load("SentimentModel.zip", out var modelSchema);
        var predictionEngine = mlContext.Model.CreatePredictionEngine<Review, SentimentPrediction>(model);

        while (true)
        {
            Console.Write("Enter a review text (or 'exit' to quit): ");
            string input = Console.ReadLine();
            if (input.ToLower() == "exit")
                break;

            var prediction = predictionEngine.Predict(new Review { Text = input });
            string sentiment = prediction.Prediction ? "Positive" : "Negative";
            Console.WriteLine($"Predicted Sentiment: {sentiment}");
        }
    }
}








