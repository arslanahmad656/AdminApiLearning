using Aro.Admin.Application.Repository;
using Aro.Admin.Domain.Entities;
using Aro.Common.Infrastructure.Repository;
using Aro.Common.Infrastructure.Repository.Context;

namespace Aro.Admin.Infrastructure.Repository;

internal class RefreshTokenRepository(AroDbContext dbContext) : RepositoryBase<RefreshToken>(dbContext), IRefreshTokenRepository
{
    public Task Create(RefreshToken token, CancellationToken cancellationToken = default) => base.Add(token, cancellationToken);

    public IQueryable<RefreshToken> GetActiveTokensByUserId(Guid userId) => FindByCondition(filter: rt => rt.UserId == userId && rt.ExpiresAt > DateTime.UtcNow && rt.RevokedAt != null);

    public IQueryable<RefreshToken> GetActiveTokenById(Guid tokenId) => FindByCondition(filter: rt => rt.Id == tokenId && rt.ExpiresAt > DateTime.UtcNow && rt.RevokedAt != null);

    public IQueryable<RefreshToken> GetActiveTokenByTokenHash(string tokenHash) => FindByCondition(filter: rt => rt.TokenHash == tokenHash && rt.ExpiresAt > DateTime.UtcNow && rt.RevokedAt != null);

    public IQueryable<RefreshToken> GetActiveTokensByUserAndTokenHash(Guid userId, string tokenHash) => FindByCondition(filter: rt => rt.UserId == userId && rt.TokenHash == tokenHash && rt.ExpiresAt > DateTime.UtcNow && rt.RevokedAt != null);
}
