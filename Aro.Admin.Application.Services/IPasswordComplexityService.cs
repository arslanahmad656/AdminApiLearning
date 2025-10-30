using Aro.Admin.Application.Services.DTOs.ServiceResponses.PasswordComplexity;

namespace Aro.Admin.Application.Services;

public interface IPasswordComplexityService
{
    Task<PasswordComplexityValidationResult> Validate(string password);
}
