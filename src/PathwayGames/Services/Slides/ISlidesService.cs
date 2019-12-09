using PathwayGames.Models;
using PathwayGames.Models.Enums;
using System;
using System.Collections.Generic;

namespace PathwayGames.Services.Slides
{
    public interface ISlidesService
    {
        Game Generate(GameType gameType, UserGameSettings gameSettings, long userId, string userName, string seed);
        Slide GetRandomRewardSlide(double displayDuration);
        void CalculateGameScoreAndStats(Game game);
        ResponseOutcome EvaluateSlideResponse(Game game, Slide slide);
        TimeSpan CalculateBlankSlideTimeLeft(Slide slide);
        ConfusionMatrix CalculateConfusionMatrix(List<Slide> slides);
        void EndGame(Game game, string sensorDataFile);
    }
}
