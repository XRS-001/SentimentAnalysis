using System;
using Microsoft.ML;
using Microsoft.ML.Data;

using Microsoft.ML.Transforms;

public class ModelTrainer
{
    public static void TrainModelAndSave()
    {
        var mlContext = new MLContext();

        var data = mlContext.Data.LoadFromTextFile<Review>("train-2.csv", separatorChar: ',', hasHeader: false);
        var textPipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(Review.Text));
        var pipeline = textPipeline.Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression());
        var model = pipeline.Fit(data);

        mlContext.Model.Save(model, data.Schema, "SentimentModel.zip");
        Console.WriteLine("Model trained and saved.");
    }
}













