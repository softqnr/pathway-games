using System.Threading.Tasks;

namespace PathwayGames.Infrastructure.Share
{
    public interface IShare
    {
        void ShareFile(string title, string message, string filePath);
    }
}
