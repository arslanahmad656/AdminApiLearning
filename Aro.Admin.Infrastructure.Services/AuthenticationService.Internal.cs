using Aro.Admin.Application.Services.Authentication;
using Aro.Admin.Domain.Shared.Exceptions;
using System.Threading;

namespace Aro.Admin.Infrastructure.Services;

public partial class AuthenticationService
{
    private async Task<CompositeToken> AuthenticateInternal(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await userService.GetUserByEmail(email, false, true, cancellationToken).ConfigureAwait(false);
        logger.LogDebug("Retrieved user for authentication, userId: {UserId}, email: {Email}", user.Id, email);

        var isPasswordCorrect = haser.Verify(password, user.PasswordHash);
        logger.LogDebug("Password verification completed for email: {Email}, result: {IsValid}", email, isPasswordCorrect);

        if (!isPasswordCorrect)
        {
            logger.LogWarn("Authentication failed for email: {Email} - invalid password", email);
            throw new AroException(errorCodes.INVALID_PASSWORD, $"Invalid password for user {email}.");
        }

        logger.LogInfo("Generating access token for user: {UserId}", user.Id);
        var accessToken = await accessTokenService.GenerateAccessToken(user.Id, cancellationToken).ConfigureAwait(false);
        logger.LogDebug("Access token generated for user: {UserId}, expires: {Expiry}", user.Id, accessToken.Expiry);

        logger.LogInfo("Generating refresh token for user: {UserId}", user.Id);
        var refreshToken = await refreshTokenService.GenerateRefreshToken(cancellationToken).ConfigureAwait(false);
        logger.LogDebug("Refresh token generated for user: {UserId}, expires: {Expiry}", user.Id, refreshToken.ExpiresAt);

        var refreshEntity = new Domain.Entities.RefreshToken
        {
            Id = idGenerator.Generate(),
            UserId = user.Id,
            TokenHash = haser.Hash(refreshToken.Token),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = refreshToken.ExpiresAt,
        };
        logger.LogDebug("Created refresh token entity for user: {UserId}, tokenId: {TokenId}", user.Id, refreshEntity.Id);

        await refreshTokenRepo.Create(refreshEntity, cancellationToken).ConfigureAwait(false);
        await repositoryManager.SaveChanges(cancellationToken).ConfigureAwait(false);
        logger.LogDebug("Refresh token entity saved to database for user: {UserId}", user.Id);

        var response = new CompositeToken(
            string.Empty,
            user.Id,
            refreshEntity.Id,
            accessToken.Token,
            refreshToken.Token,
            accessToken.Expiry,
            refreshToken.ExpiresAt,
            accessToken.TokenIdentifier
        );

        logger.LogInfo("Authentication successful for email: {Email}, userId: {UserId}", email, user.Id);

        logger.LogDebug("Completed {MethodName}", nameof(Authenticate));
        return response;
    }
}
