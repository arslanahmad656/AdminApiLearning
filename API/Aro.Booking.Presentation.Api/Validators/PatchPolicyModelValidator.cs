using Aro.Booking.Presentation.Api.DTOs;
using FluentValidation;

namespace Aro.Booking.Presentation.Api.Validators;

public class PatchPolicyModelValidator : AbstractValidator<PatchPolicyModel>
{
    public PatchPolicyModelValidator()
    {
        RuleFor(m => m.Title)
            .MaximumLength(50);

        RuleFor(m => m.Description)
            .MaximumLength(250);
    }
}

