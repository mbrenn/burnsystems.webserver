using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnSystems.ObjectActivation;
using BurnSystems.WebServer.Modules.Sessions;

namespace BurnSystems.WebServer.Modules.UserManagement
{
    public class Authentication : IAuthentication
    {
        /// <summary>
        /// Gets or sets the current session
        /// </summary>
        public Session Session
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current session
        /// </summary>
        public IWebUserManagement UserManagement
        {
            get;
            set;
        }

        [Inject]
        public Authentication(Session session, IWebUserManagement usermanagement)
        {
            this.Session = session;
            this.UserManagement = usermanagement;
        }

        public IWebUser LoginUser(string username, string password)
        {
            var user = this.UserManagement.GetUser(username, password);
            if (user == null)
            {
                return null;
            }

            this.Session["Authentication.Username"] = user.Username;
            this.Session["Authentication.TokenSet"] = user.CredentialTokenSet;

            return user;
        }

        /// <summary>
        /// Performs the logout
        /// </summary>
        public void LogoutUser()
        {
            this.Session.Remove("Authentication.Username");
        }

        public bool IsUserLoggedIn()
        {
            return this.Session["Authentication.Username"] != null;
        }

        public IWebUser GetLoggedInUser()
        {
            if (!this.IsUserLoggedIn())
            {
                return null;
            }

            var user = this.UserManagement.GetUser(
                 this.Session["Authentication.Username"].ToString());
            return user;
        }

        /// <summary>
        /// Gets a list of tokens of the current user
        /// </summary>
        /// <returns>List of Tokens</returns>
        public TokenSet GetTokensOfLogin()
        {
            return this.Session["Authentication.TokenSet"] as TokenSet;
        }
    }
}
