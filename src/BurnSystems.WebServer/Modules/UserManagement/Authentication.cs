﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnSystems.ObjectActivation;
using BurnSystems.WebServer.Modules.Sessions;
using BurnSystems.Test;
using BurnSystems.WebServer.Dispatcher;
using System.Net;

namespace BurnSystems.WebServer.Modules.UserManagement
{
    public class Authentication : IAuthentication
    {
        /// <summary>
        /// Defines the cookie name
        /// </summary>
        private const string cookieName = "BS.Auth.Cookie";

        /// <summary>
        /// Gets or sets the current session
        /// </summary>
        [Inject(IsMandatory=true)]
        public Session Session
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current session
        /// </summary>
        [Inject(IsMandatory=true)]
        public IWebUserManagement UserManagement
        {
            get;
            set;
        }

        [Inject(IsMandatory = true)]
        public HttpListenerContext ListenerContext
        {
            get;
            set;
        }

        /// <summary>
        /// Performs the login
        /// </summary>
        /// <param name="username">Name of the user</param>
        /// <param name="password">Password of user (or at least assumed password)</param>
        /// <param name="isPersistant">true, if credentials shall be stored in cookie</param>
        /// <returns>Retrieved webuser</returns>
        public IWebUser LoginUser(string username, string password, bool isPersistant = false)
        {
            var user = this.UserManagement.GetUser(username, password);
            if (user == null)
            {
                return null;
            }

            this.Session["Authentication.Username"] = user.Username;
            this.Session["Authentication.TokenSet"] = user.CredentialTokenSet;
                        
            if (isPersistant)
            {
                // Store cookies over lifetime, add some additional secrets
                this.AssignPersistantCookie(user);
            }

            return user;
        }

        /// <summary>
        /// Performs the logout
        /// </summary>
        public void LogoutUser()
        {
            if (this.IsUserLoggedIn())
            {
                this.Session.Remove("Authentication.Username");

                // Checks, if we have a permanent login that needs to be stopped
                if (this.Session["Authentication.PersistantSeries"] != null)
                {
                    var user = this.UserManagement.GetUser(this.Session["Authentication.Username"] as string);
                    if (user != null)
                    {
                        this.UserManagement.DeletePersistantCookie(
                            user,
                            this.Session["Authentication.PersistantSeries"] as string);
                    }
                }
            }
        }

        public bool IsUserLoggedIn()
        {
            var isLoggedIn = this.Session["Authentication.Username"] != null;
            if ( isLoggedIn )
            {
                return true;
            }

            // Check, if user is logged via permanent cookie
            var cookie = this.ListenerContext.Request.Cookies[cookieName];
            if (cookie != null)
            {
                // Check for cookie
                var splitted = cookie.Value.Split(new[] { '|' });
                if (splitted.Length != 3)
                {
                    return false;
                }
                var username = splitted[0];
                var series = splitted[1];
                var token = splitted[2];

                // Get user
                var user = this.UserManagement.GetUser(username);
                if (user == null)
                {
                    // User not found
                    return false;
                }

                if (this.UserManagement.CheckPersistantCookie(user, series, token))
                {
                    // Login, create new token
                    this.AssignPersistantCookie(user, series);

                    // Perform the login
                    this.Session["Authentication.Username"] = user.Username;
                    this.Session["Authentication.TokenSet"] = user.CredentialTokenSet;
                }
                else
                {
                    // No login
                    return false;
                }
            }

            // No cookie
            return false;
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
            if (!this.IsUserLoggedIn())
            {
                return null;
            }

            return this.Session["Authentication.TokenSet"] as TokenSet;
        }

        /// <summary>
        /// Assigns a persistant cookie to the user that is used to reauthenticate the user
        /// Idea taken from: http://stackoverflow.com/questions/244882/what-is-the-best-way-to-implement-remember-me-for-a-website        
        /// </summary>
        /// <param name="user">User, which shall be assigned</param>
        /// <param name="series">Series which shall be reused</param>
        private void AssignPersistantCookie(IWebUser user, string series = null)
        {
            if (string.IsNullOrEmpty(series))
            {
                series = StringManipulation.SecureRandomString(32);
            }

            var token = StringManipulation.SecureRandomString(32);

            var cookieValue = string.Format(
                "{0}|{1}|{2}",
                user.Id,
                series,
                token);

            var cookie = new Cookie(
                cookieName,
                cookieValue);
            cookie.Expires = DateTime.Now.AddDays(30);

            this.UserManagement.SetPersistantCookie(user, series, token);
            this.Session["Authentication.PersistantSeries"] = series;

            this.ListenerContext.Response.SetCookie(cookie);
        }
    }
}
