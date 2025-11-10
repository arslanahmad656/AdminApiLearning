namespace Aro.Admin.Application.Services.Authentication;

public record CompositeToken(string OldRefreshTokenHash, Guid UserId, Guid RefreshTokenId, string AccessToken, string RefreshToken, DateTime AccessTokenExpiry, DateTime RefreshTokenExpiry, string AccessTokenIdentifier);


