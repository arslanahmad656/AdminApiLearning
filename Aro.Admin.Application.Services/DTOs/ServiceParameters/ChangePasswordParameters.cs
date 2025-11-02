namespace Aro.Admin.Application.Services.DTOs.ServiceParameters;

public record ChangePasswordParameters(string UserEmail, string OldPassword, string NewPassword);
