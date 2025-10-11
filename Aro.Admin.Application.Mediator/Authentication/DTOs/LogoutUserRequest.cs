namespace Aro.Admin.Application.Mediator.Authentication.DTOs;

public record LogoutUserRequest(Guid UserId, string RefreshToken);

