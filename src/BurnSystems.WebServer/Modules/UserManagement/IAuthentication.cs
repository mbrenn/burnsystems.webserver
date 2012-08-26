using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BurnSystems.WebServer.Modules.UserManagement
{
    public interface IAuthentication
    {
        /// <summary>
        /// Performs the login for the user
        /// </summary>
        /// <param name="username">Name of the user</param>
        /// <param name="password">Password of the user</param>
        /// <returns>Found user, if successful, otherwise null</returns>
        IUser LoginUser(string username, string password);

        /// <summary>
        /// Performs logout of the user
        /// </summary>
        void LogoutUser();

        /// <summary>
        /// Checks, if a user is logged in
        /// </summary>
        /// <returns>Checks, if user is logged in</returns>
        bool IsUserLoggedIn();

        /// <summary>
        /// Gets the currently logged in user
        /// </summary>
        /// <returns>User, who is logged in</returns>
        IUser GetLoggedInUser();

        /// <summary>
        /// Gets a list of tokens of the current user
        /// </summary>
        /// <returns>List of Tokens</returns>
        TokenSet GetTokensOfLogin();
    }
}
