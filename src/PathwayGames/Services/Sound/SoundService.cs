using System.Threading.Tasks;
using Xamarin.Forms;

namespace PathwayGames.Services.Sound
{
    public class SoundService : ISoundService
    {
        private ISoundService _soundProvider;
        public SoundService()
        {
            _soundProvider = DependencyService.Get<ISoundService>();
        }

        public Task PlaySoundAsync(string filename)
        {
            return _soundProvider.PlaySoundAsync(filename);
        }
    }
}
