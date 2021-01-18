using PathwayGames.Models.Enums;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using Xamarin.Forms;

namespace PathwayGames.Localization
{
    public class LocalizedResources : INotifyPropertyChanged
    {
        private readonly ResourceManager _resourceManager;
        private CultureInfo _currentCultureInfo;

        public string this[string key] => _resourceManager.GetString(key, _currentCultureInfo);

        public LocalizedResources(Type resource, string language = null)
            : this(resource, new CultureInfo(language ?? Languages.EN.ToString()))
        { }

        public LocalizedResources(Type resource, CultureInfo cultureInfo)
        {
            _currentCultureInfo = cultureInfo;
            _resourceManager = new ResourceManager(resource);

            MessagingCenter.Subscribe<object, CultureChangedMessage>(this, string.Empty, OnCultureChanged);
        }

        private void OnCultureChanged(object s, CultureChangedMessage ccm)
        {
            _currentCultureInfo = ccm.NewCultureInfo;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item"));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}