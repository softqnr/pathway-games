using System.IO;
using System.Threading.Tasks;
using AVFoundation;
using Foundation;
using PathwayGames.iOS.Services.Sound;
using PathwayGames.Services.Sound;
using Xamarin.Forms;

[assembly: Dependency(typeof(SoundProvider))]
namespace PathwayGames.iOS.Services.Sound
{
    public class SoundProvider : NSObject, ISoundService
    {
        private AVAudioPlayer _player;

        public Task PlaySoundAsync(string filename)
        {
            var tcs = new TaskCompletionSource<bool>();

            string path = NSBundle.MainBundle.PathForResource(Path.GetFileNameWithoutExtension(filename),
                Path.GetExtension(filename));

            var url = NSUrl.FromString(path);
            _player = AVAudioPlayer.FromUrl(url);

            _player.FinishedPlaying += (object sender, AVStatusEventArgs e) => {
                _player = null;
                tcs.SetResult(true);
            };

            _player.Play();

            return tcs.Task;
        }
    }
}