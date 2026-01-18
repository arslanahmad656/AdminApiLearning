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
    public required Header[] CustomHeaders { get; init; }

    public EmailSettings() { }

    public EmailSettings(string host, int port, bool useSsl, bool useStartTls, string userName, string password, string fromName, string fromEmail, Header[] customHeaders)
        => (Host, Port, UseSsl, UseStartTls, UserName, Password, FromName, FromEmail, CustomHeaders)
            = (host, port, useSsl, useStartTls, userName, password, fromName, fromEmail, customHeaders);

    public record Header
    {
        public required string Key { get; init; }
        public required string Value { get; init; }
    }
}
