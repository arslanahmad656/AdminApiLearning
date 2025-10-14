namespace Aro.Admin.Application.Services.DTOs.ServiceResponses;

public record UserRefreshToken(string Token, DateTime ExpiresAt, Guid UserId) : RefreshToken(Token, ExpiresAt);
