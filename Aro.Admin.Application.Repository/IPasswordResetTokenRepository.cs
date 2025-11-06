using Aro.Admin.Domain.Entities;

namespace Aro.Admin.Application.Repository;

public interface IPasswordResetTokenRepository
{
    Task Create(PasswordResetToken token, CancellationToken cancellationToken = default);

    void Update(PasswordResetToken token);

    IQueryable<PasswordResetToken> GetActiveTokensByUserId(Guid userId);

    IQueryable<PasswordResetToken> GetActiveTokenById(Guid tokenId);

    IQueryable<PasswordResetToken> GetActiveTokenByTokenHash(string tokenHash);

    IQueryable<PasswordResetToken> GetActiveTokensByUserAndTokenHash(Guid userId, string tokenHash);

    IQueryable<PasswordResetToken> GetExpiredTokens();

    Task DeleteExpiredTokens(CancellationToken cancellationToken = default);
}
