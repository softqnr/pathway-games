using System.Threading.Tasks;

namespace PathwayGames.Services.Sound
{
    public interface ISoundService
    {
        Task PlaySoundAsync(string filename);
    }
}
