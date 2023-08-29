using Microsoft.ML.Data;

public class SentimentPrediction
{
    [ColumnName("PredictedLabel")]
    public bool Prediction;

    public float Score; // Add this property for sentiment score
}
