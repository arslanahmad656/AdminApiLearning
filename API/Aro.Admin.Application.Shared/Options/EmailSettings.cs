namespace Aro.Admin.Application.Shared.Options;

public record EmailSettings
{
    public required string Host { get; init; }
    public required int Port { get; init; }
    public required bool UseSsl { get; init; }
    public required bool UseStartTls { get; init; }
    public required string UserName { get; init; }
    public required string Password { get; init; }
    public required string FromName { get; init; }
    public required string FromEmail { get; init; }

    public EmailSettings() { }

    public EmailSettings(string host, int port, bool useSsl, bool useStartTls, string userName, string password, string fromName, string fromEmail)
        => (Host, Port, UseSsl, UseStartTls, UserName, Password, FromName, FromEmail)
            = (host, port, useSsl, useStartTls, userName, password, fromName, fromEmail);
}
