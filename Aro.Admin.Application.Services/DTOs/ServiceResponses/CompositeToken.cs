namespace Aro.Admin.Application.Services.DTOs.ServiceResponses;

public record CompositeToken(Guid RefreshTokenId, string AccessToken, string RefreshToken, DateTime AccessTokenExpiry, DateTime RefreshTokenExpiry);
