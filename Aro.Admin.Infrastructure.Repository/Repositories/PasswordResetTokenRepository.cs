using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Infrastructure.Repository.Context;

namespace Aro.Admin.Infrastructure.Repository.Repositories;

internal class PasswordResetTokenRepository(AroAdminApiDbContext dbContext) : RepositoryBase<PasswordResetToken>(dbContext), IPasswordResetTokenRepository
{
    public Task Create(PasswordResetToken token, CancellationToken cancellationToken = default) => base.Add(token, cancellationToken);

    public IQueryable<PasswordResetToken> GetActiveTokensByUserId(Guid userId) => 
        FindByCondition(filter: prt => prt.UserId == userId && prt.Expiry > DateTime.UtcNow && !prt.IsUsed);
    
    public IQueryable<PasswordResetToken> GetActiveTokenById(Guid tokenId) => 
        FindByCondition(filter: prt => prt.Id == tokenId && prt.Expiry > DateTime.UtcNow && !prt.IsUsed);

    public IQueryable<PasswordResetToken> GetActiveTokenByTokenHash(string tokenHash) => 
        FindByCondition(filter: prt => prt.TokenHash == tokenHash && prt.Expiry > DateTime.UtcNow && !prt.IsUsed);

    public IQueryable<PasswordResetToken> GetActiveTokensByUserAndTokenHash(Guid userId, string tokenHash) => 
        FindByCondition(filter: prt => prt.UserId == userId && prt.TokenHash == tokenHash && prt.Expiry > DateTime.UtcNow && !prt.IsUsed);

    public IQueryable<PasswordResetToken> GetExpiredTokens() => 
        FindByCondition(filter: prt => prt.Expiry <= DateTime.UtcNow);

    public void DeleteExpiredTokens()
    {
        var expiredTokens = GetExpiredTokens().ToList();
        if (expiredTokens.Any())
        {
            base.DeleteRange(expiredTokens);
        }
    }
}
