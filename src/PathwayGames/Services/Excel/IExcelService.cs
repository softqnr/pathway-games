using PathwayGames.Models;
using System.Threading.Tasks;

namespace PathwayGames.Services.Excel
{
    public interface IExcelService
    {
        Task<string> ExportAsync(Game game);
        string Export(Game game);
    }
}
