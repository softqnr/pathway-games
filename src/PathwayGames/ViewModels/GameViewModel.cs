using PathwayGames.Models;
using PathwayGames.Models.Enums;
using PathwayGames.Services.Sensors;
using PathwayGames.Services.Slides;
using PathwayGames.Services.Sound;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace PathwayGames.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        private Game _game;
        private ISlidesService _slidesService;
        private ISensorsService _sensorsService;
        private ISoundService _soundService;

        private int? _slideIndex;
        private int? _slideCount;
        private bool _isVisible = true;
        private string _image;
        private ImageSource _imageSource;
        private ImageSource _buttonImageSource;
        private ImageSource _recordingImageSource;
        private string _seed;
        private string _userName;

        private Slide CurrentSlide { get => _game.Slides[_slideIndex.Value - 1]; }
        //
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
        public string Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }
        public ImageSource ImageSource
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
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
        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
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
        public Command SlideTimeoutCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await OnSlideTimeoutCommand();
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
        public GameViewModel(ISlidesService slidesService, ISoundService soundService, ISensorsService sensorsService)
        {
            _slidesService = slidesService;
            _sensorsService = sensorsService;
            _soundService = soundService;

            _userName = "Jack";
            _seed = "HJSND";
        }
        
        public async Task OnButtonTapped(Point p)
        {
            CurrentSlide.ButtonPresses.Add(new ButtonPress() { Coordinates = p, Time = DateTime.Now });
            // Button press effect
            ButtonImageSource = ImageSource.FromFile("button_pressed.jpg");
            await Task.Delay(100);
            ButtonImageSource = ImageSource.FromFile("button.jpg");
        }

        private void CreateGame(GameType gameType)
        {
            _game = new Game();
            _game.Slides = _slidesService.Generate(gameType, new GameSettings() { SlideCount = 10 },"");
            _slideIndex = 0;
            SlideCount = _game.Slides.Count;

            ButtonImageSource = ImageSource.FromFile("button.jpg");
        }

        public async Task ShowNextSlide()
        {
            //if (SlideIndex != 0)
            //{
            //    await ShowBlankSlide();
            //}
            if (SlideIndex < SlideCount)
            {
                SlideIndex++;
                await ShowSlide(CurrentSlide);
            }else{
                // Game finished
                _sensorsService.StopRecording();
                RecordingImageSource = ImageSource.FromFile("rec_off.png");
                // Calculate game stats
                // _slidesService.Save(_game);
                // Navigate to thank you view
                await NavigationService.NavigateToAsync<ThankYouViewModel>();
                await NavigationService.RemoveLastFromBackStackAsync();
            }
        }

        public async Task ShowBlankSlide()
        {
            IsVisible = false;
            var slide = _slidesService.GetBlankSlide();
            await Task.Delay(TimeSpan.FromSeconds(slide.DisplayDuration));
            IsVisible = true;
        }

        public async Task ShowRewardSlide()
        {
            Slide rewardSlide = _slidesService.GetRandomRewardSlide();
            await ShowSlide(rewardSlide);
            await Task.Delay(TimeSpan.FromSeconds(rewardSlide.DisplayDuration));
        }

        private async Task ShowSlide(Slide slide)
        {
            // Slide image
            switch (slide.SlideType)
            {
                case SlideType.X:
                case SlideType.Y:
                    //Image = $"resource://PathwayGames.Assets.Images.{_game.Slides[_slideIndex.Value - 1].Image}";
                    ImageSource = ImageSource.FromResource($"PathwayGames.Assets.Images.{slide.Image}");
                    break;
                case SlideType.Reward:
                    ImageSource = ImageSource.FromResource($"PathwayGames.Assets.Images.{slide.Image}");
                    break;
                case SlideType.Blank:
                    IsVisible = false;
                    //await Task.Delay(TimeSpan.FromSeconds(slide.DisplayDuration));
                    await OnSlideTimeoutCommand();
                    IsVisible = true;
                    break;
            }
            // Slide sound
            if (!String.IsNullOrEmpty(slide.Sound))
            {
                await _soundService.PlaySoundAsync(slide.Sound);
            }
        }

        private async Task OnSlideTimeoutCommand()
        {
            // Do to record response for reward slides
            if (CurrentSlide.SlideType != SlideType.Reward)
            {
                // Set slide displayed time
                CurrentSlide.SlideDisplayed = DateTime.Now;
                await Task.Delay(TimeSpan.FromSeconds(CurrentSlide.DisplayDuration));
                // Set slide hidden time
                CurrentSlide.SlideHidden = DateTime.Now;
                // Evaluete response
                // ShowRewardSlide();
            }
            // Proceed to next slide
            await ShowNextSlide();
        }

        public async Task StartGame(GameType gameType)
        {
            // Create game
            CreateGame(gameType);
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
                await StartGame(gameType);
            }
        }
    }
}
