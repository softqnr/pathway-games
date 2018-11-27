using PathwayGames.Models;
using PathwayGames.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathwayGames.Services.Slides
{
    public interface ISlidesService
    {
        IList<Slide> Generate(GameType gameType, GameSettings gameSettings, string randomSeed);
        Slide GetRandomRewardSlide();
        void CalculateGameScoreAndStats(Game game);
        ResponseOutcome EvaluateSlideResponse(Game game, Slide slide);
        Game Load(string filePath);
        void Save(Game game);
    }
}
