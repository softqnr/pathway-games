using PathwayGames.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using PathwayGames.Models.Enums;

namespace PathwayGames.Services.User
{
    public interface IUserService
    {
        Task<IList<Models.User>> GetByNameAndUserType(string name, string userType);

        Task<Models.User> GetSelectedUser();

        Task<Models.User> SetSelectedUser(Models.User user);

        Task SaveGameSessionData(long userId, Game game, string gameDataFile, string sensorDataFile);

        Task<IList<UserGameSession>> GetUserGameSessions(long userId);

        Task<UserSettings> GetUserSettings(long userId);
    }
}
