namespace Aro.Admin.Application.Services.DTOs.ServiceParameters.PasswordReset;

public record GenerateTokenParameters(Guid UserId, string RequestIp, string UserAgent);

