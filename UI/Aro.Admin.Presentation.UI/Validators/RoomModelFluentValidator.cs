using Aro.UI.Application.DTOs.Room;
using FluentValidation;

namespace Aro.Admin.Presentation.UI.Validators;

public class RoomModelFluentValidator : AbstractValidator<RoomModel>
{
    public RoomModelFluentValidator()
    {
        RuleFor(x => x.RoomName)
            .NotEmpty()
            .Length(2, 100);

        RuleFor(m => m.RoomCode)
            .NotEmpty()
            .Matches("^[A-Z0-9-]+$")
            .Length(2, 20);

        RuleFor(m => m.Description)
            .MaximumLength(800);

        RuleFor(m => m.MaxOccupancy)
            .GreaterThan(0)
            .LessThanOrEqualTo(20);

        RuleFor(m => m.MaxAdults)
            .GreaterThanOrEqualTo(0);

        RuleFor(m => m.MaxChildren)
            .GreaterThanOrEqualTo(0);

        RuleFor(m => m.RoomSizeSQM)
            .GreaterThan(0);

        RuleFor(m => m.RoomSizeSQFT)
            .GreaterThan(0);

        RuleFor(m => m.BedConfig)
            .NotEmpty();

        // List
        RuleFor(m => m.Amenities)
            .Must(amenities => amenities == null || amenities.Select(a => a.Name).Distinct().Count() == amenities.Count())
            .WithMessage("There are duplicate amenity names in the list.")
            .When(m => m.Amenities != null);

        RuleForEach(m => m.Amenities).SetValidator(new AmenityModelFluentValidator());

        // Single entity
        //RuleFor(m => m.Amenity).SetValidator(new AmenityModelFluentValidator());
    }
}

