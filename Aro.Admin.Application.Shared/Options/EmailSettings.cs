namespace Aro.Admin.Application.Shared.Options;

public record EmailSettings(string Host, int Port, bool UseSsl, bool UseStartTls, string UserName, string Password, string FromName, string FromEmail);
