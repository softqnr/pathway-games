using System.Threading.Tasks;
using Android.Media;
using PathwayGames.Droid.Infrastructure.Sound;
using PathwayGames.Infrastructure.Sound;
using Xamarin.Forms;

[assembly: Dependency(typeof(SoundProvider))]
namespace PathwayGames.Droid.Infrastructure.Sound
{
    public class SoundProvider : ISoundService
    {
        private MediaPlayer player;
        public async Task PlaySoundAsync(string filename)
        {
            // Create media player
            player = player ?? new MediaPlayer();

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
            //player.SetDataSource(fd.FileDescriptor, fd.StartOffset, fd.Length);

            player.PrepareAsync();           
        }

    }
}