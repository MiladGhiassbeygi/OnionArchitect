using Infra.Authentication.Jwt.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infra.Authentication.Jwt.Contracts
{
    public interface ITokenService
    {
        Task DeleteExpiredRefreshTokensAsync(CancellationToken cancellationToken);
        Task DeleteRefreshTokenAsync(long id, CancellationToken cancellationToken);
        Task<UserToken> FindRefreshToken(int userId, string refreshToken,CancellationToken cancellationToken);
       void UpdateRefreshToken(UserToken userToken);
        Task SaveRefreshTokenAsync(UserToken userToken, CancellationToken cancellationToken);
    }
}
