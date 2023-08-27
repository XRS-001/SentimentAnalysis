using System;

namespace SentimentAnalysis
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Choose an option: 1. Train Model, 2. Run Predictions");
            int choice = int.Parse(Console.ReadLine());

            if (choice == 1)
            {
                ModelTrainer.TrainModelAndSave();
            }
            else if (choice == 2)
            {
                PredictionRunner.LoadModelAndPredict();
            }
        }
    }
}


