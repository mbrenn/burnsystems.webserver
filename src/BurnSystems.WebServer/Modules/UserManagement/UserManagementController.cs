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

            this.TemplateOrAjax(result);
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



    }
}
