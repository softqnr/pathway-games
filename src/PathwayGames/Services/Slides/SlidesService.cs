using Newtonsoft.Json;
using PathwayGames.Models;
using PathwayGames.Models.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PathwayGames.Services.Slides
{
    public class SlidesService : ISlidesService
    {
        private static List<Slide> Slides {
            get
            {
                List<Slide> SlideCollection = new List<Slide>();
                // X
                for (int i = 0; i < 30; i++)
                {
                    SlideCollection.Add(new Slide(SlideType.X, 1.5) { Name = "Mickey Mouse " + i, Image = "mickey.jpg" });
                }
                // DistractorY
                for (int i = 0; i < 30; i++)
                {
                    SlideCollection.Add(new Slide(SlideType.Y, 1.5) { Name = "Donald " + i, Image = "donald.jpg" });
                }
                // Reward
                for (int i = 0; i < 10; i++)
                {
                    SlideCollection.Add(new Slide(SlideType.Reward, 2) { Name = "Reward " + i, Image = "reward.gif", Sound = "success.mp3" });
                }
                return SlideCollection;
            }
        }

        public Game Generate(GameType gameType, GameSettings gameSettings, string userName, string randomSeed)
        {
            Game game = new Game() { UserName = userName, Seed = randomSeed };
            // Generate random number generator
            Random random = randomSeed != "" ? new Random(randomSeed.GetHashCode()) : new Random();
            // Pick slides
            game.Slides = Slides.Where(x => x.SlideType == SlideType.X)
                    .OrderBy(i => random.Next())
                    .Take((int)(gameSettings.SlideCount * 0.7))
                .Concat<Slide>(
                    Slides.Where(x => x.SlideType == SlideType.Y)
                    .OrderBy(i => random.Next())
                    .Take((int)(gameSettings.SlideCount * 0.3))
                ).OrderBy(i => random.Next())
                .Select(x => { x.BlankDuration = GetRandomNumber(gameSettings.BlankSlideDisplayTimes); return x; })
                .ToList<Slide>();
            // Add blank slides between
            //slides = slides.SelectMany(
            //    x => new[] { x, new Slide(SlideType.Blank, GetRandomNumber(new[] { 1, 1.2 })) })
            //    .ToList();

            return game;
        }
        public Slide GetRandomRewardSlide()
        {
            Random random = new Random();
            var slide = Slides.Where(x => x.SlideType == SlideType.Reward)
                    .OrderBy(i => random.Next())
                    .First();
            return slide;
        }
        public Game Load(string filePath)
        {
            return JsonConvert.DeserializeObject<Game>(filePath);
        }
        public void Save(Game game)
        {
            string json = JsonConvert.SerializeObject(game);
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filePath = Path.Combine(path, "game.json");
            using (var file = File.Open(filePath, FileMode.Create, FileAccess.Write))
            using (var sw = new StreamWriter(file))
            {
                sw.Write(json);
            }
        }
        public void CalculateGameScoreAndStats(Game game)
        {
            // Score
            game.Score = game.Slides.Select(x => x.Points).Sum();
            // ScorePercentage
            int correctCount = game.Slides.Where(x => x.ResponseOutcome == ResponseOutcome.CorrectCommission ||
                                                      x.ResponseOutcome == ResponseOutcome.CorrectOmission).Count();
            int wrongCount = game.Slides.Count - correctCount;
            game.ScorePercentage = ((double)(correctCount - wrongCount) / game.Slides.Count) * 100;
            // Average Response Time
            game.AverageResponseTime = TimeSpan.FromMilliseconds(game.Slides.
                Where(x => x.ResponseOutcome == ResponseOutcome.CorrectCommission || x.ResponseOutcome == ResponseOutcome.WrongCommission)
                .Select(x => x.ResponseTime.TotalMilliseconds).DefaultIfEmpty().Average());
            // Average Response Time Correct
            game.AverageResponseTimeCorrect = TimeSpan.FromMilliseconds(game.Slides.
                Where(x => x.ResponseOutcome == ResponseOutcome.CorrectCommission)
                .Select(x => x.ResponseTime.TotalMilliseconds).DefaultIfEmpty().Average());
            // Average Response Time Wrong

            game.AverageResponseTimeWrong = TimeSpan.FromMilliseconds(game.Slides.
                Where(x => x.ResponseOutcome == ResponseOutcome.WrongCommission)
                .Select(x => x.ResponseTime.TotalMilliseconds).DefaultIfEmpty().Average());
        }
        public ResponseOutcome EvaluateSlideResponse(Game game, Slide slide)
        {
            Int32? slideIndex = game.Slides.IndexOf(slide);

            var firstButtonPress = game.ButtonPresses.Where(x => x.SlideIndex == slideIndex).FirstOrDefault();

            if (firstButtonPress != null)
            {
                // Calculate response time
                slide.ResponseTime = firstButtonPress.Time.Subtract(slide.SlideDisplayed);
            }
            switch (slide.SlideType)
            {
                case SlideType.X:
                    if (firstButtonPress != null)
                    {
                        slide.ResponseOutcome = ResponseOutcome.CorrectCommission;
                        slide.Points = 2;
                    }
                    else
                    {
                        slide.ResponseOutcome = ResponseOutcome.WrongOmission;
                        slide.Points = -1;
                    }
                    break;
                case SlideType.Y:
                    if (firstButtonPress == null)
                    {
                        slide.ResponseOutcome = ResponseOutcome.CorrectOmission;
                        slide.Points = 1;
                    }
                    else
                    {
                        slide.ResponseOutcome = ResponseOutcome.WrongCommission;
                        slide.Points = -2;
                    }
                    break;
                case SlideType.A:
                    break;
                case SlideType.B:
                    break;
                default:
                    break;
            }
            return slide.ResponseOutcome;
        }

        private static double GetRandomNumber(double[] values)
        {
            Random random = new Random();
            int index = random.Next(0, values.Count());
            return values[index];
        }
    }
}
