namespace Aro.UI.Application.DTOs;

public record AuthenticationRequest(
    string Email,
    string Password
);
