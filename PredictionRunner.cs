using System;
using System.Diagnostics;
using HtmlAgilityPack;
using System.Net;
using Microsoft.ML;
using Newtonsoft.Json.Linq;

public class PredictionRunner
{
    public static void LoadModelAndPredict()
    {
        var mlContext = new MLContext();
        var model = mlContext.Model.Load("SentimentModel.zip", out var modelSchema);
        var predictionEngine = mlContext.Model.CreatePredictionEngine<Review, SentimentPrediction>(model);

        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Pick a game");
            Console.ForegroundColor = ConsoleColor.White;
            string gamePicked = Console.ReadLine();
            gamePicked = CapitalizeFirstLetter(gamePicked);
            int appId = GetAppId(gamePicked);
            Console.WriteLine("Calculating...");

            if (appId != 0)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start(); // Start the stopwatch
                string cursor = "*";
                int totalPositiveReviews = 0;
                int totalNegativeReviews = 0;
                int totalReviews = 0;
                float totalCertainty = 0;
                int reviewsLimit = 1000;

                while (!string.IsNullOrEmpty(cursor) && totalReviews < reviewsLimit)
                {
                    var steamApiUrl = $"https://store.steampowered.com/appreviews/{appId}?json=1&filter=all&language=all&num_per_page=100&cursor={cursor}";

                    using (var client = new WebClient())
                    {
                        client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3");

                        var json = client.DownloadString(steamApiUrl);
                        var response = JObject.Parse(json);

                        var reviews = response["reviews"];
                        cursor = response["cursor"].ToString();

                        foreach (var review in reviews)
                        {
                            totalReviews++;
                            string reviewText = review["review"].ToString();
                            var prediction = predictionEngine.Predict(new Review { Text = reviewText });

                            float certainty = Math.Min(Math.Abs(0.5f - ((prediction.Score + 1) / 2)) * 200, 100);
                            totalCertainty += certainty;

                            bool predictedSentiment = prediction.Prediction;
                            if (!predictedSentiment && certainty < 70)
                            {
                                predictedSentiment = true;
                            }

                            if (predictedSentiment)
                            {
                                totalPositiveReviews++;
                            }
                            else
                            {
                                totalNegativeReviews++;
                            }
                        }
                    }
                }
                stopwatch.Stop();
                double positivePercentage = (double)totalPositiveReviews / totalReviews * 100;
                double negativePercentage = (double)totalNegativeReviews / totalReviews * 100;

                if (totalReviews > 0)
                {
                    Console.WriteLine($"Positive Reviews: {positivePercentage:F2}%");
                    Console.WriteLine($"Negative Reviews: {negativePercentage:F2}%");
                    Console.WriteLine($"Certainty: {(totalCertainty / totalReviews):F2}%");
                    Console.WriteLine($"Elapsed Time: {stopwatch.Elapsed.TotalSeconds:F2} seconds");
                }
                else
                {
                    Console.WriteLine("Could not scrape reviews");
                }
            }
            else
            {
                Console.WriteLine("Game not found");
            }
        }
    }
    static string CapitalizeFirstLetter(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        string[] words = input.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            char[] charArray = words[i].ToCharArray();
            if (charArray.Length > 0)
            {
                charArray[0] = char.ToUpper(charArray[0]);
                words[i] = new string(charArray);
            }
        }

        return string.Join(" ", words);
    }
    static int GetAppId(string gameName)
    {
        var url = $"https://store.steampowered.com/search/?term={gameName}&type=vg&count=100";
        using (var client = new WebClient())
        {
            var html = client.DownloadString(url);

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var gameNode = doc.DocumentNode.SelectSingleNode("//div[@id='search_resultsRows']/a");
            string gameNameLower = gameName.ToLower();
            string innerTextLower = doc.DocumentNode.SelectSingleNode("/html/body/div[1]/div[7]/div[6]/form/div[1]/div/div[1]/div[3]/div/div[3]/a[1]/div[2]/div[1]/span")?.InnerText.ToLower();
            if (gameNode != null && gameNameLower == innerTextLower)
            {
                var href = gameNode.Attributes["href"].Value;
                var parts = href.Split('/');
                return int.Parse(parts[4]);
            }
            else
            {
                Console.WriteLine("Game not found");
                return 0;
            }
        }
    }
}