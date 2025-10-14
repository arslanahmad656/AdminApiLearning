namespace Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;

public record TokenRefreshedLog(Guid UserId, string OldRefreshTokenHash, string NewRefreshTokenHash, DateTime NewAccessTokenExpiry, DateTime NewRefreshTokenExpiry);

