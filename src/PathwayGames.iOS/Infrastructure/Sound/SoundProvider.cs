using System.IO;
using System.Threading.Tasks;
using AVFoundation;
using CoreFoundation;
using Foundation;
using PathwayGames.iOS.Infrastructure.Sound;
using PathwayGames.Infrastructure.Sound;
using Xamarin.Forms;

[assembly: Dependency(typeof(SoundProvider))]
namespace PathwayGames.iOS.Infrastructure.Sound
{
    public class SoundProvider : NSObject, ISoundService
    {
        private AVAudioPlayer _player;

        public Task PlaySoundAsync(string filename)
        {
            var tcs = new TaskCompletionSource<bool>();
            
            // Any existing sound playing?
            if (_player != null)
            {
                //Stop and dispose of any sound
                _player.Stop();
                _player = null;
            }

            string path = NSBundle.MainBundle.PathForResource(Path.GetFileNameWithoutExtension(filename),
                Path.GetExtension(filename));

            var url = NSUrl.FromString(path);

            _player = AVAudioPlayer.FromUrl(url);

            _player.PrepareToPlay();

            _player.FinishedPlaying += (object sender, AVStatusEventArgs e) => {
                _player = null;
                tcs.SetResult(true);
            };

            _player.Play();

            return tcs.Task;
        }
    }
}