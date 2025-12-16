using Aro.UI.Application.DTOs.Room;
using FluentValidation;

namespace Aro.Admin.Presentation.UI.Validators;

public class AmenityModelFluentValidator : AbstractValidator<Amenity>
{
    public AmenityModelFluentValidator()
    {
        RuleFor(x => x.Name)
            .Length(5, 100);
    }
}

