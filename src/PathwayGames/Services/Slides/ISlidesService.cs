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
        Game Load(string fileName);
        ConfusionMatrix CalculateConfusionMatrix(List<Slide> slides);
        void FinalizeGame(Game game);
        string Save(Game game);
    }
}
