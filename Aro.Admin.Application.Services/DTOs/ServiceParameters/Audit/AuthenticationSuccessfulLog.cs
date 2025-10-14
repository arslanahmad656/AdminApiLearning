namespace Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;

public record AuthenticationSuccessfulLog(Guid UserId, string Email, Guid RefreshTokenId, DateTime AccessTokenExpiry, DateTime RefreshTokenExpiry, string AccessTokenIdentifier);


