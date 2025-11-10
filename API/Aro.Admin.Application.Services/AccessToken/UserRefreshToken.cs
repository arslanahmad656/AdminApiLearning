namespace Aro.Admin.Application.Services.AccessToken;

public record UserRefreshToken(string Token, DateTime ExpiresAt, Guid UserId) : RefreshToken(Token, ExpiresAt);
