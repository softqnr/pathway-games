using PathwayGames.Models;
using PathwayGames.Models.Enums;
using PathwayGames.Services.Slides;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PathwayGames.ViewModels
{
    public class GameViewModel : ViewModelBase
    {
        private Game _game;
        private ISlidesService _slidesService;

        private int? _slideIndex;
        private int? _slideCount;
        private string _image;
        private string _seed;
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
        public string Seed
        {
            get => _seed;
            set => SetProperty(ref _seed, value);
        }
        public GameViewModel(ISlidesService slidesService)
        {
            _slidesService = slidesService;
        }
        // Commands
        public Command<Point> ButtonTappedCommand { get { return new Command<Point>((p) => OnButtonTapped(p)); } }

        public void OnButtonTapped(Point p)
        {
            // Button tap

        }

        private void CreateGame(GameType gameType)
        {
            _game = new Game();
            _game.Slides = _slidesService.Generate(gameType, "");
            _slideIndex = 0;
            SlideCount = _game.Slides.Count;
        }

        public void ShowNextSlide()
        {
            if (SlideIndex <= SlideCount)
            {
                SlideIndex += 1;
                Image = $"resource://PathwayGames.Assets.Images.{_game.Slides[_slideIndex.Value - 1].Image}";

                //Set slide timeout timer
                Device.StartTimer(TimeSpan.FromSeconds(1.5), () =>
                {
                    // Change slide
                    ShowNextSlide();
                    return false; // True = Repeat again, False = Stop the timer
                });
            }
        }

        public void ShowBlankSlide()
        {

        }

        public void ShowRewardSlide()
        {

        }

        public override async Task InitializeAsync(object navigationData)
        {
            if (navigationData != null &&
                Enum.TryParse<GameType>(navigationData.ToString(), out var gameType))
            {
                CreateGame(gameType);
                ShowNextSlide();
                //await LoadData(gameType);
            }
        }
    }
}
