using Aro.Common.Application.Shared;

namespace Aro.Admin.Application.Services.AccountLockout;

public interface IAccountLockoutService : IService
{
    Task<bool> IsLockedOut(Guid userId, CancellationToken cancellationToken = default);

    Task<DateTime?> GetLockoutEnd(Guid userId, CancellationToken cancellationToken = default);

    Task RecordFailedAttempt(Guid userId, CancellationToken cancellationToken = default);

    Task ResetFailedAttempts(Guid userId, CancellationToken cancellationToken = default);

    Task<int> GetFailedAttempts(Guid userId, CancellationToken cancellationToken = default);
}
