﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BurnSystems.WebServer.Modules.UserManagement.InMemory
{
    [Serializable]
    public class User : IUser
    {
        public long Id
        {
            get;
            private set;
        }

        public string Username
        {
            get;
            set;
        }

        public Guid TokenId
        {
            get;
            private set;
        }

        public TokenSet TokenSet
        {
            get
            {
                return this.CreateTokenSetForUser();
            }
        }

        private string EncryptedPassword
        {
            get;
            set;
        }

        public User(long id)
        {
            this.Id = id;
            this.TokenId = Guid.NewGuid();
        }

        public User(long id, string username, string password)
            : this ( id )
        {
            this.Username = username;
            this.SetPassword(password);
        }

        public bool IsPasswordCorrect(string password)
        {
            var hash = this.Username + password;
            return this.EncryptedPassword == hash.Sha1();
        }

        public void SetPassword(string password)
        {
            this.EncryptedPassword = (this.Username + password).Sha1();
        }

        /// <summary>
        /// Creates the token for the user
        /// </summary>
        /// <returns>Token of the user</returns>
        public TokenSet CreateTokenSetForUser()
        {
            var token = new Token()
            {
                Id = this.TokenId,
                Name = this.Username
            };

            var set = new TokenSet();
            set.Add(token);

            return set;
        }
    }
}
