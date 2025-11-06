namespace Aro.Admin.Application.Services.Password;

public record GenerateTokenParameters(Guid UserId, string RequestIp, string UserAgent);

