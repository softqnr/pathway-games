using System.Threading.Tasks;
using Android.Media;
using PathwayGames.Droid.Services.Sound;
using PathwayGames.Services.Sound;
using Xamarin.Forms;

[assembly: Dependency(typeof(SoundProvider))]
namespace PathwayGames.Droid.Services.Sound
{
    public class SoundProvider : ISoundService
    {
        MediaPlayer player;
        public async Task PlaySoundAsync(string filename)
        {
            // Create media player
            if (player == null)
            {
                player = new MediaPlayer();
            }

            player.Reset();

            // Open the resource
            var fd = Android.App.Application.Context.Assets.OpenFd(filename);

            // Hook up some events
            player.Prepared += (s, e) => {
                player.Start();
            };

            player.Completion += (sender, e) => {
                Task.FromResult(true);
            };

            // Initialize
            await player.SetDataSourceAsync(fd.FileDescriptor, fd.StartOffset, fd.Length);

            player.PrepareAsync();           
        }

    }
}