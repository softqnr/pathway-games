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
                    SlideCollection.Add(new Slide(SlideType.X, 1.5) { Name = "Mickey Mouse " + i,Image = "mickey.jpg" });
                }
                // DistractorY
                for (int i = 0; i < 30; i++)
                {
                    SlideCollection.Add(new Slide(SlideType.Y, 1.5) { Name = "Donald " + i, Image = "donald.jpg" });
                }
                // Reward
                for (int i = 0; i < 10; i++)
                {
                    SlideCollection.Add(new Slide(SlideType.Reward, 1.5) { Name = "Reward " + i, Image = "reward.jpg" });
                }
                return SlideCollection;
            }
        }

        public IList<Slide> Generate(GameType gameType, GameSettings gameSettings, string randomSeed)
        {
            // Generate random number generator
            Random random = randomSeed != "" ? new Random(randomSeed.GetHashCode()) : new Random();
              // Pick slides
            var slides = Slides.Where(x => x.SlideType == SlideType.X)
                    .OrderBy(i => random.Next())
                    .Take(gameSettings.SlideCount / 2)
                .Concat<Slide>(
                    Slides.Where(x => x.SlideType == SlideType.Y)
                    .OrderBy(i => random.Next())
                    .Take(gameSettings.SlideCount / 2)
                ).OrderBy(i => random.Next())
                .ToList<Slide>();
            // Add blank slides between
            slides = slides.SelectMany(
                x => new[] { x, new Slide(SlideType.Blank, GetRandomNumber(new[] { 1, 1.2 })) })
                .ToList();

            return slides;
        }
        public Slide GetBlankSlide()
        {
            Random random = new Random();
            var slide = new Slide(SlideType.Blank, GetRandomNumber(new[] { 1, 1.2 }));
            return slide;
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
        public double CalculateGameScoreAndPercentage(Game game)
        {
            //percentage: correct-wrong/total-slides * 100
            return 100;
        }
        public bool EvaluateSlideResponse(Slide slide)
        {
            bool result = false;
            if (slide.ButtonPresses.Count > 0)
            {
                // Calculate response time
                slide.ResponseTime = slide.ButtonPresses[0].Time - slide.SlideDisplayed;
            }
            switch (slide.SlideType)
            {
                case SlideType.X:
                    if (slide.ButtonPresses.Count > 0)
                    {

                    }
                    break;
                case SlideType.Y:
                    break;
                case SlideType.A:
                    break;
                case SlideType.B:
                    break;
                case SlideType.Reward:
                    break;
                case SlideType.Blank:
                    if (slide.ButtonPresses.Count == 0)
                    {
                        slide.ResponseOutcome = ResponseOutcome.CorrectOmission;
                        slide.Points = 1;
                    }
                    break;
                default:
                    break;
            }
            return result;
        }

        private static double GetRandomNumber(double[] values)
        {
            Random random = new Random();
            int index = random.Next(0, values.Count());
            return values[index];
        }
    }
}
