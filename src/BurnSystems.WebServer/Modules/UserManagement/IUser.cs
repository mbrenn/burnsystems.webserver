using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BurnSystems.WebServer.Modules.UserManagement
{
    public interface IUser
    {
        long Id
        {
            get;
        }

        string Username
        {
            get;
        }

        TokenSet TokenSet
        {
            get;
        }

        bool IsPasswordCorrect(string password);

        void SetPassword(string password);
    }
}
