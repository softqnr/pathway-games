using PathwayGames.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using PathwayGames.Models.Enums;

namespace PathwayGames.Services.User
{
    public interface IUserService
    {
        Task<IList<Models.User>> GetAll();

        Task<IList<Models.User>> GetByNameAndUserType(string name, string userType);

        Task<Models.User> GetSelectedUser();

        Task<Models.User> SetSelectedUser(Models.User user);

        Task<UserGameSession> SaveGameSessionData(Game game);

        Task<IList<UserGameSession>> GetUserGameSessions(long userId);

        string PackUserGameSessions(IList<UserGameSession> gameSessions, string fileName);

        Task<string> PackUserGameSessions(long userId);

        Task<string> PackAllUserGameSessions();

        Task<UserGameSettings> GetUserSettings(long userId);

        Task UpdateUserSettings(UserGameSettings gameSettings);

        Task CreateUser(string userName, UserType userType, int ppi, float widthCompensantion, float heightCompensantion);

        Task<IList<SeekGridOption>> GetSeekGridOptionsByIdiom(string idiom);

        Task<SeekGridOption> GetSeekGridOptionByIdiomDefault(string idiom);

        Task DeleteGameSession(UserGameSession gameSession);

        Task DeleteUser(Models.User user);

        Task UpdateLanguage(Models.User user);
    }
}
