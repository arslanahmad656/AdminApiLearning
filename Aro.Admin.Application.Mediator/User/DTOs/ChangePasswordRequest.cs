namespace Aro.Admin.Application.Mediator.User.DTOs;

public record ChangePasswordRequest(string Email, string OldPassword, string NewPassword);
