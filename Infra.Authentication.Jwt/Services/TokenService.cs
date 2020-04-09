using Infra.Authentication.Jwt.Contracts;
using Infra.Authentication.Jwt.DbContexts;
using Infra.Authentication.Jwt.Models;
using Infra.Authentication.Jwt.Settings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infra.Authentication.Jwt.Services
{
    public class TokenService:ITokenService
    {
        private readonly TokenStoreDbContext tokenStoreDbContext;
        private readonly JwtSettings settings;

        public TokenService(TokenStoreDbContext tokenStoreDbContext,JwtSettings settings)
        {
            this.tokenStoreDbContext = tokenStoreDbContext;
            this.settings = settings;
        }

        public async Task<UserToken> FindRefreshToken(int userId,string refreshToken,CancellationToken cancellationToken)
        {
          return await tokenStoreDbContext.UserToken.FirstOrDefaultAsync(u => u.UserId == userId && u.RefreshToken == refreshToken,cancellationToken);
        }
        public async Task SaveRefreshTokenAsync(UserToken userToken,CancellationToken cancellationToken)
        {
            await this.tokenStoreDbContext.UserToken.AddAsync(userToken,cancellationToken);
            await tokenStoreDbContext.SaveChangesAsync(cancellationToken);
        }
        public void UpdateRefreshToken(UserToken userToken)
        {
            tokenStoreDbContext.Update(userToken);
            tokenStoreDbContext.SaveChanges();
        }
        public async Task DeleteRefreshTokenAsync(long id, CancellationToken cancellationToken)
        {
            UserToken userToken = await tokenStoreDbContext.UserToken.FirstOrDefaultAsync(u=>u.Id==id);
            tokenStoreDbContext.UserToken.Remove(userToken);
            tokenStoreDbContext.SaveChanges();
            
        }
        public async Task DeleteExpiredRefreshTokensAsync(CancellationToken cancellationToken)
        {
            var expiredUserTokens = await tokenStoreDbContext.UserToken.Where(u=>u.Expiration<DateTime.Now).ToListAsync(cancellationToken);
            tokenStoreDbContext.RemoveRange(expiredUserTokens);
            tokenStoreDbContext.SaveChanges();

        }
      
    }
}
