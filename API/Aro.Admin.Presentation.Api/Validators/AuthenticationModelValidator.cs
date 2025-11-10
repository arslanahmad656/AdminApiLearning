using Aro.Admin.Presentation.Api.DTOs;
using FluentValidation;

namespace Aro.Admin.Presentation.Api.Validators;

public class AuthenticationModelValidator : AbstractValidator<AuthenticationModel>
{
    public AuthenticationModelValidator()
    {
        RuleFor(m => m.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress();

        RuleFor(m => m.Password)
            .NotEmpty()
            .NotNull();
    }
}
