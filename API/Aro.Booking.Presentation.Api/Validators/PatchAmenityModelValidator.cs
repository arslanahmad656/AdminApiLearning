using Aro.Booking.Presentation.Api.DTOs;
using FluentValidation;

namespace Aro.Booking.Presentation.Api.Validators;

public class PatchAmenityModelValidator : AbstractValidator<PatchAmenityModel>
{
    public PatchAmenityModelValidator()
    {
        RuleFor(m => m.Name)
            .NotEmpty()
            .Length(2, 50);
    }
}
