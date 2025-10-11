namespace Aro.Admin.Infrastructure.Services;

public partial class RefreshTokenService
{
    public void MarkRevoked(Domain.Entities.RefreshToken token, DateTime now)
    {
        token.RevokedAt = now;
        refreshTokenRepo.Update(token);
    }
}
