﻿using FFImageLoading;
using FFImageLoading.Forms;
using PathwayGames.Models;
using PathwayGames.Models.Enums;
using PathwayGames.Services.Sensors;
using PathwayGames.Services.Slides;
using PathwayGames.Services.Sound;
using System;
using System.Collections.Generic;
using System.Threading;
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

        private bool _showReward;
        private int? _slideIndex;
        private int? _slideCount;
        private string _slideImage;
        private CachedImage _slideCachedImage;
        private ImageSource _slideImageSource;
        private ImageSource _engangementImageSource;
        private ImageSource _buttonImageSource;
        private ImageSource _recordingImageSource;
        private string _seed;
        private string _userName;
        private CancellationTokenSource _cts = new System.Threading.CancellationTokenSource();

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
        public string SlideImage
        {
            get => _slideImage;
            set => SetProperty(ref _slideImage, value);
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
        public CachedImage SlideCachedImage
        {
            get => _slideCachedImage;
            set => SetProperty(ref _slideCachedImage, value);
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
        public GameViewModel(ISlidesService slidesService, ISoundService soundService, ISensorsService sensorsService)
        {
            _slidesService = slidesService;
            _sensorsService = sensorsService;
            _soundService = soundService;

            EngangementImageSource = ImageSource.FromFile("icon_engangement_absent.png");
            ButtonImageSource = ImageSource.FromFile("button.jpg");
            // TODO: This should come from parameters
            _userName = "Quest";
            _seed = "HJSND";
        }
        
        public async Task OnButtonTapped(Point p)
        {
            CurrentSlide.ButtonPresses.Add(new ButtonPress() { Coordinates = p, Time = DateTime.Now });
            // Evaluate response
            ResponseOutcome outcome = _slidesService.EvaluateSlideResponse(CurrentSlide);
            // On blank and on correct commision response
            if (outcome == ResponseOutcome.CorrectCommission)
            {
                if (CurrentSlide.SlideHidden.HasValue)
                {
                    Console.WriteLine("Correct Commission in blank");
                    _cts.Cancel();
                }
                else
                {
                    _showReward = true;
                }
            }
            // Button press effect
            ButtonImageSource = ImageSource.FromFile("button_pressed.jpg");
            await Task.Delay(100);
            ButtonImageSource = ImageSource.FromFile("button.jpg");
        }

        private void CreateGame(GameType gameType)
        {
            _game = new Game();
            _game.Slides = _slidesService.Generate(gameType, new GameSettings() {
                SlideCount = 10,
                BlankSlideDisplayTimes = new []{ 1, 1.2} }, _seed);
            _slideIndex = 0;
            SlideCount = _game.Slides.Count;
        }

        public async Task ShowNextSlide()
        {
            if (SlideIndex < SlideCount)
            {
                SlideIndex++;
                CurrentSlide = _game.Slides[_slideIndex.Value - 1];
                await RenderSlide(CurrentSlide);
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

        public async Task ShowRewardSlide()
        {
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
                // if cancelled show reward
                await ShowRewardSlide();
            }
        }

        private async Task RenderSlide(Slide slide)
        {
            Console.WriteLine(slide.Image);
            // Slide image
            SlideImageSource = ImageSource.FromResource($"PathwayGames.Assets.Images.{slide.Image}");
            //SlideImageSource = ImageSource.FromFile($"slide_{slide.Image}");

            // Slide sound
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
                // Check if we allready have a correct answer
                if (_showReward)
                {
                    _showReward = false;
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
