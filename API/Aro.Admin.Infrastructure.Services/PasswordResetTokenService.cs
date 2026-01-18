using Aro.Admin.Application.Repository;
using Aro.Admin.Application.Services.Password;
using Aro.Admin.Application.Shared.Options;
using Aro.Admin.Domain.Entities;
using Aro.Common.Application.Repository;
using Aro.Common.Application.Services.Hasher;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Application.Services.RandomValueGenerator;
using Aro.Common.Application.Services.RequestInterpretor;
using Aro.Common.Application.Services.Serializer;
using Aro.Common.Application.Services.UniqueIdGenerator;
using Aro.Common.Domain.Shared;
using Aro.Common.Domain.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Aro.Admin.Infrastructure.Services;

public partial class PasswordResetTokenService(
    IOptionsSnapshot<PasswordResetSettings> passwordResetOptions,
    Application.Repository.IRepositoryManager repositoryManager,
    IUnitOfWork unitOfWork,
    IHasher hasher,
    IUniqueIdGenerator idGenerator,
    IRandomValueGenerator randomValueGenerator,
    ISerializer serializer,
    ErrorCodes errorCodes,
    IRequestInterpretorService requestInterpretorService,
    ILogManager<PasswordResetTokenService> logger) : IPasswordResetTokenService
{
    private readonly PasswordResetSettings passwordResetSettings = passwordResetOptions.Value;
    private readonly IPasswordResetTokenRepository passwordResetTokenRepo = repositoryManager.PasswordResetTokenRepository;


    public async Task<string> GenerateToken(GenerateTokenParameters parameters, CancellationToken ct = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(GenerateToken));
        logger.LogDebug("Generating password reset token for user: {UserId}, requestIP: {RequestIP}", parameters.UserId, parameters.RequestIp);

        try
        {
            var now = DateTime.UtcNow;
            logger.LogDebug("Using timestamp: {Timestamp}", now);

            // Generate structured token
            var rawToken = GenerateStructuredToken(parameters, now);

            logger.LogDebug("Generated structured token with user context, length: {Length}", rawToken.Length);

            var tokenHash = hasher.Hash(rawToken);
            var expiry = now.AddMinutes(passwordResetSettings.TokenExpiryMinutes);
            logger.LogDebug("Created password reset token with expiry: {Expiry}", expiry);

            // Create token entity
            var tokenEntity = new PasswordResetToken
            {
                Id = idGenerator.Generate(),
                UserId = parameters.UserId,
                TokenHash = tokenHash,
                CreatedAt = now,
                Expiry = expiry,
                IsUsed = false,
                RequestIP = parameters.RequestIp,
                UserAgent = parameters.UserAgent
            };
            logger.LogDebug("Created password reset token entity, tokenId: {TokenId}, userId: {UserId}", tokenEntity.Id, parameters.UserId);

            // Save to database
            await passwordResetTokenRepo.Create(tokenEntity, ct).ConfigureAwait(false);
            await unitOfWork.SaveChanges(ct).ConfigureAwait(false);
            logger.LogDebug("Password reset token saved to database, tokenId: {TokenId}", tokenEntity.Id);

            logger.LogInfo("Password reset token generated successfully for user: {UserId}, tokenId: {TokenId}, expiry: {Expiry}",
                parameters.UserId, tokenEntity.Id, expiry);

            logger.LogDebug("Completed {MethodName}", nameof(GenerateToken));
            return tokenHash;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while generating password reset token for user: {UserId}", parameters.UserId);
            throw new AroException(errorCodes.UNKNOWN_ERROR, "Failed to generate password reset token", ex);
        }
    }

    public async Task<ValidateTokenResult> ValidateToken(string tokenHash, CancellationToken ct = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(ValidateToken));
        logger.LogDebug("Validating password reset token");

        try
        {
            logger.LogDebug("Using provided token hash for validation");

            var query = passwordResetTokenRepo.GetActiveTokenByTokenHash(tokenHash);
            var tokenEntity = await query
                .FirstOrDefaultAsync(ct)
                .ConfigureAwait(false);

            if (tokenEntity == null)
            {
                logger.LogWarn("Password reset token not found or invalid");
                logger.LogDebug("Completed {MethodName}", nameof(ValidateToken));
                return new ValidateTokenResult(false, null, null, null, null);
            }

            logger.LogDebug("Found password reset token entity, tokenId: {TokenId}, userId: {UserId}, isUsed: {IsUsed}, expiry: {Expiry}",
                tokenEntity.Id, tokenEntity.UserId, tokenEntity.IsUsed, tokenEntity.Expiry);

            // Since we're working with hashes, we can't extract context from the token
            // Use data from the token entity instead
            var userId = tokenEntity.UserId;
            var ipAddress = tokenEntity.RequestIP;
            var userAgent = tokenEntity.UserAgent ?? string.Empty;
            var timestamp = tokenEntity.CreatedAt;

            logger.LogDebug("Using token entity data - UserId: {UserId}, IP: {IpAddress}, UserAgent: {UserAgent}, Timestamp: {Timestamp}",
                userId, ipAddress, userAgent, timestamp);

            // Check if token is already used
            if (tokenEntity.IsUsed)
            {
                logger.LogWarn("Password reset token already used, tokenId: {TokenId}, userId: {UserId}",
                    tokenEntity.Id, tokenEntity.UserId);
                logger.LogDebug("Completed {MethodName}", nameof(ValidateToken));
                return new ValidateTokenResult(false, userId, ipAddress, userAgent, timestamp);
            }

            // Check if token is expired
            if (tokenEntity.Expiry <= DateTime.UtcNow)
            {
                logger.LogWarn("Password reset token expired, tokenId: {TokenId}, userId: {UserId}, expiry: {Expiry}",
                    tokenEntity.Id, tokenEntity.UserId, tokenEntity.Expiry);
                logger.LogDebug("Completed {MethodName}", nameof(ValidateToken));
                return new ValidateTokenResult(false, userId, ipAddress, userAgent, timestamp);
            }

            if (passwordResetSettings.UseStrictSecurityChecks)
            {
                logger.LogDebug("Validating the strict security measurements.");
                var currentIpAddress = requestInterpretorService.RetrieveIpAddress(true);
                var currentUserAgent = requestInterpretorService.GetUserAgent();

                if (!string.Equals(currentIpAddress, ipAddress) || !string.Equals(currentUserAgent, userAgent))
                {
                    return new ValidateTokenResult(false, userId, ipAddress, userAgent, timestamp);
                }

                logger.LogDebug($"Strict security measurements validated.");
            }
            else
            {
                logger.LogWarn("Strict security checks are disabled, skipping the strict security checks.");
            }

            logger.LogInfo("Password reset token validation successful, tokenId: {TokenId}, userId: {UserId}",
                tokenEntity.Id, tokenEntity.UserId);

            logger.LogDebug("Completed {MethodName}", nameof(ValidateToken));
            return new ValidateTokenResult(true, userId, ipAddress, userAgent, timestamp);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during password reset token validation");
            logger.LogDebug("Completed {MethodName}", nameof(ValidateToken));
            return new ValidateTokenResult(false, null, null, null, null);
        }
    }

    public async Task MarkTokenUsed(string tokenHash, CancellationToken ct = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(MarkTokenUsed));
        logger.LogDebug("Marking password reset token as used");

        try
        {
            logger.LogDebug("Using provided token hash for marking as used");

            var query = passwordResetTokenRepo.GetActiveTokenByTokenHash(tokenHash);
            var tokenEntity = await query
                .FirstOrDefaultAsync(ct)
                .ConfigureAwait(false);

            if (tokenEntity == null)
            {
                logger.LogWarn("Password reset token not found when attempting to mark as used");
                logger.LogDebug("Completed {MethodName}", nameof(MarkTokenUsed));
                return;
            }

            logger.LogDebug("Found password reset token entity to mark as used, tokenId: {TokenId}, userId: {UserId}",
                tokenEntity.Id, tokenEntity.UserId);

            // Mark token as used
            tokenEntity.IsUsed = true;
            passwordResetTokenRepo.Update(tokenEntity);
            await unitOfWork.SaveChanges(ct).ConfigureAwait(false);

            logger.LogInfo("Password reset token marked as used successfully, tokenId: {TokenId}, userId: {UserId}",
                tokenEntity.Id, tokenEntity.UserId);

            logger.LogDebug("Completed {MethodName}", nameof(MarkTokenUsed));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while marking password reset token as used");
            throw new AroException(errorCodes.UNKNOWN_ERROR, "Failed to mark password reset token as used", ex);
        }
    }

    public async Task DeleteExpiredTokens(CancellationToken ct = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(DeleteExpiredTokens));
        logger.LogDebug("Deleting expired password reset tokens");

        try
        {
            logger.LogDebug("Calling repository DeleteExpiredTokens method");
            await passwordResetTokenRepo.DeleteExpiredTokens(ct).ConfigureAwait(false);
            await unitOfWork.SaveChanges(ct).ConfigureAwait(false);

            logger.LogInfo("Expired password reset tokens cleanup completed");

            logger.LogDebug("Completed {MethodName}", nameof(DeleteExpiredTokens));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while deleting expired password reset tokens");
            throw new AroException(errorCodes.UNKNOWN_ERROR, "Failed to delete expired password reset tokens", ex);
        }
    }
}
