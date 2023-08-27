using Microsoft.ML.Data;

public class Review
{
    [LoadColumn(0)] public bool Label;
    [LoadColumn(1)] public string Text;
}



