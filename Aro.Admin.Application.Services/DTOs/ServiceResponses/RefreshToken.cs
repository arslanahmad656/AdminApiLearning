namespace Aro.Admin.Application.Services.DTOs.ServiceResponses;

public record RefreshToken(string Token, DateTime ExpiresAt);
