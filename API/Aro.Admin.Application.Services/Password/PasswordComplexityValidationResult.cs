namespace Aro.Admin.Application.Services.Password;

public record PasswordComplexityValidationResult(bool Success, IList<string>? Errors);
