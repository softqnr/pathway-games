using PathwayGames.Localization;
using PathwayGames.Models;
using PathwayGames.Models.Enums;
using PathwayGames.Services.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace PathwayGames.ViewModels
{
    public class LanguagesViewModel : ViewModelBase
    {
        private readonly IUserService _userService;

        private List<Language> _languagesList;

        public List<Language> LanguagesList
        {
            get => _languagesList;
            set
            {
                _languagesList = value;
                OnPropertyChanged();
            }
        }

        public ICommand LanguageSelectedCommand
        {
            get
            {
                return new Command((isSelected) =>
                {
                    if ((bool)isSelected)
                        SetLanguage();
                });
            }
        }

        public LanguagesViewModel(IUserService userService)
        {
            _userService = userService;
            LanguagesList = GetLanguageList();
            var currentLanguage = LanguagesList.FirstOrDefault(x => x.ShortName == App.CurrentLanguage.ShortName);
            currentLanguage.IsSelected = true;
        }

        private async void SetLanguage()
        {
            var selectedLanguage = LanguagesList.FirstOrDefault(x => x.IsSelected);
            if (selectedLanguage == null)
                return;

            App.CurrentLanguage = selectedLanguage;
            await _userService.UpdateLanguage(App.SelectedUser);

            MessagingCenter.Send<object, CultureChangedMessage>(this, string.Empty, new CultureChangedMessage(selectedLanguage.GetShortNameText()));
        }

        private List<Language> GetLanguageList() => 
                               Enum.GetValues(typeof(Languages))
                               .Cast<Languages>()
                               .Select(x => new Language(x))
                               .ToList();
    }
}