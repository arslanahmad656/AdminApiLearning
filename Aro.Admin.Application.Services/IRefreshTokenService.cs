using Aro.Admin.Application.Services.DTOs.ServiceResponses;

namespace Aro.Admin.Application.Services;

public interface IRefreshTokenService : IService
{
    Task<RefreshToken> GenerateRefreshToken(CancellationToken cancellationToken = default);
    Task<List<UserRefreshToken>> GetActiveToken(Guid userId, CancellationToken cancellationToken = default);
    Task<UserRefreshToken> GetActiveToken(string refreshTokenHash, CancellationToken cancellationToken = default);
    Task Revoke(Guid tokenId, CancellationToken cancellationToken = default);
    Task RevokeAll(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> Revoke(Guid userId, string refreshToken, CancellationToken cancellationToken = default);
    Task<CompositeToken> RefreshToken(Guid userId, string refreshToken, CancellationToken cancellationToken = default);
    Task<CompositeToken> RefreshToken(string refreshToken, CancellationToken cancellationToken = default);
}
