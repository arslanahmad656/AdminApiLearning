using Aro.Admin.Application.Services.AccountLockout;
using Aro.Admin.Application.Shared.Options;
using Aro.Common.Application.Repository;
using Aro.Common.Application.Services.LogManager;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Aro.Admin.Infrastructure.Services;

public class AccountLockoutService(
    Common.Application.Repository.IRepositoryManager commonRepository,
    IUnitOfWork unitOfWork,
    IOptions<AccountLockoutSettings> settings,
    ILogManager<AccountLockoutService> logger) : IAccountLockoutService
{
    private readonly AccountLockoutSettings _settings = settings.Value;

    public async Task<bool> IsLockedOut(Guid userId, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Checking lockout status for user: {UserId}", userId);

        var user = await commonRepository.UserRepository.GetById(userId)
            .SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);

        if (user == null)
        {
            logger.LogWarn("User not found when checking lockout: {UserId}", userId);
            return false;
        }

        if (user.LockoutEnd == null)
        {
            return false;
        }

        if (user.LockoutEnd > DateTime.UtcNow)
        {
            logger.LogWarn("User {UserId} is locked out until {LockoutEnd}", userId, user.LockoutEnd);
            return true;
        }

        logger.LogInfo("Lockout expired for user {UserId}, resetting failed attempts", userId);
        await ResetFailedAttempts(userId, cancellationToken).ConfigureAwait(false);
        return false;
    }

    public async Task<DateTime?> GetLockoutEnd(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await commonRepository.UserRepository.GetById(userId)
            .SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);

        return user?.LockoutEnd;
    }

    public async Task<int> GetFailedAttempts(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await commonRepository.UserRepository.GetById(userId)
            .SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);

        return user?.FailedLoginAttempts ?? 0;
    }

    public async Task RecordFailedAttempt(Guid userId, CancellationToken cancellationToken = default)
    {
        if (!_settings.EnableLockout)
        {
            logger.LogDebug("Account lockout is disabled, skipping failed attempt recording");
            return;
        }

        var user = await commonRepository.UserRepository.GetById(userId)
            .SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);

        if (user == null)
        {
            logger.LogWarn("User not found when recording failed attempt: {UserId}", userId);
            return;
        }

        user.FailedLoginAttempts = (user.FailedLoginAttempts ?? 0) + 1;
        logger.LogInfo("Failed login attempt {AttemptCount} for user {UserId}", user.FailedLoginAttempts, userId);

        if (user.FailedLoginAttempts.Value >= _settings.MaxFailedAttempts)
        {
            user.LockoutEnd = DateTime.UtcNow.AddMinutes(_settings.LockoutDurationMinutes);
            logger.LogWarn("User {UserId} locked out until {LockoutEnd} after {Attempts} failed attempts",
                userId, user.LockoutEnd, user.FailedLoginAttempts);
        }

        commonRepository.UserRepository.Update(user);
        await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);
    }

    public async Task ResetFailedAttempts(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await commonRepository.UserRepository.GetById(userId)
            .SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);

        if (user == null)
        {
            logger.LogWarn("User not found when resetting failed attempts: {UserId}", userId);
            return;
        }

        if ((user.FailedLoginAttempts ?? 0) == 0 && user.LockoutEnd == null)
        {
            return;
        }

        user.FailedLoginAttempts = null;
        user.LockoutEnd = null;

        commonRepository.UserRepository.Update(user);
        await unitOfWork.SaveChanges(cancellationToken).ConfigureAwait(false);

        logger.LogInfo("Reset failed login attempts for user {UserId}", userId);
    }
}
