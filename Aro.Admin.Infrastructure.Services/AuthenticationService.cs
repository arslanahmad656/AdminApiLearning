using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceResponses;
using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Domain.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Services;

public partial class AuthenticationService(IHasher haser, IUserService userService, IAccessTokenService accessTokenService, IRefreshTokenService refreshTokenService, IRepositoryManager repositoryManager, IUniqueIdGenerator idGenerator, ErrorCodes errorCodes) : IAuthenticationService
{
    private readonly IRefreshTokenRepository refreshTokenRepo = repositoryManager.RefreshTokenRepository;

    public async Task<CompositeToken> Authenticate(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await userService.GetUserByEmail(email, false, true, cancellationToken).ConfigureAwait(false);
        var isPasswordCorrect = haser.Verify(password, user.PasswordHash);

        if (!isPasswordCorrect)
        {
            throw new AroException(errorCodes.INVALID_PASSWORD, $"Invalid password for user {email}.");
        }

        var accessToken = await accessTokenService.GenerateAccessToken(user.Id, cancellationToken).ConfigureAwait(false);

        var refreshToken = await refreshTokenService.GenerateRefreshToken(cancellationToken).ConfigureAwait(false);

        var refreshEntity = new Domain.Entities.RefreshToken
        {
            Id = idGenerator.Generate(),
            UserId = user.Id,
            TokenHash = haser.Hash(refreshToken.Token),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = refreshToken.ExpiresAt,
        };

        await refreshTokenRepo.Create(refreshEntity, cancellationToken).ConfigureAwait(false);
        await repositoryManager.SaveChanges(cancellationToken).ConfigureAwait(false);

        var response = new CompositeToken(accessToken.Token, refreshToken.Token, accessToken.Expiry);

        return response;
    }

    public async Task<bool> Logout(Guid userId, string refreshToken, CancellationToken cancellationToken = default)
    {
        var result = await refreshTokenService.Revoke(userId, refreshToken, cancellationToken).ConfigureAwait(false);

        return result;
    }

    public async Task LogoutAll(Guid userId, CancellationToken cancellationToken = default)
    {
        await refreshTokenService.RevokeAll(userId, cancellationToken).ConfigureAwait(false);
    }
}
