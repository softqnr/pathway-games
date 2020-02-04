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
using PathwayGames.Helpers;

namespace PathwayGames.Services.Slides
{
    public class SlidesService : ISlidesService
    {
        const string XSlideImage = "casey_the_cat.jpg";
        const string YDistractorSlideImage = "berry_the_dog.jpg";
        const string BContrastImage = "backpack.jpg";
        readonly string[] RewardSlideImages = new[] { "reward_animation_wow.gif" };
        const string RewardSound = "success.mp3";
        const int SeedLength = 5;

        public Game Generate(GameType gameType, UserGameSettings gameSettings, long userId, string userName, string seed = "")
        {
           if (string.IsNullOrWhiteSpace(seed))
                seed = ThreadSafeRandom.CreateRandomString(SeedLength);

            // Create game
            Game game = new Game(gameType, gameSettings, userId, userName, seed);

            // Pick game slides
            switch (gameType)
            {
                case GameType.TypeX:
                    game.Slides = GenerateTypeXSlideSequence(gameSettings, seed);
                    break;
                case GameType.SeekX:
                    game.Slides = GenerateSeekXSlideSequence(gameSettings, seed);
                    break;
                case GameType.TypeAX:
                    game.Slides = GenerateTypeAXSlideSequence(gameSettings, seed);
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
             // Create slide collection using the 50%X and 50%Distractor 
            List<Slide> SlideCollection = new List<Slide>();
            // X 
            int typeXSlideCount = (int)(gameSettings.SlideCount * 0.5);
            for (int i = 0; i < typeXSlideCount; i++)
            {
                SlideCollection.Add(new Slide(SlideType.X, gameSettings.SlideDisplayDuration, XSlideImage, ThreadSafeRandom.GetRandomNumber(gameSettings.BlankSlideDisplayTimes)));
            }
            // DistractorY
            for (int i = 0; i < gameSettings.SlideCount - typeXSlideCount; i++)
            {
                SlideCollection.Add(new Slide(SlideType.Y, gameSettings.SlideDisplayDuration, YDistractorSlideImage, ThreadSafeRandom.GetRandomNumber(gameSettings.BlankSlideDisplayTimes)));
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
                SlideCollection.Add(new Slide(SlideType.A, gameSettings.SlideDisplayDuration, "alex_the_alien.jpg", ThreadSafeRandom.GetRandomNumber(gameSettings.BlankSlideDisplayTimes)));
                SlideCollection.Add(new Slide(SlideType.X, gameSettings.SlideDisplayDuration, XSlideImage, ThreadSafeRandom.GetRandomNumber(gameSettings.BlankSlideDisplayTimes)));
            }
            // BY
            for (int i = 0; i < (int)gameSettings.SlideCount * 0.1; i++)
            {
                SlideCollection.Add(new Slide(SlideType.B, gameSettings.SlideDisplayDuration, "alex_the_alien.jpg", ThreadSafeRandom.GetRandomNumber(gameSettings.BlankSlideDisplayTimes)));
                SlideCollection.Add(new Slide(SlideType.Y, gameSettings.SlideDisplayDuration, YDistractorSlideImage, ThreadSafeRandom.GetRandomNumber(gameSettings.BlankSlideDisplayTimes)));
            }
            // AY
            for (int i = 0; i < (int)gameSettings.SlideCount * 0.1; i++)
            {
                SlideCollection.Add(new Slide(SlideType.A, gameSettings.SlideDisplayDuration, "alex_the_alien.jpg", ThreadSafeRandom.GetRandomNumber(gameSettings.BlankSlideDisplayTimes)));
                SlideCollection.Add(new Slide(SlideType.Y, gameSettings.SlideDisplayDuration, YDistractorSlideImage, ThreadSafeRandom.GetRandomNumber(gameSettings.BlankSlideDisplayTimes)));
            }
            // BX
            for (int i = 0; i < (int)gameSettings.SlideCount * 0.1; i++)
            {
                SlideCollection.Add(new Slide(SlideType.B, gameSettings.SlideDisplayDuration, "alex_the_alien.jpg", ThreadSafeRandom.GetRandomNumber(gameSettings.BlankSlideDisplayTimes)));
                SlideCollection.Add(new Slide(SlideType.X, gameSettings.SlideDisplayDuration, XSlideImage, ThreadSafeRandom.GetRandomNumber(gameSettings.BlankSlideDisplayTimes)));
            }

            // Generate random number generator
            Random random = new Random(seed.GetHashCode());
            // Shuffle
            return SlideCollection.OrderBy(i => random.Next()).ToList<Slide>();
        }

        private List<Slide> GenerateSeekXSlideSequence(UserGameSettings gameSettings, string seed)
        {
            // Create slide collection using the 50%X and 50%Distractor 
            List<Slide> SlideCollection = new List<Slide>();
            // X 
            int typeXSlideCount = (int)(gameSettings.SlideCount * 0.5);
            for (int i = 0; i < typeXSlideCount; i++)
            {
                SlideCollection.Add(new Slide(SlideType.X, gameSettings.SlideDisplayDuration,
                    GenerateSeekXImageSequence(gameSettings.SeekGridOptions, true),
                        ThreadSafeRandom.GetRandomNumber(gameSettings.BlankSlideDisplayTimes)));
            }
            // DistractorY
            for (int i = 0; i < gameSettings.SlideCount - typeXSlideCount; i++)
            {
                SlideCollection.Add(new Slide(SlideType.Y, gameSettings.SlideDisplayDuration,
                    GenerateSeekXImageSequence(gameSettings.SeekGridOptions, false),
                        ThreadSafeRandom.GetRandomNumber(gameSettings.BlankSlideDisplayTimes)));
            }
            // Generate random number generator
            Random random = new Random(seed.GetHashCode());
            // Shuffle
            return SlideCollection.OrderBy(i => random.Next()).ToList<Slide>();
        }

        private List<string> GenerateSeekXImageSequence(SeekGridOption seekGridOptions, bool target)
        {
            int imageCount = seekGridOptions.GridColumns * seekGridOptions.GridRows;
            List<string> Images = new List<string>();

            int distractorCount = imageCount - seekGridOptions.ContrastCount;

            // Target
            if (target) {
                Images.Add(XSlideImage);
                distractorCount -= 1;
            }
            // Imitation
            Images.AddRange(Enumerable.Repeat(YDistractorSlideImage, distractorCount));
            // Contrast
            Images.AddRange(Enumerable.Repeat(BContrastImage, seekGridOptions.ContrastCount));
            // Shuffle
            Images.Shuffle();
            return Images;
        }

        public Slide GetRandomRewardSlide(double displayDuration)
        {
            string slideImage = RewardSlideImages[ThreadSafeRandom.CurrentThreadRandom.Next(RewardSlideImages.Length - 1)];
            return new Slide(SlideType.Reward, displayDuration, slideImage, 0, RewardSound);
        }

        public ConfusionMatrix CalculateConfusionMatrix(List<Slide> slides)
        {
            return new ConfusionMatrix(slides);
        }

        public void EndGame(Game game, string sensorDataFile)
        {
            game.SessionData.EndDate = DateTime.Now;
            // Calculate game stats
            CalculateGameScoreAndStats(game);
            // Calculate engangement
            game.Outcome.ConfusionMatrix = CalculateConfusionMatrix(game.Slides);

            // Save to Json
            SaveGameToJson(game);

            // Merge sensor data
            if (sensorDataFile != "")
            {
                MergeDataFiles(Path.Combine(App.LocalStorageDirectory, game.GameDataFile)
                    , Path.Combine(App.LocalStorageDirectory, sensorDataFile));

                File.Delete(Path.Combine(App.LocalStorageDirectory, sensorDataFile));
            }
        }

        private string SaveGameToJson(Game game)
        {
            game.GameDataFile = $"{Guid.NewGuid().ToString()}.json";
            string filePathName = Path.Combine(App.LocalStorageDirectory, game.GameDataFile);
            using (var file = File.Open(filePathName, FileMode.Create, FileAccess.Write))
            {
                using (var sw = new StreamWriter(file, new UTF8Encoding(false)))
                {
                    // Ignore some properties
                    var settings = new JsonSerializerSettings
                    {
                        ContractResolver = ShouldSerializeContractResolver.Instance
                    };
                    sw.Write(JsonConvert.SerializeObject(game, Formatting.Indented, settings));
                }
            }
            return filePathName;
        }

        private void MergeDataFiles(string gameDataFile, string gameSensorFile)
        {
            const string FaceAnchorJson = "\"FaceAnchorData\": []";
            using (var file = File.Open(gameDataFile, FileMode.Open, FileAccess.ReadWrite))
            {
                long faceAnchorPosition = file.Seek(FaceAnchorJson);
              
                using (var sensorFile = File.Open(gameSensorFile, FileMode.Open, FileAccess.Read))
                {
                    // Make a copy of last JSON part
                    file.Seek(faceAnchorPosition + FaceAnchorJson.Length, SeekOrigin.Begin);
                    using (var sr = new StreamReader(file))
                    {
                        string text = sr.ReadToEnd();
                    
                        // Copy sensor data
                        file.Seek(faceAnchorPosition, SeekOrigin.Begin);
                        sensorFile.CopyTo(file);
                        // Copy remaining JSON file contents                       
                        byte[] textByteArray = Encoding.UTF8.GetBytes(text);
                        file.Write(textByteArray, 0, textByteArray.Length);
                        file.SetLength(file.Position);
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
            int? slideIndex = game.Slides.IndexOf(slide);

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
    }
}
