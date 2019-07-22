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

        public UserService(IRepository<Models.User> repositoryUser)
        {
            _repositoryUser = repositoryUser;

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
            Models.User currentSelectedUser = await GetSelectedUser();
            currentSelectedUser.IsSelected = false;
            await _repositoryUser.UpdateAsync(currentSelectedUser);
            user.IsSelected = true;
            await _repositoryUser.UpdateAsync(user);
            return await _repositoryUser.GetAsync(user.Id);
        }
    }
}
