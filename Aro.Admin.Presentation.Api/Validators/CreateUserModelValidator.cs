using Aro.Admin.Presentation.Api.DTOs;
using FluentValidation;

namespace Aro.Admin.Presentation.Api.Validators;

public class CreateUserModelValidator : AbstractValidator<CreateUserModel>
{
    public CreateUserModelValidator()
    {
        RuleFor(m => m.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress();

        RuleFor(m => m.Password)
            .NotNull()
            .NotEmpty();

        RuleFor(m => m.DisplayName)
            .NotNull()
            .NotEmpty();

        RuleFor(m => m.AssignedRoles)
            .NotNull()
            .NotEmpty();

        RuleForEach(m => m.AssignedRoles)
            .NotEmpty()
            .NotEmpty();
    }
}
