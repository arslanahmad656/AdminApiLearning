using Aro.Admin.Domain.Entities;

namespace Aro.Admin.Domain.Repository;

public interface IRefreshTokenRepository
{
    Task Create(RefreshToken token, CancellationToken cancellationToken = default);

    void Update(RefreshToken token);

    IQueryable<RefreshToken> GetActiveTokensByUserId(Guid userId);

    IQueryable<RefreshToken> GetActiveTokenById(Guid tokenId);

    IQueryable<RefreshToken> GetActiveTokenByTokenHash(string tokenHash);

    IQueryable<RefreshToken> GetActiveTokensByUserAndTokenHash(Guid userId, string tokenHash);
}