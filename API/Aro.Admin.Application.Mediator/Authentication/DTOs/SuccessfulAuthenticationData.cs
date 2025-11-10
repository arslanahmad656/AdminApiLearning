namespace Aro.Admin.Application.Mediator.Authentication.DTOs;

public record SuccessfulAuthenticationData(Guid UserId, string Email, Guid RefreshTokenId, DateTime AccessTokenExpiry, DateTime RefreshTokenExpiry, string AccessTokenIdentifier);

