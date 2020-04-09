using Framework.Core.Extentions;
using Framework.Web.Extentions;
using Infra.Authentication.Identity.Models;
using Infra.Authentication.Identity.Services;
using Infra.Authentication.Jwt.Contracts;
using Infra.Authentication.Jwt.DbContexts;
using Infra.Authentication.Jwt.Services;
using Infra.Authentication.Jwt.Settings;
using Infra.Common.Bootstrapper.StartupSettings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infra.Common.Bootstrapper.StartupConfigurations
{
    public static class JwtConfiguration
    {
        public static void AddCustomJwtAuthentication(this IServiceCollection services, Settings settings)
        {
            services.AddDbContextPool<TokenStoreDbContext>(options =>
            {
                options
                    .UseSqlServer(settings.ConnectionStrings.TokenStore);
            });
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(options =>
            {
                var secretkey = Encoding.UTF8.GetBytes(settings.Jwt.SecretKey);
                var encryptionkey = Encoding.UTF8.GetBytes(settings.Jwt.Encryptkey);
                options.SaveToken = true;
                var validationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero, // default: 5 min
                    RequireSignedTokens = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(secretkey),

                    RequireExpirationTime = true,
                    ValidateLifetime = true,

                    ValidateAudience = true, //default : false
                    ValidAudience = settings.Jwt.Audience,

                    ValidateIssuer = true, //default : false
                    ValidIssuer = settings.Jwt.Issuer,

                    TokenDecryptionKey = new SymmetricSecurityKey(encryptionkey)
                };

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = validationParameters;
                options.Events = new JwtBearerEvents
                {
                   
                    OnAuthenticationFailed = context =>
                    {
                        return Task.CompletedTask;
                    },
                    OnTokenValidated =  context =>
                    {
                        var signInManager = context.HttpContext.RequestServices.GetRequiredService<SignInManager<User>>();
                       
                        
                        var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                        if (claimsIdentity.Claims?.Any() != true)
                            context.Fail("This token has no claims.");

                        var securityStamp = claimsIdentity.FindFirstValue(new ClaimsIdentityOptions().SecurityStampClaimType);
                        if (!securityStamp.HasValue())
                            context.Fail("This token has no secuirty stamp");
         
                        var validatedUser =  signInManager.ValidateSecurityStampAsync(context.Principal).Result;
                        if (validatedUser == null)
                            context.Fail("Token secuirty stamp is not valid.");

                    
                       return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        
                        string error = context.Error;
                        string errorDescription = context.ErrorDescription;
                       

                        if (context.AuthenticateFailure != null)
                        {
                            Exception failure = context.AuthenticateFailure;
                        }
                          
                        
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddScoped<ITokenFactory, TokenFactory>(provider=> {               
                return new TokenFactory(settings.Jwt);
            });
            services.AddScoped<ITokenService, TokenService>(provider => {
                var tokenStoreDbContext = provider.GetService<TokenStoreDbContext>();
                return new TokenService(tokenStoreDbContext, settings.Jwt);
            });
        }
    }
}
