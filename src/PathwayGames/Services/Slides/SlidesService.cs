using Newtonsoft.Json;
using PathwayGames.Infrastructure.Json;
using PathwayGames.Models;
using PathwayGames.Models.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace PathwayGames.Services.Slides
{
    public class SlidesService : ISlidesService
    {
        const string XSlideImage = "casey_the_cat.jpg";
        const string YDistractorSlideImage = "berry_the_dog.jpg";
        readonly string[] RewardSlideImages = new[] { "reward_animation_wow.gif" };

    public Game Generate(GameType gameType, UserGameSettings gameSettings, string userName, string seed)
        {
           if (string.IsNullOrWhiteSpace(seed))
                seed = CreateRandomSeed();

            // Create game
            Game game = new Game(gameType, gameSettings, userName, seed);

            // Pick game slides
            switch (gameType)
            {
                case GameType.TypeX:
                    game.Slides = GenerateTypeXSlideSequence(gameSettings, seed);
                    break;
                case GameType.SeekX:
                    game.Slides = GenerateTypeAXSlideSequence(gameSettings, seed);
                    break;
                case GameType.TypeAX:
                    break;
                case GameType.SeekAX:
                    break;
                case GameType.SeekAXQuiz:
                    break;
                default:
                    break;
            }

            return game;
        }

        public TimeSpan CalculateBlankSlideTimeLeft(Slide slide)
        {
            var timeLeft = new TimeSpan();
            var currentDateTime = DateTime.Now;
            // Check if within blank duration
            if (slide.SlideHidden.HasValue)
            {
                var blankSlideTimeUsed = (currentDateTime - slide.SlideHidden.Value);
                timeLeft = TimeSpan.FromSeconds(slide.BlankDuration) - blankSlideTimeUsed;
            }
            return timeLeft;
        }

        private List<Slide> GenerateTypeXSlideSequence(UserGameSettings gameSettings, string seed)
        {
             // Create slide collection using the 70%X and 30%Distractor 
            List<Slide> SlideCollection = new List<Slide>();
            // X 
            for (int i = 0; i < (int)gameSettings.SlideCount * 0.7; i++)
            {
                SlideCollection.Add(new Slide(SlideType.X, gameSettings.SlideDisplayDuration) {
                    Image = XSlideImage,
                    BlankDuration = GetRandomNumber(gameSettings.BlankSlideDisplayTimes)
                });
            }
            // DistractorY
            for (int i = 0; i < (int)gameSettings.SlideCount * 0.3; i++)
            {
                SlideCollection.Add(new Slide(SlideType.Y, gameSettings.SlideDisplayDuration) {
                    Image = YDistractorSlideImage,
                    BlankDuration = GetRandomNumber(gameSettings.BlankSlideDisplayTimes)
                });
            }
            // Generate random number generator
            Random random = new Random(seed.GetHashCode());
            // Shuffle
            return SlideCollection.OrderBy(i => random.Next()).ToList<Slide>();
        }

        private List<Slide> GenerateTypeAXSlideSequence(UserGameSettings gameSettings, string seed)
        {
            // Create slide collection using the 70%AX 10%BY 10%AY 10%BX
            List<Slide> SlideCollection = new List<Slide>();
            // AX
            for (int i = 0; i < (int)gameSettings.SlideCount * 0.7; i++)
            {
                SlideCollection.Add(new Slide(SlideType.A, gameSettings.SlideDisplayDuration) { Image = "alex_the_alien.jpg", BlankDuration = GetRandomNumber(gameSettings.BlankSlideDisplayTimes) });
                SlideCollection.Add(new Slide(SlideType.X, gameSettings.SlideDisplayDuration) { Image = XSlideImage, BlankDuration = GetRandomNumber(gameSettings.BlankSlideDisplayTimes) });
            }
            // BY
            for (int i = 0; i < (int)gameSettings.SlideCount * 0.1; i++)
            {
                SlideCollection.Add(new Slide(SlideType.B, gameSettings.SlideDisplayDuration) { Image = "alex_the_alien.jpg", BlankDuration = GetRandomNumber(gameSettings.BlankSlideDisplayTimes) });
                SlideCollection.Add(new Slide(SlideType.Y, gameSettings.SlideDisplayDuration) { Image = YDistractorSlideImage, BlankDuration = GetRandomNumber(gameSettings.BlankSlideDisplayTimes) });
            }
            // AY
            for (int i = 0; i < (int)gameSettings.SlideCount * 0.1; i++)
            {
                SlideCollection.Add(new Slide(SlideType.A, gameSettings.SlideDisplayDuration) { Image = "alex_the_alien.jpg", BlankDuration = GetRandomNumber(gameSettings.BlankSlideDisplayTimes) });
                SlideCollection.Add(new Slide(SlideType.Y, gameSettings.SlideDisplayDuration) { Image = YDistractorSlideImage, BlankDuration = GetRandomNumber(gameSettings.BlankSlideDisplayTimes) });
            }
            // BX
            for (int i = 0; i < (int)gameSettings.SlideCount * 0.1; i++)
            {
                SlideCollection.Add(new Slide(SlideType.B, gameSettings.SlideDisplayDuration) { Image = "alex_the_alien.jpg", BlankDuration = GetRandomNumber(gameSettings.BlankSlideDisplayTimes) });
                SlideCollection.Add(new Slide(SlideType.X, gameSettings.SlideDisplayDuration) { Image = XSlideImage, BlankDuration = GetRandomNumber(gameSettings.BlankSlideDisplayTimes) });
            }

            // Generate random number generator
            Random random = new Random(seed.GetHashCode());
            // Shuffle
            return SlideCollection.OrderBy(i => random.Next()).ToList<Slide>();
        }

        public Slide GetRandomRewardSlide(double displayDuration)
        {
            Random random = new Random();
           
            return new Slide(SlideType.Reward, displayDuration) {
                Image = RewardSlideImages[random.Next(RewardSlideImages.Length - 1)],
                Sound = "success.mp3"
            };
        }

        public Game Load(string fileName)
        {
            string path = FileSystem.AppDataDirectory;
            string filePath = Path.Combine(path, fileName);
            return JsonConvert.DeserializeObject<Game>(filePath);
        }

        public string Save(Game game)
        {
            string path = FileSystem.AppDataDirectory;
            //string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string fileName = Guid.NewGuid().ToString() + ".json";
            game.GameDataFile = fileName;
            //game.SensorDataFile
            string filePath = Path.Combine(path, fileName);
            using (var file = File.Open(filePath, FileMode.Create, FileAccess.Write))
            {
                using (var sw = new StreamWriter(file, Encoding.UTF8))
                {
                    // Ignore IsEmpty
                    var settings = new JsonSerializerSettings
                    {
                        ContractResolver = ShouldSerializeContractResolver.Instance
                    };
                    sw.Write(JsonConvert.SerializeObject(game, Formatting.Indented, settings));
                }
            }
            return fileName;
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

        private string CreateRandomSeed(int length = 6)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
