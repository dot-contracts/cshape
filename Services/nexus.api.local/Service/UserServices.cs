
using nexus.web.auth;
using nexus.web.auth.Data;

namespace nexus.web.auth
{
    public class UserServices : IUsers
    {
        private DbContextData _dbcontext;

        public UserServices(DbContextData dbContextData)
        {
            _dbcontext = dbContextData;
        }
        public IEnumerable<Users> GetAll()
        {
            return _dbcontext.Users;
        }

    }
}
