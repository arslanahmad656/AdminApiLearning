using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Services;

public partial class RefreshTokenService
{
    public void MarkRevoked(RefreshToken token, DateTime now)
    {
        token.RevokedAt = now;
        refreshTokenRepo.Update(token);
    }
}
