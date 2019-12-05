using PathwayGames.Models;
using System.Threading.Tasks;
using PathwayGames.Data;
using System.Collections.Generic;
using PathwayGames.Models.Enums;
using System;
using Xamarin.Essentials;
using System.IO.Compression;
using System.IO;

namespace PathwayGames.Services.User
{
    public class UserService : IUserService
    {
        private readonly IRepository<Models.User> _repositoryUser;
        private readonly IRepository<UserGameSettings> _repositoryUserSettings;
        private readonly IRepository<UserGameSession> _repositoryUserGameSession;
        private readonly IRepository<SeekGridOption> _repositorySeekGridOption;

        public UserService(IRepository<Models.User> repositoryUser,
            IRepository<UserGameSettings> repositoryUserSettings,
            IRepository<UserGameSession> repositoryUserGameSession,
            IRepository<SeekGridOption> repositorySeekGridOption)
        {
            _repositoryUser = repositoryUser;
            _repositoryUserSettings = repositoryUserSettings;
            _repositoryUserGameSession = repositoryUserGameSession;
            _repositorySeekGridOption = repositorySeekGridOption;
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

        public async Task SaveGameSessionData(Game game)
        {
            Models.User user = await _repositoryUser.GetWithChildrenAsync(game.SessionData.UserId);
            var gameSession = user.AddGameSession(game, game.GameDataFile, game.SensorDataFile);
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

        public async Task CreateUser(string userName, UserType userType)
        {
            Models.User user = new Models.User() {
                UserName = userName,
                UserType = userType.ToString()
            };
            // Set it to default
            user.UserSettings.SeekGridOptions = await GetSeekGridOptionByIdiomDefault(DeviceInfo.Idiom.ToString());
            await _repositoryUser.InsertWithChildrenAsync(user, true);
        }

        public async Task<IList<SeekGridOption>> GetSeekGridOptionsByIdiom(string idiom)
        {
            return await _repositorySeekGridOption.AsQueryable()
                .Where(x => (x.TargetIdiom == idiom)).ToListAsync();
        }

        public async Task<SeekGridOption> GetSeekGridOptionByIdiomDefault(string idiom)
        {
            return await _repositorySeekGridOption.AsQueryable()
                .Where(x => (x.TargetIdiom == idiom && x.IsDefault)).FirstOrDefaultAsync();
        }

        public string PackUserGameSessions(IList<UserGameSession> gameSessions)
        {
            string destinationZipFullPath = Path.Combine(App.LocalStorageDirectory, "pack.zip");
            
            if (File.Exists(destinationZipFullPath))
                File.Delete(destinationZipFullPath);

            using (ZipArchive zip = ZipFile.Open(destinationZipFullPath, ZipArchiveMode.Create))
            {
                foreach (var gameSession in gameSessions)
                {
                    zip.CreateEntryFromFile(Path.Combine(App.LocalStorageDirectory, gameSession.SensorDataFile)
                        , gameSession.SensorDataFile
                        , CompressionLevel.Optimal);
                }
            }

            return destinationZipFullPath;
        }
    }
}
