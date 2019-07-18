using System.Threading.Tasks;

namespace PathwayGames.Infrastructure.Sound
{
    public interface ISoundService
    {
        Task PlaySoundAsync(string filename);
    }
}
