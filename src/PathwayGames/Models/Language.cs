using PathwayGames.Helpers;
using PathwayGames.Models.Enums;

namespace PathwayGames.Models
{
    public class Language
    {
        public Languages ShortName { get; set; }

        public string Name => ShortName.GetDescription();

        public string Image => $"language_{ShortName.ToString().ToLower()}.png";

        public bool IsSelected { get; set; }

        public Language(Languages language)
        {
            ShortName = language;
        }

        public string GetShortNameText() => ShortName.ToString();
    }
}