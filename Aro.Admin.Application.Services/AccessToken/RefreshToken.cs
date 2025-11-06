namespace Aro.Admin.Application.Services.AccessToken;

public record RefreshToken(string Token, DateTime ExpiresAt);

