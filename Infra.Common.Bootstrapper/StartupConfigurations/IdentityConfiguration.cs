using Infra.Authentication.Identity.DbContext;
using Infra.Authentication.Identity.Models;
using Infra.Authentication.Identity.Settings;
using Infra.Common.Bootstrapper.StartupSettings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infra.Common.Bootstrapper.StartupConfigurations
{
    public static class IdentityConfiguration
    {
        public static void AddCustomIdentity(this IServiceCollection services, Settings settings)
        {
            services.AddDbContextPool<AuthenticationDbContext>(options =>
            {
                options
                    .UseSqlServer(settings.ConnectionStrings.Identity);
            });

            services.AddIdentity<User,Role>(options=> {
                options.Password.RequireDigit = settings.Identity.PasswordRequireDigit;
                options.Password.RequiredLength = settings.Identity.PasswordRequiredLength;
                options.Password.RequireNonAlphanumeric = settings.Identity.PasswordRequireNonAlphanumeric;
                options.Password.RequireUppercase = settings.Identity.PasswordRequireUppercase;
                options.Password.RequireLowercase = settings.Identity.PasswordRequireLowercase;
                options.User.RequireUniqueEmail = settings.Identity.UserRequireUniqueEmail;
                options.SignIn.RequireConfirmedEmail = settings.Identity.SignInRequireConfirmedEmail;     
            })
            .AddEntityFrameworkStores<AuthenticationDbContext>()
            .AddDefaultTokenProviders();


        }
    }
}
