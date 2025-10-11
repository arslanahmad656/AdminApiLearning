namespace Aro.Admin.Application.Services.DTOs.ServiceResponses;

public record CompositeToken(string OldRefreshTokenHash, Guid UserId, Guid RefreshTokenId, string AccessToken, string RefreshToken, DateTime AccessTokenExpiry, DateTime RefreshTokenExpiry);
