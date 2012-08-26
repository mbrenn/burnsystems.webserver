using System;
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

        public Guid TokenId
        {
            get;
            private set;
        }

        public TokenSet TokenSet
        {
            get;
            private set;
        }

        public string Username
        {
            get;
            set;
        }

        public string EncryptedPassword
        {
            get;
            set;
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

        public User(long id, TokenSet tokenSet)
        {
            this.Id = id;
            this.TokenSet = tokenSet;
        }

        /// <summary>
        /// Creates the token for the user
        /// </summary>
        /// <returns>Token of the user</returns>
        public Token CreateTokenForUser()
        {
            var token = new Token()
            {
                Id = this.TokenId,
                Name = this.Username
            };

            return token;
        }
    }
}
