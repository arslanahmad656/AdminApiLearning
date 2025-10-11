using Aro.Admin.Application.Services.DTOs.ServiceResponses;

namespace Aro.Admin.Application.Services;

public interface IRefreshTokenService
{
    Task<RefreshToken> GenerateRefreshToken(CancellationToken cancellationToken = default);
    Task<RefreshToken> GetActiveToken(Guid userId, CancellationToken cancellationToken = default);
    Task Revoke(Guid tokenId, CancellationToken cancellationToken = default);
    Task RevokeAll(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> Revoke(Guid userId, string refreshToken, CancellationToken cancellationToken = default);
    Task<CompositeToken> RefreshToken(Guid userId, string refreshToken, CancellationToken cancellationToken = default);
}
