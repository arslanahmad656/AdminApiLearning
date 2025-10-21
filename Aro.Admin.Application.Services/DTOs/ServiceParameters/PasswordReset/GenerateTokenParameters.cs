namespace Aro.Admin.Application.Services.DTOs.ServiceParameters.PasswordReset;

public record GenerateTokenParameters(Guid userId, string requestIp, string userAgent);

