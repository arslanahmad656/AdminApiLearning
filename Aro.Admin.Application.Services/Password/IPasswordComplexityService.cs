namespace Aro.Admin.Application.Services.Password;

public interface IPasswordComplexityService
{
    Task<PasswordComplexityValidationResult> Validate(string password);
}
