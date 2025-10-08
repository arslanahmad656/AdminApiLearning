namespace Aro.Admin.Application.Mediator.Authentication.DTOs;

public record SuccessfulAuthenticationData(string Email, string Token, DateTime Expiry);
