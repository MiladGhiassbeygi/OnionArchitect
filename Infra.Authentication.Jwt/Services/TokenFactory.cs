using Infra.Authentication.Jwt.Contracts;
using Infra.Authentication.Jwt.Models;
using Infra.Authentication.Jwt.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infra.Authentication.Identity.Services
{
   
    public class TokenFactory: ITokenFactory
    {
        private readonly JwtSettings settings;
        public TokenFactory(JwtSettings settings)
        {
            this.settings = settings;
        }
        public Token GenerateToken(IEnumerable<Claim> claims)
        {
            var accessTokenExpirationTime = DateTime.Now.AddMinutes(settings.AccessTokenExpirationMinutes);
            var refreshTokenExpirationTime = DateTime.Now.AddMinutes(settings.RefreshTokenExpirationMinutes);
            {
                var secretKey = Encoding.UTF8.GetBytes(settings.SecretKey); // longer that 16 character
                var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature);

                var encryptionkey = Encoding.UTF8.GetBytes(settings.Encryptkey); //must be 16 character
                var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptionkey), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

               

                var descriptor = new SecurityTokenDescriptor
                {
                    Issuer = settings.Issuer,
                    Audience = settings.Audience,
                    IssuedAt = DateTime.Now,
                    NotBefore = DateTime.Now.AddMinutes(settings.NotBeforeMinutes),
                    Expires = accessTokenExpirationTime,
                    SigningCredentials = signingCredentials,
                    EncryptingCredentials = encryptingCredentials,
                    Subject = new ClaimsIdentity(claims)
                };

                //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
                //JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
                //JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

                var tokenHandler = new JwtSecurityTokenHandler();

                var securityToken = tokenHandler.CreateToken(descriptor);

                Token token = new Token();
                token.AccessToken= tokenHandler.WriteToken(securityToken);
                token.AccessTokenExpirationTime = accessTokenExpirationTime;
                token.RefreshToken = this.GenerateRefreshToken();
                token.RefreshTokenExpirationTime = refreshTokenExpirationTime;

                return token;
            }

        }
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var secretkey = Encoding.UTF8.GetBytes(settings.SecretKey);
            var encryptionkey = Encoding.UTF8.GetBytes(settings.Encryptkey);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.Zero, // default: 5 min
                RequireSignedTokens = true,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretkey),

                RequireExpirationTime = true,
                ValidateLifetime = true,

                ValidateAudience = true, //default : false
                ValidAudience = settings.Audience,

                ValidateIssuer = true, //default : false
                ValidIssuer = settings.Issuer,

                TokenDecryptionKey = new SymmetricSecurityKey(encryptionkey)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.Aes128KW, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;

         
        }
        protected string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
