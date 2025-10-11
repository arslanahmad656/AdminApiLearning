using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Infrastructure.Repository.Context;

namespace Aro.Admin.Infrastructure.Repository.Repositories;

internal class RefreshTokenRepository(AroAdminApiDbContext dbContext) : RepositoryBase<RefreshToken>(dbContext), IRefreshTokenRepository
{
    public Task Create(RefreshToken token, CancellationToken cancellationToken = default) => base.Add(token, cancellationToken);

    public IQueryable<RefreshToken> GetActiveTokensByUserId(Guid userId) => FindByCondition(filter: rt => rt.UserId == userId && rt.ExpiresAt > DateTime.UtcNow && rt.RevokedAt != null);
    
    public IQueryable<RefreshToken> GetActiveTokenById(Guid tokenId) => FindByCondition(filter: rt => rt.Id == tokenId && rt.ExpiresAt > DateTime.UtcNow && rt.RevokedAt != null);

    public IQueryable<RefreshToken> GetActiveTokensByUserAndTokenHash(Guid userId, string tokenHash) => FindByCondition(filter: rt => rt.UserId == userId && rt.TokenHash == tokenHash && rt.ExpiresAt > DateTime.UtcNow && rt.RevokedAt != null);
}
