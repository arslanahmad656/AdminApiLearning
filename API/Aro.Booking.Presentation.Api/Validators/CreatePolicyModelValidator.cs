using Aro.Booking.Presentation.Api.DTOs;
using FluentValidation;

namespace Aro.Booking.Presentation.Api.Validators;

public class CreatePolicyModelValidator : AbstractValidator<CreatePolicyModel>
{
    public CreatePolicyModelValidator()
    {
        RuleFor(m => m.PropertyId)
            .NotEmpty()
            .NotNull();

        RuleFor(m => m.Title)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(m => m.Description)
            .NotEmpty()
            .MaximumLength(250);
    }
}

