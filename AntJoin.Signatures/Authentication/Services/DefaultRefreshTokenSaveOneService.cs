using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AntJoin.Signatures
{
    public class DefaultRefreshTokenSaveOneService : DefaultRefreshTokenService
    {
        public DefaultRefreshTokenSaveOneService(IRefreshTokenStore refreshTokenStore, IProfileService profile, ISystemClock clock, ILogger<DefaultRefreshTokenService> logger) 
            : base(refreshTokenStore, profile, clock, logger)
        {
        }

        public override async Task<string> CreateRefreshTokenAsync(ClaimsPrincipal subject, Token accessToken, Client client)
        {
            await RefreshTokenStore.RemoveRefreshTokensAsync(accessToken.SubjectId, client.ClientId);
            var rToken = await base.CreateRefreshTokenAsync(subject, accessToken, client);
            return rToken;

        }

        public override async Task<string> UpdateRefreshTokenAsync(string handle, RefreshToken refreshToken, Client client)
        {
            var rToken = await base.UpdateRefreshTokenAsync(handle, refreshToken, client);
            return rToken;
        }
    }
}
