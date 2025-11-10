namespace Aro.Admin.Application.Shared.Options;

public record BootstrapUserSettings
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string DisplayName { get; init; }
    public BootstrapUserSettings() { }

    public BootstrapUserSettings(string email, string password, string displayName)
        => (Email, Password, DisplayName) = (email, password, displayName);
}
