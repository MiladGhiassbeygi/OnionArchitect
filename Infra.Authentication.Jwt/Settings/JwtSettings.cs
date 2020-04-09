using System;
using System.Collections.Generic;
using System.Text;

namespace Infra.Authentication.Jwt.Settings
{
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public string Encryptkey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int NotBeforeMinutes { get; set; }
        public int AccessTokenExpirationMinutes { get; set; }
        public int RefreshTokenExpirationMinutes { get; set; }
        public bool AllowMultipleLoginsFromTheSameUser { get; set; }
        public bool AllowSignoutAllUserActiveClients { get; set; }
    }
}
