using PathwayGames.Models;
using PathwayGames.Models.Enums;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace PathwayGames.Services.Slides
{
    public interface ISlidesService
    {
        Game Generate(GameType gameType, UserGameSettings gameSettings, long userId, string userName, string seed);

        Slide GetRandomRewardSlide(double displayDuration);

        void CalculateGameScoreAndStats(Game game);

        ResponseOutcome EvaluateSlideResponse(Game game, Slide slide);

        TimeSpan CalculateBlankSlideTimeLeft(Slide slide);

        void EndGame(Game game, string sensorDataFile);
    }
}
