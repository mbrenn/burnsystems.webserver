using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnSystems.WebServer.Modules.MVC;
using BurnSystems.ObjectActivation;
using BurnSystems.WebServer.Parser;

namespace BurnSystems.WebServer.Modules.UserManagement
{
    public class UserManagementController : Controller
    {
        [Inject]
        public IAuthentication Authentication
        {
            get;
            set;
        }

        /// <summary>
        /// Performs the login
        /// </summary>
        /// <param name="loginData">Data of login</param>
        /// <param name="template">Template being shown to user</param>
        [WebMethod]
        public void Login([PostModel] LoginData loginData)
        {
            var user = this.Authentication.LoginUser(loginData.Username, loginData.Password);

            var result = new
            {
                Success = user != null
            };

            this.TemplateOrJson(result);
        }

        [WebMethod]
        public void Logout()
        {
            this.Authentication.LogoutUser();
            var result = new
            {
                Success = true
            };

            this.TemplateOrJson(result);
        }

        [WebMethod]
        public void IsUserLoggedIn()
        {
            var loggedIn = this.Authentication.IsUserLoggedIn();

            var result = new
            {
                IsUserLoggedIn = loggedIn
            };

            this.TemplateOrJson(result);
        }

        [WebMethod]
        public void CurrentUser()
        {
            var user = this.Authentication.GetLoggedInUser();

            object result;

            if (user == null)
            {
                result = new
                {
                    IsUserLoggedIn = false
                };
            }
            else
            {
                result = new
                {
                    IsUserLoggedIn = true,
                    UserId = user.Id,
                    Username = user.Username
                };
            }

            this.TemplateOrJson(result);
        }

        public class LoginData
        {
            public string Username
            {
                get;
                set;
            }

            public string Password
            {
                get;
                set;
            }
        }

        public class LoginResult
        {
            public bool Success
            {
                get;
                set;
            }
        }

        public class IsUserLoggedInResult
        {
            public bool IsUserLoggedIn
            {
                get;
                set;
            }
        }

        public class GetLoggedInUserResult
        {
            public bool IsUserLoggedIn
            {
                get;
                set;
            }

            public long UserId
            {
                get;
                set;
            }

            public string Username
            {
                get;
                set;
            }
        }
    }
}
