using System;
using Microsoft.ML.Data;
public class Review
{
    [LoadColumn(1)] public string Text;
    [LoadColumn(0)] public string Label;
}





