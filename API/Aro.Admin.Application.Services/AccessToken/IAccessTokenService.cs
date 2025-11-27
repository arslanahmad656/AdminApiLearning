using Aro.Common.Application.Shared;

namespace Aro.Admin.Application.Services.AccessToken;

public interface IAccessTokenService : IService
{
    Task<AccessTokenResponse> GenerateAccessToken(Guid userId, CancellationToken cancellationToken = default);
}
