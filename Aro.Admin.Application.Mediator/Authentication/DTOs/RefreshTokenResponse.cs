namespace Aro.Admin.Application.Mediator.Authentication.DTOs;

public record RefreshTokenResponse(Guid RefreshTokenId, string AccessToken, string RefreshToken, DateTime AccessTokenExpiry, DateTime RefreshTokenExpiry);

