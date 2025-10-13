using Aro.Admin.Presentation.Api.DTOs;
using FluentValidation;

namespace Aro.Admin.Presentation.Api.Validators;

public class InitializeApplicationModelValidator : AbstractValidator<InitializeApplicationModel>
{
    public InitializeApplicationModelValidator()
    {
        RuleFor(m => m.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress();

        RuleFor(m => m.Password)
            .NotEmpty()
            .NotNull();

        RuleFor(m => m.DisplayName)
            .NotEmpty()
            .NotNull();
    }
}
