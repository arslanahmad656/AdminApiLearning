namespace Aro.Admin.Application.Services.User;

public record ChangePasswordParameters(string UserEmail, string OldPassword, string NewPassword);
