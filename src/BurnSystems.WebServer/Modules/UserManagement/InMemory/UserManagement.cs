using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnSystems.ObjectActivation;

namespace BurnSystems.WebServer.Modules.UserManagement.InMemory
{
    public class UserManagement : IWebUserManagement
    {
        [Inject]
        public UserStorage Storage
        {
            get;
            set;
        }

        public IWebUser GetUser(string username)
        {
            lock (this.Storage)
            {
                return this.Storage.Users
                    .Where(x => x.Username == username)
                    .FirstOrDefault();
            }
        }

        public IWebUser GetUser(string username, string password)
        {
            lock (this.Storage)
            {
                return this.Storage.Users
                    .Where(x => x.Username == username && x.IsPasswordCorrect(password))
                    .FirstOrDefault();
            }
        }


        public void SetPersistantCookie(IWebUser user, string series, string token)
        {
            throw new NotImplementedException();
        }

        public bool CheckPersistantCookie(IWebUser user, string series, string token)
        {
            throw new NotImplementedException();
        }

        public void DeletePersistantCookie(IWebUser user, string series)
        {
            throw new NotImplementedException();
        }
    }
}
