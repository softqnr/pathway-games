namespace PathwayGames.Models
{
    public class UserSettings : GameSettings
    {
        public string UserName { get; set; }

        public bool EyeGazeSensor { get; set; }

        public bool EEGSensor { get; set; }

        public bool AccelarationSensor { get; set; }
    }
}
