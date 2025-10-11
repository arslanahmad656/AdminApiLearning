using Aro.Admin.Application.Services.DTOs.ServiceResponses;

namespace Aro.Admin.Application.Services;

public interface IAccessTokenService
{
    Task<AccessTokenResponse> GenerateAccessToken(Guid userId, CancellationToken cancellationToken = default);
}
