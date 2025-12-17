using Aro.Common.Application.Shared;

namespace Aro.Admin.Application.Services.AccountLockout;

public interface IAccountLockoutService : IService
{
    Task<bool> IsLockedOutAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<DateTime?> GetLockoutEndAsync(Guid userId, CancellationToken cancellationToken = default);

    Task RecordFailedAttemptAsync(Guid userId, CancellationToken cancellationToken = default);

    Task ResetFailedAttemptsAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<int> GetFailedAttemptsAsync(Guid userId, CancellationToken cancellationToken = default);
}
