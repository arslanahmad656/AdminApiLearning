using Aro.Admin.Presentation.Api.DTOs;
using FluentValidation;

namespace Aro.Admin.Presentation.Api.Validators;

public class GeneratePasswordResetTokenModelValidator : AbstractValidator<GeneratePasswordResetTokenModel>
{
    public GeneratePasswordResetTokenModelValidator()
    {
        RuleFor(m => m.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress();
    }
}
