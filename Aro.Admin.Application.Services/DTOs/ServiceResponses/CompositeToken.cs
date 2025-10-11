namespace Aro.Admin.Application.Services.DTOs.ServiceResponses;

public record CompositeToken(string AccessToken, string RefreshToken, DateTime AccessTokenExpiry);
