using Aro.Admin.Presentation.Api.DTOs;
using FluentValidation;

namespace Aro.Admin.Presentation.Api.Validators;

public class ResetPasswordModelValidator : AbstractValidator<ResetPasswordModel>
{
    public ResetPasswordModelValidator()
    {
        RuleFor(m => m.Token)
            .NotNull()
            .NotEmpty();

        RuleFor(m => m.NewPassword)
            .NotNull()
            .NotEmpty();
    }
}
