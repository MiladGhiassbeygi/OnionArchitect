using System;
using System.Collections.Generic;
using System.Text;

namespace Infra.Authentication.Jwt.Models
{
    public class Token
    {
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpirationTime { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpirationTime { get; set; }
    }
}
