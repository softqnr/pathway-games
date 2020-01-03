﻿using PathwayGames.Infrastructure.Navigation;
using PathwayGames.Infrastructure.Sound;
using PathwayGames.Models;
using PathwayGames.Models.Enums;
using PathwayGames.Sensors;
using PathwayGames.Services.Sensors;
using PathwayGames.Services.Slides;
using PathwayGames.Services.User;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PathwayGames.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        // Services
        private readonly ISlidesService _slidesService;
        private readonly IUserService _userService;
        private readonly ISensorLogWriterService _sensorLowWriterService;
        private readonly ISoundService _soundService;

        private Game _game;
        private bool _paused;
        private int? _slideIndex;
        private int? _slideCount;
        private int _imageGridColumns = 1;
        private IList<string> _slideImages;
        private string _seed;
        private string _userName;
        private CancellationTokenSource _cts;

        private Slide CurrentSlide { get; set; }
        public SlideStateMachine StateMachine { get; private set; }

        // Bindable properties
        public int? SlideIndex
        {
            get => _slideIndex;
            set => SetProperty(ref _slideIndex, value);
        }

        public int? SlideCount
        {
            get => _slideCount;
            set => SetProperty(ref _slideCount, value);
        }

        public IList<string> SlideImages
        {
            get => _slideImages;
            set => SetProperty(ref _slideImages, value);
        }

        public bool Paused
        {
            get => _paused;
            set => SetProperty(ref _paused, value);
        }

        public int ImageGridColumns
        {
            get => _imageGridColumns;
            set => SetProperty(ref _imageGridColumns, value);
        }

        public Game Game
        {
            get => _game;
            private set => SetProperty(ref _game, value);
        }

        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }

        public string Seed
        {
            get => _seed;
            set => SetProperty(ref _seed, value);
        }

        // Commands
        public ICommand ButtonTappedCommand
        {
            get
            {
                return new Command<Point>(async (p) =>
                {
                    await OnButtonTapped(p);
                });
            }
        }

        public ICommand EyeGazeChangedCommand
        {
            get
            {
                return new Command<FaceAnchorChangedEventArgs>((e) =>
                {
                    _sensorLowWriterService.WriteToLog(e.Reading.ToString());
                });
            }
        }

        //CTor
        public GameViewModel(ISlidesService slidesService,
            IUserService userService,
            ISoundService soundService,
            ISensorLogWriterService sensorLowWriterService)
        {
            _slidesService = slidesService;
            _userService = userService;
            _sensorLowWriterService = sensorLowWriterService;
            _soundService = soundService;

            _userName = App.SelectedUser.UserName;

            // Create StateMachine
            StateMachine = new SlideStateMachine
            (
                startGameAction: async () => await StartGame(),
                nextSlideAction: async () => await GotoNextSlide(),
                evaluateSlideResponseAction: async () => await EvaluateResponse(),
                blankSlideAction: async (d) => await ShowBlankSlide(d),
                blankSlideCancelableAction: async (d) => await ShowBlankSlideCancelable(d),
                rewardSlideAction: async () => await ShowRewardSlide(),
                endAction: async () => await EndGame()
            );
        }

        public async Task OnButtonTapped(Point p)
        {
            System.Diagnostics.Debug.WriteLine("({0}/{1}) - {2:HH:mm:ss.fff} - OnButtonTapped()", SlideIndex, SlideCount, DateTime.Now);
            // Save response
            SaveResponse(p);
            // Ommit response for reward slide
            if (CurrentSlide.SlideType == SlideType.Reward)
                return;
            // Handle response
            var response = _slidesService.EvaluateSlideResponse(_game, CurrentSlide);
            if (response == ResponseOutcome.CorrectCommission)
            {
                await StateMachine.FireAsync(Triggers.CorrectCommision); // Cancel blank immediately
                // Play ding sound
                await _soundService.PlaySoundAsync("ding.mp3");
            } else if(response ==  ResponseOutcome.WrongCommission) { 
                // Play mistake sound
                await _soundService.PlaySoundAsync("mistake.mp3");
            }
        }

        public async Task GotoNextSlide()
        {
            System.Diagnostics.Debug.WriteLine("({0}/{1}) - {2:HH:mm:ss.fff} - ShowNextSlide()", SlideIndex, SlideCount, DateTime.Now);
            if (SlideIndex < SlideCount)
            {
                // Forward to next slide
                SlideIndex++;
                CurrentSlide = _game.Slides[_slideIndex.Value - 1];
                // Render slide
                await RenderSlide(CurrentSlide);
                await StateMachine.FireAsync(Triggers.SlideFinished);
            }
            else
            {
                await StateMachine.FireAsync(Triggers.NoSlides);
            }
        }

        public async Task<ResponseOutcome> EvaluateResponse()
        {
            // Evaluate response
            ResponseOutcome outcome = _slidesService.EvaluateSlideResponse(_game, CurrentSlide);

            // Fire trigger based on outcome
            switch (outcome)
            {
                case ResponseOutcome.CorrectCommission:
                    // Within normal slide duration
                    System.Diagnostics.Debug.WriteLine("({0}/{1}) - {2:HH:mm:ss.fff} - Correct Commission", SlideIndex, SlideCount, DateTime.Now);
                    // 
                    await StateMachine.FireAsync(Triggers.CorrectCommision);
                    break;
                case ResponseOutcome.WrongCommission:
                    System.Diagnostics.Debug.WriteLine("({0}/{1}) - {2:HH:mm:ss.fff} - Wrong Commission", SlideIndex, SlideCount, DateTime.Now);
                    //
                    //await StateMachine.FireAsync(Triggers.WrongCommision);
                    await StateMachine.ChangeStateToShowBlankSlide(CurrentSlide.BlankDuration);
                    break;
                case ResponseOutcome.WrongOmission:
                case ResponseOutcome.CorrectOmission:
                    await StateMachine.ChangeStateToShowBlankCancelableSlide(CurrentSlide.BlankDuration);
                    break;
            }
            return outcome;
        }

        public async Task ShowRewardSlide()
        {
            System.Diagnostics.Debug.WriteLine("({0}/{1}) - {2:HH:mm:ss.fff} - ShowRewardSlide()", SlideIndex, SlideCount, DateTime.Now);
            TimeSpan blankSlideTime = TimeSpan.FromSeconds(CurrentSlide.BlankDuration);
            // Cancel blank display if token exists
            if (_cts != null)
            {
                blankSlideTime = _slidesService.CalculateBlankSlideTimeLeft(CurrentSlide);
                _cts.Cancel();
            }
            CurrentSlide = _slidesService.GetRandomRewardSlide(_game.GameSettings.RewardDisplayDuration);
            // Set span to 1 to display reward slide
            int GameImageGridColumns = ImageGridColumns;
            SlideImages = null;
            ImageGridColumns = 1;
            // Render slide
            await RenderSlide(CurrentSlide);
            // Restore ImageGridColumns
            ImageGridColumns = GameImageGridColumns;
            System.Diagnostics.Debug.WriteLine("({0}/{1}) - {2:HH:mm:ss.fff} - ShowRewardSlide() Finished {3} Blank delay left", SlideIndex, SlideCount, DateTime.Now, blankSlideTime.TotalSeconds);
            //
            await StateMachine.ChangeStateToShowBlankSlide(blankSlideTime.TotalSeconds);
        }

        public async Task ShowBlankSlideCancelable(double duration)
        {
            System.Diagnostics.Debug.WriteLine("({0}/{1}) - {2:HH:mm:ss.fff} - ShowBlankSlideCancelable({3})", SlideIndex, SlideCount, DateTime.Now,
                duration);
            // Display blank slide / cancelable
            SlideImages = null;

            try
            {
                // Reset Cancellation token
                _cts = new CancellationTokenSource();
                await Task.Delay(TimeSpan.FromSeconds(duration), _cts.Token);
                //
                await StateMachine.FireAsync(Triggers.NextSlide);
            }
            catch (TaskCanceledException)
            {
                // Blank display canceled because award slide has to be displayed within
                _cts.Dispose();
                _cts = null;
            }
        }

        public async Task ShowBlankSlide(double duration)
        {
            System.Diagnostics.Debug.WriteLine("({0}/{1}) - {2:HH:mm:ss.fff} - ShowBlankSlide({3})", SlideIndex, SlideCount, DateTime.Now,
               duration);
            // Display blank slide
            SlideImages = null;
            
            await Task.Delay(TimeSpan.FromSeconds(duration));
            //
            await StateMachine.FireAsync(Triggers.SlideFinished);
        }

        private async Task RenderSlide(Slide slide)
        {
            // Display slide image
            SlideImages = slide.Images;
            // Set slide displayed time
            CurrentSlide.SlideDisplayed = DateTime.Now;
            // Play slide sound
            if (!String.IsNullOrEmpty(slide.Sound))
            {
                await _soundService.PlaySoundAsync(slide.Sound);
            }
            System.Diagnostics.Debug.WriteLine("({0}/{1} - {2}) - {3:HH:mm:ss.fff} - RenderSlide()", SlideIndex, SlideCount, CurrentSlide.SlideType.ToString(), CurrentSlide.SlideDisplayed);
            // Wait for the slide duration
            await Task.Delay(TimeSpan.FromSeconds(CurrentSlide.DisplayDuration));
            // Set slide hidden time
            CurrentSlide.SlideHidden = DateTime.Now;
        }

        public async Task CreateGameAndStart(GameType gameType, UserGameSettings userGameSettings)
        {
            // Create game
            Game = _slidesService.Generate(gameType, userGameSettings, App.SelectedUser.Id, App.SelectedUser.UserName, _seed);
            SlideIndex = 0;
            SlideCount = _game.Slides.Count;
            Seed = _game.SessionData.Seed;

            // Triger start game
            await StateMachine.FireAsync(Triggers.Start);
        }

        public async Task StartGame()
        {
            _game.SessionData.StartDate = DateTime.Now;
            // Start sensor recording
            StartSensorRecording();
            // Start game
            await StateMachine.FireAsync(Triggers.NextSlide);
        }

        public void PauseGame()
        {

        }

        public void ResumeGame()
        {

        }

        public async Task EndGame()
        {
            System.Diagnostics.Debug.WriteLine("({0}) - {1:HH:mm:ss.fff}", "EndGame()", DateTime.Now);
            // Set sensor data filename
            _sensorLowWriterService.Stop();
            string sensorDataFile = _sensorLowWriterService.LogFile;
            // End game session
            _slidesService.EndGame(_game, sensorDataFile);
            // Save game session to db
            UserGameSession gameSession = await _userService.SaveGameSessionData(_game);
            // Go to results view
            NavigateToResultsView(gameSession);
        }

        private void StartSensorRecording()
        {
            _sensorLowWriterService.LogPrefix = "\"FaceAnchorData\": [";
            _sensorLowWriterService.LogSuffix = "] ";

            _sensorLowWriterService.Start($"sensor_{Guid.NewGuid().ToString()}.json", ",");
        }

        private void SaveResponse(Point p)
        {

            Int32? slideIndex = (CurrentSlide.SlideType == SlideType.Reward) ? (Int32?)null : _game.Slides.IndexOf(CurrentSlide);
            _game.RecordButtonPress(slideIndex, p);
        }

        public override async Task InitializeAsync(object navigationData)
        {
            if (navigationData != null &&
                Enum.TryParse<GameType>(navigationData.ToString(), out var gameType))
            {
                Title = $"{gameType.ToString()} Game";

                // Read User Game Settings
                UserGameSettings userGameSettings = await _userService.GetUserSettings(App.SelectedUser.Id);

                // Set grid columns for images
                if (gameType == GameType.SeekX)
                {
                    ImageGridColumns = userGameSettings.SeekGridOptions.GridColumns;
                }

                await CreateGameAndStart(gameType, userGameSettings);
            }
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            if (StateMachine.State != States.End)
            {
                StateMachine.Fire(Triggers.Exit);
                _sensorLowWriterService.Cancel();
            }
        }

        private void NavigateToResultsView(UserGameSession gameSession)
        {
            // If called from statemachine has to be run on main thread
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                NavigationParameters p = new NavigationParameters();
                p.Add("game", _game);
                p.Add("game_session", gameSession);
                // Navigate to result view
                await NavigationService.NavigateToAsync<GameResultsViewModel>(p);
                await NavigationService.RemoveLastFromBackStackAsync();
            });
        }
    }
}
