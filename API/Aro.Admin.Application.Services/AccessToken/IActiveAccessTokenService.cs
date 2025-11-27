using Aro.Common.Application.Shared;

namespace Aro.Admin.Application.Services.AccessToken;

public interface IActiveAccessTokenService : IService
{
    Task RegisterToken(Guid userId, string tokenIdentifier, DateTime expiry, CancellationToken cancellationToken = default);
    Task<List<TokenInfo>> GetActiveTokens(Guid userId, CancellationToken cancellationToken = default);
    Task RemoveToken(Guid userId, string tokenIdentifier, CancellationToken cancellationToken = default);
    Task RemoveAllTokens(Guid userId, CancellationToken cancellationToken = default);
}