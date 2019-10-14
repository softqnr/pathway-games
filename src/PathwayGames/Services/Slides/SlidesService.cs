using Newtonsoft.Json;
using PathwayGames.Infrastructure.Json;
using PathwayGames.Models;
using PathwayGames.Models.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PathwayGames.Extensions;

namespace PathwayGames.Services.Slides
{
    public class SlidesService : ISlidesService
    {
        const string XSlideImage = "casey_the_cat.jpg";
        const string YDistractorSlideImage = "berry_the_dog.jpg";
        readonly string[] RewardSlideImages = new[] { "reward_animation_wow.gif" };

    public Game Generate(GameType gameType, UserGameSettings gameSettings, long userId, string userName, string seed)
        {
           if (string.IsNullOrWhiteSpace(seed))
                seed = CreateRandomSeed();

            // Create game
            Game game = new Game(gameType, gameSettings, userId, userName, seed);

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
            // Check if within blank duration
            if (slide.SlideHidden.HasValue)
            {
                var blankSlideTimeUsed = (DateTime.Now - slide.SlideHidden.Value);
                timeLeft = TimeSpan.FromSeconds(slide.BlankDuration) - blankSlideTimeUsed;
            }
            return timeLeft;
        }

        private List<Slide> GenerateTypeXSlideSequence(UserGameSettings gameSettings, string seed)
        {
             // Create slide collection using the 70%X and 30%Distractor 
            List<Slide> SlideCollection = new List<Slide>();
            // X 
            int typeXSlideCount = (int)(gameSettings.SlideCount * 0.7);
            for (int i = 0; i < typeXSlideCount; i++)
            {
                SlideCollection.Add(new Slide(SlideType.X, gameSettings.SlideDisplayDuration) {
                    Image = XSlideImage,
                    BlankDuration = GetRandomNumber(gameSettings.BlankSlideDisplayTimes)
                });
            }
            // DistractorY
            for (int i = 0; i < gameSettings.SlideCount - typeXSlideCount; i++)
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

        public void FinalizeGame(Game game)
        {
            game.SessionData.EndDate = DateTime.Now;
            // Calculate game stats
            CalculateGameScoreAndStats(game);
            // Calculate engangement
            game.Outcome.ConfusionMatrix = CalculateConfusionMatrix(game.Slides);  
            // Save game data to file
            Save(game);
        }

        public ConfusionMatrix CalculateConfusionMatrix(List<Slide> slides)
        {
            return new ConfusionMatrix(slides);
        }

        public Game Load(string fileName)
        {
            string filePath = Path.Combine(App.LocalStorageDirectory, fileName);
            return JsonConvert.DeserializeObject<Game>(filePath);
        }

        public string Save(Game game)
        {
            game.GameDataFile = Guid.NewGuid().ToString() + ".json";
            string filePathName = Path.Combine(App.LocalStorageDirectory, game.GameDataFile);
 
            SaveGameToJson(game, filePathName);

            if (game.SensorDataFile != "")
            {
                MergeDataFiles(filePathName, Path.Combine(App.LocalStorageDirectory, game.SensorDataFile));
            }

            return filePathName;
        }

        private void SaveGameToJson(Game game, string filePathName)
        {
            using (var file = File.Open(filePathName, FileMode.Create, FileAccess.Write))
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
        }

        private void MergeDataFiles(string gameDataFile, string gameSensorFile)
        {
            const string FaceAnchorJson = "\"FaceAnchorData\": []";
            using (var file = File.Open(gameDataFile, FileMode.Open, FileAccess.ReadWrite))
            {
                file.Seek(FaceAnchorJson);
                using (var sw = new StreamWriter(file, Encoding.UTF8))
                {
                    using (var sensorFile = File.Open(gameSensorFile, FileMode.Open, FileAccess.Read))
                    {
                        sensorFile.CopyTo(file);
                        file.WriteByte(Convert.ToByte('}'));
                        file.WriteByte(Convert.ToByte('}'));
                    }
                }
            }
        }

        public void CalculateGameScoreAndStats(Game game)
        {
            // Score
            game.Outcome.Score = game.Slides.Select(x => x.Points).Sum();
            // ScorePercentage
            int correctCount = game.Slides.Where(x => x.ResponseOutcome == ResponseOutcome.CorrectCommission ||
                                                      x.ResponseOutcome == ResponseOutcome.CorrectOmission).Count();
            int wrongCount = game.Slides.Count - correctCount;
            game.Outcome.ScorePercentage = ((double)(correctCount - wrongCount) / game.Slides.Count) * 100;
            // Average Response Time
            game.Outcome.AverageResponseTime = TimeSpan.FromMilliseconds(game.Slides.
                Where(x => x.ResponseOutcome == ResponseOutcome.CorrectCommission || x.ResponseOutcome == ResponseOutcome.WrongCommission)
                .Select(x => x.ResponseTime.TotalMilliseconds).DefaultIfEmpty().Average());
            // Average Response Time Correct
            game.Outcome.AverageResponseTimeCorrect = TimeSpan.FromMilliseconds(game.Slides.
                Where(x => x.ResponseOutcome == ResponseOutcome.CorrectCommission)
                .Select(x => x.ResponseTime.TotalMilliseconds).DefaultIfEmpty().Average());
            // Average Response Time Wrong
            game.Outcome.AverageResponseTimeWrong = TimeSpan.FromMilliseconds(game.Slides.
                Where(x => x.ResponseOutcome == ResponseOutcome.WrongCommission)
                .Select(x => x.ResponseTime.TotalMilliseconds).DefaultIfEmpty().Average());
        }

        public ResponseOutcome EvaluateSlideResponse(Game game, Slide slide)
        {
            Int32? slideIndex = game.Slides.IndexOf(slide);

            var firstButtonPress = game.SensoryData.ButtonPresses.Where(x => x.SlideIndex == slideIndex).FirstOrDefault();

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
