using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DTOs.ServiceResponses.PasswordComplexity;
using Aro.Admin.Application.Shared.Options;
using Easy_Password_Validator;
using Microsoft.Extensions.Options;

namespace Aro.Admin.Infrastructure.Services;

public partial class EasyPasswordValidator(IOptions<PasswordPolicyOptions> passwordPolicy) : IPasswordComplexityService
{
    private readonly PasswordValidatorService validator = CreateValidator(passwordPolicy.Value);

    public Task<PasswordComplexityValidationResult> Validate(string password)
    {
        var success = validator.TestAndScore(password);

        var result = new PasswordComplexityValidationResult(
            Success: success,
            Errors: [.. (validator.FailureMessages ?? [])]
        );

        return Task.FromResult(result);
    }
}
