namespace Aro.Admin.Application.Mediator.Authentication.DTOs;

public record AuthenticateUserResponse(Guid RefreshTokenId, string AccessToken, string RefreshToken, DateTime AccessTokenExpiry, DateTime RefreshTokenExpiry);
