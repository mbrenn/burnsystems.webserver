using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BurnSystems.WebServer.Modules.UserManagement
{
    public interface IWebUserManagement
    {
        IWebUser GetUser(string username);
        IWebUser GetUser(string username, string password);
    }
}
