namespace Aro.Admin.Application.Services.DTOs.ServiceResponses;

public record AccessTokenResponse(string Token, DateTime Expiry, string TokenIdentifier);
