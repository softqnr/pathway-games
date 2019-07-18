using System.Threading.Tasks;
using Xamarin.Forms;

namespace PathwayGames.Infrastructure.Sound
{
    public class SoundService : ISoundService
    {
        private ISoundService _soundProvider;
        public SoundService()
        {
            _soundProvider = DependencyService.Get<ISoundService>();
        }

        public async Task PlaySoundAsync(string filename)
        {
            await _soundProvider.PlaySoundAsync(filename);
        }
    }
}
