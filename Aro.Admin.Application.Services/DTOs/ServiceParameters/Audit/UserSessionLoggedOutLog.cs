namespace Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;

public record UserSessionLoggedOutLog(Guid UserId, string RefreshTokenHash, string TokenIdentifier);

