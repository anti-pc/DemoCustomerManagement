using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.TokenAuthentication
{
    public class TokenManager : ITokenManager
    {
        private List<Token> listTokens;

        public TokenManager()
        {
            listTokens = new List<Token>();
        }

        public bool Authenticate(string username, string password)
        {

            if (!string.IsNullOrEmpty(username) &&
                !string.IsNullOrEmpty(password) &&
                username.ToLower() == "admin" && password.ToLower() == "password")
                return true;

            return false;

        }

        public Token NewToken()
        {
            var token = new Token
            {
                Value = Guid.NewGuid().ToString(),
                ExpirationDate = DateTime.Now.AddMinutes(5)
            };

            listTokens.Add(token);
            return token;
        }

        public bool VerifyToken(string token)
        {
            if (listTokens.Any(x => x.Value == token && x.ExpirationDate > DateTime.Now))
                return true;

            return false;
        }

    }
}
