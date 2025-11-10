using Aro.Common.Application.Services;

namespace Aro.Admin.Application.Services.AccessToken;

public interface IAccessTokenService : IService
{
    Task<AccessTokenResponse> GenerateAccessToken(Guid userId, CancellationToken cancellationToken = default);
}
