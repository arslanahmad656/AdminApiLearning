namespace Aro.Admin.Application.Services.DTOs.ServiceResponses;

public record UserRefreshToken(Guid UserId, string Token, DateTime ExpiresAt) : RefreshToken(Token, ExpiresAt);
