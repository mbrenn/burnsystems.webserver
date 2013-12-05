﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BurnSystems.WebServer.Modules.UserManagement
{
    public interface IWebUser
    {
        long Id
        {
            get;
        }

        string Username
        {
            get;
        }

        /// <summary>
        /// Gets the token of the user, which impersonates the user
        /// </summary>
        Token Token
        {
            get;
        }

        /// <summary>
        /// Gets the tokenset of at the user which stores all the available credentials
        /// </summary>
        TokenSet CredentialTokenSet
        {
            get;
        }

        bool IsPasswordCorrect(string password);

        void SetPassword(string password);
		
		/// <summary>
		/// Sets the last login date
		/// </summary>
		DateTime LastLoginDate {
			get;
			set;
		}
		
		void AddOrUpdateToken(string series, string token);
		
		void RemoveToken(string series);
		
		bool CheckAndRemoveToken(string series, string token);
    }
}
