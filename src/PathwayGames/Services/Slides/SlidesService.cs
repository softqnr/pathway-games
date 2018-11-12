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
                    SlideCollection.Add(new Slide(SlideType.X) { Name = "Mickey Mouse " + i, DisplayDuration = 1.2, Image = "Mickey_Mouse.png" });
                }
                //Distractor
                for (int i = 0; i < 30; i++)
                {
                    SlideCollection.Add(new Slide(SlideType.DistractorY) { Name = "Red car " + i, DisplayDuration = 1.2, Image = "f1.jpg" });
                }
                return SlideCollection;
            }
        }

        public IList<Slide> Generate(GameType gameType, string randomSeed)
        {
            // Generate random number generator
            Random random = randomSeed != "" ? new Random(randomSeed.GetHashCode()) : new Random();
            // TODO: Calculate slide distribution
            int xCount = 15;
            int distractorCount = 5;
            // Pick slides
            var slides = Slides.Where(x => x.SlideType == SlideType.X)
                    .OrderBy(i => random.Next())
                    .Take(xCount)
                .Concat<Slide>(
                    Slides.Where(x => x.SlideType == SlideType.DistractorY)
                    .OrderBy(i => random.Next())
                    .Take(distractorCount)
                ).OrderBy(i => random.Next())
                .ToList<Slide>();

            return slides;
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
    }
}
