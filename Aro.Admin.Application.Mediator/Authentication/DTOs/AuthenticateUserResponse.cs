namespace Aro.Admin.Application.Mediator.Authentication.DTOs;

public record AuthenticateUserResponse(string Token, DateTime Expiry);
