using PathwayGames.Models;
using PathwayGames.Models.Enums;
using System;

namespace PathwayGames.Services.Slides
{
    public interface ISlidesService
    {
        Game Generate(GameType gameType, GameSettings gameSettings, string userName, string seed);
        Slide GetRandomRewardSlide(double displayDuration);
        void CalculateGameScoreAndStats(Game game);
        ResponseOutcome EvaluateSlideResponse(Game game, Slide slide);
        TimeSpan CalculateBlankSlideTimeLeft(Slide slide);
        Game Load(string filePath);
        void Save(Game game);
    }
}
