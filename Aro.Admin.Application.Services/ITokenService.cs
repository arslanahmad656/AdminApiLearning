using Aro.Admin.Application.Services.DTOs.ServiceResponses;

namespace Aro.Admin.Application.Services;

public interface ITokenService
{
    Task<TokenResponse> GenerateToken(Guid userId, CancellationToken cancellationToken = default);
}
