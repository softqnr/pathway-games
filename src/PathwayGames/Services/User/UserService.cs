using PathwayGames.Infrastructure.Data;
using PathwayGames.Helpers;
using PathwayGames.Models;
using PathwayGames.Models.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace PathwayGames.Services.User
{
    public class UserService : IUserService
    {
        private readonly IRepository<Models.User> _repositoryUser;
        private readonly IRepository<UserGameSettings> _repositoryUserSettings;
        private readonly IRepository<UserGameSession> _repositoryUserGameSession;
        private readonly IRepository<SeekGridOption> _repositorySeekGridOption;
        const string AllUserType = "All";

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
            return await _repositoryUser.AsQueryable().Where(x => (x.UserType == userType || userType == AllUserType)
                && x.UserName.StartsWith(name, StringComparison.CurrentCultureIgnoreCase)).ToListAsync();
        }

        public async Task<IList<Models.User>> GetAll()
        {
            return await _repositoryUser.GetAllWithChildrenAsync();
        }

        public async Task<Models.User> GetSelectedUser()
        {
            var users = await _repositoryUser.GetAllWithChildrenAsync(x => x.IsSelected);
            if (users.Count > 0) {
                return users[0];
            } else {
                return null;
            }
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

        public async Task<UserGameSession> SaveGameSessionData(Game game)
        {
            Models.User user = await _repositoryUser.GetWithChildrenAsync(game.SessionData.UserId);
            UserGameSession gameSession = user.AddGameSession(game, game.GameDataFile);
            await _repositoryUserGameSession.InsertAsync(gameSession);
            return gameSession;
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

        public async Task CreateUser(string userName, UserType userType, int ppi, float widthCompensantion, float heightCompensantion)
        {
            Models.User user = new Models.User() {
                UserName = userName,
                UserType = userType.ToString()
            };
            // Set it to default
            user.UserSettings.SeekGridOptions = await GetSeekGridOptionByIdiomDefault(DeviceInfo.Idiom.ToString());
            // Visualization defaults
            user.UserSettings.ScreenPPI = ppi;
            user.UserSettings.VisualizationWidthCompensation = widthCompensantion;
            user.UserSettings.VisualizationHeightCompensation = heightCompensantion;

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

        public string PackUserGameSessions(IList<UserGameSession> gameSessions, string fileName)
        {
            string destinationZipFullPath = Path.Combine(App.LocalStorageDirectory, fileName);
            
            if (File.Exists(destinationZipFullPath))
                File.Delete(destinationZipFullPath);

            using (ZipArchive zip = ZipFile.Open(destinationZipFullPath, ZipArchiveMode.Create))
            {
                foreach (var gameSession in gameSessions)
                {
                    zip.CreateEntryFromFile(Path.Combine(App.LocalStorageDirectory, gameSession.GameDataFile)
                        , gameSession.GameDataFile
                        , CompressionLevel.Optimal);
                }
            }

            return destinationZipFullPath;
        }

        public async Task<string> PackUserGameSessions(long userId)
        {
            // Get current user game sessions
            Models.User user = await _repositoryUser.GetAsync(userId);
            IList<UserGameSession> gameSessions = await GetUserGameSessions(userId);
            // Create filename e.g. Dejan_2019-01-21.zip
            string fileName = $"{user.UserName}_{DateTime.Now.ToString("MM-dd-yyyy")}.zip";
            fileName = FileHelper.MakeValidFileName(fileName);
            return PackUserGameSessions(gameSessions, fileName);
        }

        public async Task<string> PackAllUserGameSessions()
        {
            // Get all game sessions
            IList<UserGameSession> gameSessions = await _repositoryUserGameSession.GetAllAsync();
            // Create filename e.g. BulkExport_Dejan_2019-01-21.zip
            string fileName = $"BulkExport_{App.SelectedUser.UserName}_{DateTime.Now.ToString("MM-dd-yyyy")}.zip";
            fileName = FileHelper.MakeValidFileName(fileName);
            return PackUserGameSessions(gameSessions, fileName);
        }

        public async Task DeleteGameSession(UserGameSession gameSession)
        {
            string sensorDataFile = Path.Combine(App.LocalStorageDirectory, gameSession.GameDataFile);

            if (File.Exists(sensorDataFile))
                File.Delete(sensorDataFile);

            await _repositoryUserGameSession.DeleteAsync(gameSession);
        }

        public async Task DeleteUser(Models.User user)
        {
            foreach(var gameSession in user.GameSessions)
            {
                await DeleteGameSession(gameSession);
            }
        }

        public async Task UpdateLanguage(Models.User user)
        {
            await _repositoryUser.UpdateAsync(user);
        }
    }
}
