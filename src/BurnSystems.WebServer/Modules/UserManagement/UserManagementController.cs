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

        [Inject]
        public ITemplateParser TemplateParser
        {
            get;
            set;
        }

        /// <summary>
        /// Performs the login
        /// </summary>
        /// <param name="loginData">Data of login</param>
        /// <param name="template">Template being shown to user</param>
        public void Login([PostModel] LoginData loginData, [Inject("PageTemplate")] string template)
        {
            var user = this.Authentication.LoginUser(loginData.Username, loginData.Password);

            var result = new
            {
                Success = user != null
            };

            if (template == null)
            {
                this.Json(result);
            }
            else
            {
                this.Template(result);
            }
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



    }
}
