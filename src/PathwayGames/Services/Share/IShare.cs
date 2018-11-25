using System.Threading.Tasks;

namespace PathwayGames.Services.Share
{
    public interface IShare
    {
        void ShareFile(string title, string message, string filePath);
    }
}
