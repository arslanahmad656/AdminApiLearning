using Aro.Booking.Presentation.Api.DTOs;
using FluentValidation;

namespace Aro.Booking.Presentation.Api.Validators;

public class CreateAmenityModelValidator : AbstractValidator<CreateAmenityModel>
{
    public CreateAmenityModelValidator()
    {
        RuleFor(m => m.Name)
            .NotEmpty()
            .Length(2, 50);
    }
}
