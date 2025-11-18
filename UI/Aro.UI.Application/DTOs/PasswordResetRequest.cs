namespace Aro.UI.Application.DTOs;

public record PasswordResetRequest(
    string Token,
    string NewPassword
);
