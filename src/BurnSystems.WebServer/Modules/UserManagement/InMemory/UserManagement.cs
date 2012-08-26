using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnSystems.ObjectActivation;

namespace BurnSystems.WebServer.Modules.UserManagement.InMemory
{
    public class UserManagement : IUserManagement
    {
        [Inject]
        public UserStorage Storage
        {
            get;
            set;
        }

        public IUser GetUser(string username)
        {
            lock (this.Storage)
            {
                return this.Storage.Users
                    .Where(x => x.Username == username)
                    .FirstOrDefault();
            }
        }

        public IUser GetUser(string username, string password)
        {
            lock (this.Storage)
            {
                return this.Storage.Users
                    .Where(x => x.Username == username && x.IsPasswordCorrect(password))
                    .FirstOrDefault();                        
            }
        }
    }
}
