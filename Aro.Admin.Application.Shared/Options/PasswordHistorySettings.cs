namespace Aro.Admin.Application.Shared.Options;

public record PasswordHistorySettings
{
    public required bool Enabled { get; init; }
    public required int HistoryCount { get; init; }
    public int? ExpireAfterDays { get; init; }
}
