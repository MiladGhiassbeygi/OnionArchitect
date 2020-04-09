using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnionArchitect.Api.V1.Forms.Accounts
{
    public class RefreshTokenForm
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
