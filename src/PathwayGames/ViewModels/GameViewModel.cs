using PathwayGames.Models;
using PathwayGames.Models.Enums;
using PathwayGames.Services.Engangement;
using PathwayGames.Services.Sensors;
using PathwayGames.Services.Slides;
using PathwayGames.Services.Sound;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PathwayGames.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        private Game _game;
        private ISlidesService _slidesService;
        private ISensorsService _sensorsService;
        private ISoundService _soundService;
        private IEngangementService _engangementService;

        private bool _showReward;
        private int? _slideIndex;
        private int? _slideCount;
        private ImageSource _slideImageSource;
        private ImageSource _engangementImageSource;
        private ImageSource _buttonImageSource;
        private ImageSource _recordingImageSource;
        private string _seed;
        private string _userName;
        private CancellationTokenSource _cts = new CancellationTokenSource();

        private Slide CurrentSlide { get; set; }

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
        public ImageSource SlideImageSource
        {
            get => _slideImageSource;
            set => SetProperty(ref _slideImageSource, value);
        }
        public ImageSource EngangementImageSource
        {
            get => _engangementImageSource;
            set => SetProperty(ref _engangementImageSource, value);
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
        public ImageSource ButtonImageSource
        {
            get => _buttonImageSource;
            set => SetProperty(ref _buttonImageSource, value);
        }
        public ImageSource RecordingImageSource
        {
            get => _recordingImageSource;
            set => SetProperty(ref _recordingImageSource, value);
        }

        // Commands
        public Command SlideDisplayedCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await OnSlideDisplayedCommand();
                });
            }
        }
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

        //CTor
        public GameViewModel(ISlidesService slidesService, 
            ISoundService soundService, 
            ISensorsService sensorsService, 
            IEngangementService engangementService )
        {
            _slidesService = slidesService;
            _sensorsService = sensorsService;
            _soundService = soundService;
            _engangementService = engangementService;

            EngangementImageSource = ImageSource.FromFile("icon_engangement_absent.png");
            ButtonImageSource = ImageSource.FromFile("button.jpg");
            // TODO: This should come from parameters
            _userName = "Quest";
            _seed = RandomString(6);
        }
        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public async Task OnButtonTapped(Point p)
        {    
            Int32? slideIndex = (CurrentSlide.SlideType == SlideType.Reward) ? (Int32?)null : _game.Slides.IndexOf(CurrentSlide);
            _game.ButtonPresses.Add(new ButtonPress() { Coordinates = p, Time = DateTime.Now, SlideIndex = slideIndex });
            // Evaluate user response
            ResponseOutcome outcome = _slidesService.EvaluateSlideResponse(_game, CurrentSlide);
            // On blank and on correct commision response
            if (outcome == ResponseOutcome.CorrectCommission)
            {
                // if within normal slide duration
                if (CurrentSlide.SlideHidden.HasValue)
                {
                    Console.WriteLine("Correct Commission in blank");
                    // Cancel blank slide delay
                    _cts.Cancel();
                }
                else
                {
                    _showReward = true;
                }
            }
            // TODO: Calculate engangement

            await ShowButtonPressEffect();
        }

        private async Task ShowButtonPressEffect()
        {
            // Button press effect
            ButtonImageSource = ImageSource.FromFile("button_pressed.jpg");
            await Task.Delay(100);
            ButtonImageSource = ImageSource.FromFile("button.jpg");
        }

        private void CreateCPT(GameType gameType)
        {
            _game = _slidesService.Generate(gameType, new GameSettings() {
                SlideCount = 10,
                BlankSlideDisplayTimes = new []{ 1, 1.2} }, _userName, _seed);
            _slideIndex = 0;
            SlideCount = _game.Slides.Count;
        }

        public async Task ShowNextSlide()
        {
            if (SlideIndex < SlideCount)
            {
                // Evaluate current slide
                if (SlideIndex > 0) { 
                    _slidesService.EvaluateSlideResponse(_game, _game.Slides[_slideIndex.Value - 1]);
                }
                // Render next slide
                SlideIndex++;
                CurrentSlide = _game.Slides[_slideIndex.Value - 1];
                await RenderSlide(CurrentSlide);
            }else{
                // Game finished
                _sensorsService.StopRecording();
                RecordingImageSource = ImageSource.FromFile("rec_off.png");
                // Calculate game stats
                _slidesService.CalculateGameScoreAndStats(_game);
                // _slidesService.Save(_game);
                // Navigate to thank you view
                await NavigationService.NavigateToAsync<ThankYouViewModel>(_game);
                await NavigationService.RemoveLastFromBackStackAsync();
            }
        }

        public async Task ShowRewardSlide()
        {
            _showReward = false;
            CurrentSlide = _slidesService.GetRandomRewardSlide();
            await RenderSlide(CurrentSlide);
        }

        public async Task ShowBlankSlide()
        {
            // Display blank slide / cancelable
            SlideImageSource = null;
            // Reset Cancellation token
            _cts.Dispose();
            _cts = new CancellationTokenSource();
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(CurrentSlide.BlankDuration), _cts.Token);
                await ShowNextSlide();
            }
            catch (TaskCanceledException)
            {
                // if delay is cancelled then show reward
                await ShowRewardSlide();
            }
        }

        private async Task RenderSlide(Slide slide)
        {
            Console.WriteLine(slide.Image);
            // Display elide image
            SlideImageSource = ImageSource.FromFile($"slide_{slide.Image}");
            
            // Play slide sound
            if (!String.IsNullOrEmpty(slide.Sound))
            {
                await _soundService.PlaySoundAsync(slide.Sound);
            }
        }

        private async Task OnSlideDisplayedCommand()
        {
            // Do to record response for reward slides
            if (CurrentSlide.SlideType != SlideType.Reward)
            {
                // Set slide displayed time
                CurrentSlide.SlideDisplayed = DateTime.Now;
                // Wait slide duration
                await Task.Delay(TimeSpan.FromSeconds(CurrentSlide.DisplayDuration));
                // Set slide hidden time
                CurrentSlide.SlideHidden = DateTime.Now;
                // Check if user gave correct answer within normal slide time
                // in that case show reward slide without showing blank first
                if (_showReward)
                {
                    await ShowRewardSlide();
                }
                else
                {
                    await ShowBlankSlide();
                }
            }
            else
            {
                Console.WriteLine("Delay reward");
                await Task.Delay(TimeSpan.FromSeconds(CurrentSlide.DisplayDuration));
                // Proceed to next slide
                await ShowNextSlide();
            }
        }

        public async Task StartCPT(GameType gameType)
        {
            // Create game
            CreateCPT(gameType);
            // Start sensor recording
            _sensorsService.StartRecording();
            RecordingImageSource = ImageSource.FromFile("rec.png");
            // Start slide show
            await ShowNextSlide();
        }

        public override async Task InitializeAsync(object navigationData)
        {
            if (navigationData != null &&
                Enum.TryParse<GameType>(navigationData.ToString(), out var gameType))
            {
                await StartCPT(gameType);
            }
        }
    }
}
