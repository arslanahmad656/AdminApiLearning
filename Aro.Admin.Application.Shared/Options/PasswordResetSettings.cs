namespace Aro.Admin.Application.Shared.Options;

public record PasswordResetSettings
{
    public int TokenExpiryMinutes { get; init; }
    public int TokenHashLength { get; init; }
}
