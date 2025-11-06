using Aro.Common.Application.Services;

namespace Aro.Admin.Application.Services.Password;

public interface IPasswordHistoryEnforcer : IService
{
    /// <summary>
    /// Validates a new password against configured password history rules.
    /// Throws a domain exception if the password is not allowed.
    /// </summary>
    Task EnsureCanUsePassword(Guid userId, string newPassword);

    /// <summary>
    /// Records the password hash into the history store 
    /// after a successful password change/reset.
    /// </summary>
    Task RecordPassword(Guid userId, string passwordHash);

    /// <summary>
    /// Cleans up old password history entries beyond the allowed limit.
    /// Called after recording a new password.
    /// </summary>
    Task TrimHistory(Guid userId);
}
