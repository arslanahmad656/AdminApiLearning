namespace Aro.Admin.Application.Services.AccessToken;

public record AccessTokenResponse(string Token, DateTime Expiry, string TokenIdentifier);
