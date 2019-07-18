using FFImageLoading;
using PathwayGames.Models;
using PathwayGames.Models.Enums;
using PathwayGames.Services.Engangement;
using PathwayGames.Services.Sensors;
using PathwayGames.Services.Slides;
using PathwayGames.Infrastructure.Sound;
using Stateless;
using Stateless.Graph;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PathwayGames.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        private GameType _gameType;
        private Game _game;
        private GameSettings _gameSettings;

        private ISlidesService _slidesService;
        private ISensorsService _sensorsService;
        private ISoundService _soundService;
        private IEngangementService _engangementService;

        private string _title;
        private bool _paused;
        private int? _slideIndex;
        private int? _slideCount;
        private ImageSource _slideImageSource;
        private ImageSource _recordingImageSource;
        private string _seed;
        private string _userName;
        private CancellationTokenSource _cts;

        private Slide CurrentSlide { get; set; }
        public StateMachine StateMachine { get; private set; }

        // Bindable properties
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

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

        public ImageSource SlideImageSource
        {
            get => _slideImageSource;
            set => SetProperty(ref _slideImageSource, value);
        }

        public bool Paused
        {
            get => _paused;
            set => SetProperty(ref _paused, value);
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

        public ImageSource RecordingImageSource
        {
            get => _recordingImageSource;
            set => SetProperty(ref _recordingImageSource, value);
        }

        // Commands
        public Command<Point> ButtonTappedCommand
        {
            get
            {
                return new Command<Point>(async (p) =>
                {
                    await OnButtonTapped(p);
                });
            }
        }

        //public ICommand SlideAppearedCommand
        //{
        //    get
        //    {
        //        return new Command(async () =>
        //        {
        //            await OnSlideAppeared();
        //        });
        //    }
        //}

        //CTor
        public GameViewModel(ISlidesService slidesService,
            ISoundService soundService,
            ISensorsService sensorsService,
            IEngangementService engangementService)
        {
            _slidesService = slidesService;
            _sensorsService = sensorsService;
            _soundService = soundService;
            _engangementService = engangementService;

            // Create StateMachine
            StateMachine = new StateMachine
            (
                createGameAction: async () => await CreateGame(),
                startGameAction: async () => await StartGame(),
                nextSlideAction: async () => await GotoNextSlide(),
                evaluateSlideResponseAction: async () => await EvaluateResponse(),
                blankSlideAction: async (d) => await ShowBlankSlide(d),
                blankSlideCancelableAction: async (d) => await ShowBlankSlideCancelable(d),
                rewardSlideAction: async () => await ShowRewardSlide(),
                endAction: async () => await EndGame()
            );

            _userName = App.UserName;
            // TODO: This should come from parameters
            _seed = "XYZ";
        }

        public async Task OnButtonTapped(Point p)
        {
            System.Diagnostics.Debug.WriteLine("({0}/{1}) - {2:HH:mm:ss.fff} - OnButtonTapped()", SlideIndex, SlideCount, DateTime.Now);
            // Save response
            SaveResponse(p);
            // Handle response
            var response = _slidesService.EvaluateSlideResponse(_game, CurrentSlide);
            switch (response)
            {
                case ResponseOutcome.CorrectCommission:
                    // Play ding sound
                    await _soundService.PlaySoundAsync("ding.mp3");
                    await StateMachine.FireAsync(Triggers.CorrectCommision); // Cancel blank immediately
                    break;
                case ResponseOutcome.WrongCommission:
                    // Play mistake sound
                    await _soundService.PlaySoundAsync("mistake.mp3");
                    break;
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
            TimeSpan blankSlideTime = new TimeSpan();
            blankSlideTime = TimeSpan.FromSeconds(CurrentSlide.BlankDuration);
            // Cancel blank display if token exists
            if (_cts != null)
            {
                blankSlideTime = _slidesService.CalculateBlankSlideTimeLeft(CurrentSlide);
                _cts.Cancel();
            }
            CurrentSlide = _slidesService.GetRandomRewardSlide(_gameSettings.RewardDisplayDuration);

           
            await RenderSlide(CurrentSlide);
            System.Diagnostics.Debug.WriteLine("({0}/{1}) - {2:HH:mm:ss.fff} - ShowRewardSlide() Finished {3} Blank delay left", SlideIndex, SlideCount, DateTime.Now, blankSlideTime.TotalSeconds);
            //
            await StateMachine.ChangeStateToShowBlankSlide(blankSlideTime.TotalSeconds);
        }

        public async Task ShowBlankSlideCancelable(double duration)
        {
            System.Diagnostics.Debug.WriteLine("({0}/{1}) - {2:HH:mm:ss.fff} - ShowBlankSlideCancelable({3})", SlideIndex, SlideCount, DateTime.Now,
                duration);
            // Display blank slide / cancelable
            SlideImageSource = null;

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
            SlideImageSource = null;

            await Task.Delay(TimeSpan.FromSeconds(duration));
            //
            await StateMachine.FireAsync(Triggers.SlideFinished);
        }

        private async Task RenderSlide(Slide slide)
        {
            var now = DateTime.Now;
            // Display slide image
            SlideImageSource = ImageSource.FromFile(slide.Image);
            // Play slide sound
            if (!String.IsNullOrEmpty(slide.Sound))
            {
                await _soundService.PlaySoundAsync(slide.Sound);
            }
            System.Diagnostics.Debug.WriteLine("({0}/{1} - {2}) - {3:HH:mm:ss.fff} - RenderSlide()", SlideIndex, SlideCount, CurrentSlide.SlideType.ToString(), now);
            // Set slide displayed time
            CurrentSlide.SlideDisplayed = now;
            // Wait for the slide duration
            await Task.Delay(TimeSpan.FromSeconds(CurrentSlide.DisplayDuration));
            // Set slide hidden time
            CurrentSlide.SlideHidden = DateTime.Now;
        }

        public async Task CreateGame()
        {
            // TODO: Read Settings
            _gameSettings = new GameSettings(); // Use default
            // Create game
            _game = _slidesService.Generate(_gameType, _gameSettings, App.UserName, _seed);
            SlideIndex = 0;
            SlideCount = _game.Slides.Count;

            await Task.FromResult(true);
        }

        public async Task StartGame()
        {
            _game.StartDate = DateTime.Now;
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
            // Game finished
            StopSensorRecording();
            _game.EndDate = DateTime.Now;
            // Calculate engangement
            CalculateEngangement();
            // Calculate game stats
            _slidesService.CalculateGameScoreAndStats(_game);
            // _slidesService.Save(_game);
            await Task.FromResult(true);
        }

        private void StartSensorRecording()
        {
            _sensorsService.StartRecording();
            RecordingImageSource = ImageSource.FromFile("rec.png");
        }

        private void StopSensorRecording()
        {
            _sensorsService.StopRecording();
            RecordingImageSource = ImageSource.FromFile("rec_off.png");
        }

        private void SaveResponse(Point p)
        {

            Int32? slideIndex = (CurrentSlide.SlideType == SlideType.Reward) ? (Int32?)null : _game.Slides.IndexOf(CurrentSlide);
            _game.RecordButtonPress(slideIndex, p);
        }

        private void CalculateEngangement()
        {
            _game.ConfusionMatrix = _engangementService.CalculateConfusionMatrix(_game.Slides);
        }

        private async Task PreloadImages()
        {
            //await ImageService.Instance.LoadFileFromApplicationBundle("").PreloadAsync();
        }

        public override async Task InitializeAsync(object navigationData)
        {
            if (navigationData != null &&
                Enum.TryParse<GameType>(navigationData.ToString(), out var gameType))
            {
                _gameType = gameType;
                Title = _gameType.ToString() + " Game";

                await CreateGame();
                await StateMachine.FireAsync(Triggers.Start);
                // NavigateToAsync called from Stateless throws null reference exception
                await WaitEndState();
            }
        }

        private async Task WaitEndState()
        {
            while (StateMachine.State != States.End)
            {
                await Task.Delay(100);
            }
            // Navigate to result view
            await NavigationService.NavigateToAsync<GameResultsViewModel>(_game);
            await NavigationService.RemoveLastFromBackStackAsync();
        }

    }
}
