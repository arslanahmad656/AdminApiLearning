using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.PasswordReset;
using Aro.Admin.Application.Shared.Options;
using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Domain.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace Aro.Admin.Infrastructure.Services;

public class PasswordResetTokenService(
    IOptionsSnapshot<PasswordResetSettings> passwordResetOptions,
    IRepositoryManager repositoryManager,
    IHasher hasher,
    IUniqueIdGenerator idGenerator,
    IRandomValueGenerator randomValueGenerator,
    ErrorCodes errorCodes,
    ILogManager<PasswordResetTokenService> logger) : IPasswordResetTokenService
{
    private readonly PasswordResetSettings passwordResetSettings = passwordResetOptions.Value;
    private readonly IPasswordResetTokenRepository passwordResetTokenRepo = repositoryManager.PasswordResetTokenRepository;

    public async Task<string> GenerateToken(GenerateTokenParameters parameters, CancellationToken ct = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(GenerateToken));
        logger.LogDebug("Generating password reset token for user: {UserId}, requestIP: {RequestIP}", parameters.userId, parameters.requestIp);

        try
        {
            var now = DateTime.UtcNow;
            logger.LogDebug("Using timestamp: {Timestamp}", now);

            // Generate random token using IRandomValueGenerator
            var rawToken = randomValueGenerator.GenerateString(passwordResetSettings.TokenLength);
            logger.LogDebug("Generated random token using IRandomValueGenerator, length: {Length}", passwordResetSettings.TokenLength);

            var tokenHash = hasher.Hash(rawToken);
            var expiry = now.AddMinutes(passwordResetSettings.TokenExpiryMinutes);
            logger.LogDebug("Created password reset token with expiry: {Expiry}", expiry);

            // Create token entity
            var tokenEntity = new PasswordResetToken
            {
                Id = idGenerator.Generate(),
                UserId = parameters.userId,
                TokenHash = tokenHash,
                CreatedAt = now,
                Expiry = expiry,
                IsUsed = false,
                RequestIP = parameters.requestIp,
                UserAgent = parameters.userAgent
            };
            logger.LogDebug("Created password reset token entity, tokenId: {TokenId}, userId: {UserId}", tokenEntity.Id, parameters.userId);

            // Save to database
            await passwordResetTokenRepo.Create(tokenEntity, ct).ConfigureAwait(false);
            await repositoryManager.SaveChanges(ct).ConfigureAwait(false);
            logger.LogDebug("Password reset token saved to database, tokenId: {TokenId}", tokenEntity.Id);

            logger.LogInfo("Password reset token generated successfully for user: {UserId}, tokenId: {TokenId}, expiry: {Expiry}", 
                parameters.userId, tokenEntity.Id, expiry);

            logger.LogDebug("Completed {MethodName}", nameof(GenerateToken));
            return rawToken;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while generating password reset token for user: {UserId}", parameters.userId);
            throw new AroException(errorCodes.UNKNOWN_ERROR, "Failed to generate password reset token", ex);
        }
    }

    public async Task<bool> ValidateToken(string rawToken, CancellationToken ct = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(ValidateToken));
        logger.LogDebug("Validating password reset token");

        try
        {
            var tokenHash = hasher.Hash(rawToken);
            logger.LogDebug("Generated token hash for validation");

            var query = passwordResetTokenRepo.GetActiveTokenByTokenHash(tokenHash);
            var tokenEntity = await query
                .FirstOrDefaultAsync(ct)
                .ConfigureAwait(false);

            if (tokenEntity == null)
            {
                logger.LogWarn("Password reset token not found or invalid");
                logger.LogDebug("Completed {MethodName}", nameof(ValidateToken));
                throw new AroTokenValidationException(errorCodes.PASSWORD_RESET_TOKEN_NOT_FOUND, "Password reset token not found or invalid");
            }

            logger.LogDebug("Found password reset token entity, tokenId: {TokenId}, userId: {UserId}, isUsed: {IsUsed}, expiry: {Expiry}", 
                tokenEntity.Id, tokenEntity.UserId, tokenEntity.IsUsed, tokenEntity.Expiry);

            // Check if token is already used
            if (tokenEntity.IsUsed)
            {
                logger.LogWarn("Password reset token already used, tokenId: {TokenId}, userId: {UserId}", 
                    tokenEntity.Id, tokenEntity.UserId);
                logger.LogDebug("Completed {MethodName}", nameof(ValidateToken));
                throw new AroTokenValidationException(errorCodes.PASSWORD_RESET_TOKEN_ALREADY_USED, "Password reset token has already been used");
            }

            // Check if token is expired
            if (tokenEntity.Expiry <= DateTime.UtcNow)
            {
                logger.LogWarn("Password reset token expired, tokenId: {TokenId}, userId: {UserId}, expiry: {Expiry}", 
                    tokenEntity.Id, tokenEntity.UserId, tokenEntity.Expiry);
                logger.LogDebug("Completed {MethodName}", nameof(ValidateToken));
                throw new AroTokenValidationException(errorCodes.PASSWORD_RESET_TOKEN_EXPIRED, "Password reset token has expired");
            }

            logger.LogInfo("Password reset token validation successful, tokenId: {TokenId}, userId: {UserId}", 
                tokenEntity.Id, tokenEntity.UserId);

            logger.LogDebug("Completed {MethodName}", nameof(ValidateToken));
            return true;
        }
        catch (AroTokenValidationException)
        {
            // Re-throw AroTokenValidationException as-is
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred during password reset token validation");
            logger.LogDebug("Completed {MethodName}", nameof(ValidateToken));
            throw new AroTokenValidationException(errorCodes.PASSWORD_RESET_TOKEN_INVALID, "An error occurred during token validation", ex);
        }
    }

    public async Task MarkTokenUsed(string rawToken, CancellationToken ct = default)
    {
        logger.LogDebug("Starting {MethodName}", nameof(MarkTokenUsed));
        logger.LogDebug("Marking password reset token as used");

        try
        {
            var tokenHash = hasher.Hash(rawToken);
            logger.LogDebug("Generated token hash for marking as used");

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
            await repositoryManager.SaveChanges(ct).ConfigureAwait(false);

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
            await repositoryManager.SaveChanges(ct).ConfigureAwait(false);
            
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
