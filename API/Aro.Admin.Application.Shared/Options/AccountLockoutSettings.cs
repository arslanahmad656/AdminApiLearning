namespace Aro.Admin.Application.Shared.Options;

public record AccountLockoutSettings
{
    public required bool EnableLockout { get; init; }
    public required int MaxFailedAttempts { get; init; }
    public required int LockoutDurationMinutes { get; init; }
}
