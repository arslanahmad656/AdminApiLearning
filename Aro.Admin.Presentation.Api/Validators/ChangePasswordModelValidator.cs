using Aro.Admin.Presentation.Api.DTOs;
using FluentValidation;

namespace Aro.Admin.Presentation.Api.Validators;

public class ChangePasswordModelValidator : AbstractValidator<ChangePasswordModel>
{
    public ChangePasswordModelValidator()
    {
        RuleFor(m => m.UserEmail)
            .NotNull()
            .NotEmpty()
            .EmailAddress();

        RuleFor(m => m.OldPassword)
            .NotNull()
            .NotEmpty();

        RuleFor(m => m.NewPassword)
            .NotNull()
            .NotEmpty();
    }
}
