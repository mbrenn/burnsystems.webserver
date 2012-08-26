using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BurnSystems.WebServer.Modules.UserManagement
{
    public interface IUserManagement
    {
        IUser GetUser(string username);
        IUser GetUser(string username, string password);
    }
}
