using Infra.Authentication.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infra.Authentication.Identity.DbContext
{
    public class AuthenticationDbContext: IdentityDbContext<User, Role, int>
    {
        public AuthenticationDbContext(DbContextOptions options)
            : base(options)
        {

        }
    }


}
