using Aro.Admin.Application.Shared.Options;
using Easy_Password_Validator;
using Easy_Password_Validator.Models;

namespace Aro.Admin.Infrastructure.Services;

public partial class EasyPasswordValidator
{
    private static PasswordValidatorService CreateValidator(PasswordPolicyOptions passwordPolicy)
    {
        var passwordRequirements = new PasswordRequirements
        {
            UseLength = false,
            UseUnique = false,
            UseDigit = false,
            UseLowercase = false,
            UseUppercase = false,
            UsePunctuation = false,
            MinScore = -1,
            UseEntropy = false,
            UsePattern = false,
            UseRepeat = false,
            UseBadList = false,
            ExitOnFailure = false,
        };

        if (passwordPolicy.MinLength > 0)
        {
            passwordRequirements.UseLength = true;
            passwordRequirements.MinLength = passwordPolicy.MinLength;
        }

        if (passwordPolicy.RequireUpperCase)
        {
            passwordRequirements.UseUppercase = true;
            passwordRequirements.RequireUppercase = true;
        }

        if (passwordPolicy.RequireLowerCase)
        {
            passwordRequirements.UseLowercase = true;
            passwordRequirements.RequireLowercase = true;
        }

        if (passwordPolicy.RequirePunctuation)
        {
            passwordRequirements.UsePunctuation = true;
            passwordRequirements.RequirePunctuation = true;
        }

        if (passwordPolicy.RequireDigit)
        {
            passwordRequirements.UseDigit = true;
            passwordRequirements.RequireDigit = true;
        }

        if (passwordPolicy.MinUniqueCharacters > 0)
        {
            passwordRequirements.UseUnique = true;
            passwordRequirements.MinUniqueCharacters = passwordPolicy.MinUniqueCharacters;
        }

        return new PasswordValidatorService(passwordRequirements);
    }
}
