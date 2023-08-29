using System;
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


