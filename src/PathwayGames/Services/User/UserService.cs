using PathwayGames.Models;
using System.Threading.Tasks;
using PathwayGames.Data;
using System.Collections.Generic;
using PathwayGames.Models.Enums;
using System;

namespace PathwayGames.Services.User
{
    public class UserService : IUserService
    {
        private IRepository<Models.User> _repositoryUser;
        private IRepository<UserGameSettings> _repositoryUserSettings;
        private IRepository<UserGameSession> _repositoryUserGameSession;

        public UserService(IRepository<Models.User> repositoryUser,
            IRepository<UserGameSettings> repositoryUserSettings,
            IRepository<UserGameSession> repositoryUserGameSession)
        {
            _repositoryUser = repositoryUser;
            _repositoryUserSettings = repositoryUserSettings;
            _repositoryUserGameSession = repositoryUserGameSession;
        }

        public async Task<IList<Models.User>> GetByNameAndUserType(string name, string userType)
        {
            return await _repositoryUser.AsQueryable().Where(x => (x.UserType == userType || userType == "All")
                && x.UserName.StartsWith(name, StringComparison.CurrentCultureIgnoreCase)).ToListAsync();
        }

        public async Task<Models.User> GetSelectedUser()
        {
            return await _repositoryUser.AsQueryable().Where(x => x.IsSelected).FirstOrDefaultAsync();
        }

        public async Task<Models.User> SetSelectedUser(Models.User user)
        {
            if (user == null)
                throw new ArgumentException("User cannot be null");

            Models.User currentSelectedUser = await GetSelectedUser();
            currentSelectedUser.IsSelected = false;
            await _repositoryUser.UpdateAsync(currentSelectedUser);
            user.IsSelected = true;
            await _repositoryUser.UpdateAsync(user);
            return await _repositoryUser.GetAsync(user.Id);
        }

        public async Task SaveGameSessionData(long userId, Game game, string gameDataFile, string sensorDataFile)
        {
            Models.User user = await _repositoryUser.GetWithChildrenAsync(userId);
            var gameSession = user.AddGameSession(game, gameDataFile, sensorDataFile);
            await _repositoryUserGameSession.InsertAsync(gameSession);
        }

        public async Task<IList<UserGameSession>> GetUserGameSessions(long userId)
        {
            Models.User user = await _repositoryUser.GetWithChildrenAsync(userId);
            return user.GameSessions;
        }

        public async Task<UserGameSettings> GetUserSettings(long userId)
        {
            Models.User user = await _repositoryUser.GetWithChildrenAsync(userId, true);
            return user.UserSettings;
        }

        public async Task UpdateUserSettings(UserGameSettings gameSettings)
        {
            await _repositoryUserSettings.UpdateWithChildrenAsync(gameSettings);
        }
    }
}
