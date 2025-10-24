namespace Aro.Admin.Application.Shared.Options;

public record PasswordResetSettings
{
    public int TokenExpiryMinutes { get; init; }
    public int TokenLength { get; init; }
    public bool EnforceSameIPandUserAgentForTokenUsage { get; set; }
    public required string FrontendResetPasswordUrl { get; init; }
}
