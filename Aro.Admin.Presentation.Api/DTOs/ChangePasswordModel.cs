namespace Aro.Admin.Presentation.Api.DTOs;

public record ChangePasswordModel(string UserEmail, string OldPassword, string NewPassword);
