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
            .WithMessage("Room code can only contain uppercase letters, numbers, and hyphens.")
            .Length(2, 20);

        RuleFor(m => m.Description)
            .MaximumLength(800);

        RuleFor(m => m.MaxOccupancy)
            .GreaterThan(0)
            .LessThanOrEqualTo(20);

        RuleFor(m => m.MaxAdults)
            .GreaterThanOrEqualTo(1)
            .WithMessage("There must be at least one adult.")
            .Must((model, maxAdults) =>
            {
                if (model.MaxChildren.HasValue)
                {
                    return maxAdults + model.MaxChildren.Value == model.MaxOccupancy;
                }
                else
                {
                    return maxAdults == model.MaxOccupancy;
                }
            })
            .WithMessage("The sum of adults and children must equal max occupancy."); ;

        RuleFor(m => m.MaxChildren)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Number of children must be greater than or equal to 0.")
            .Must((model, maxChildren) =>
            {
                if (model.MaxAdults.HasValue)
                {
                    return maxChildren + model.MaxAdults.Value == model.MaxOccupancy;
                }
                else
                {
                    return maxChildren == model.MaxOccupancy;
                }
            })
            .WithMessage("The sum of adults and children must equal max occupancy.");

        RuleFor(m => m.RoomSizeSQM)
            .GreaterThanOrEqualTo(12)
            .LessThanOrEqualTo(500);

        RuleFor(m => m.RoomSizeSQFT)
            .GreaterThanOrEqualTo(130)
            .LessThanOrEqualTo(5382);

        RuleFor(m => m.BedConfig)
            .IsInEnum();

        RuleFor(m => m.Amenities)
            .Must(amenities => amenities == null || amenities.Select(a => a.Name).Distinct().Count() == amenities.Count)
            .WithMessage("There are duplicate amenity names in the list.")
            .When(m => m.Amenities != null);

        //RuleForEach(m => m.Amenities).SetValidator(new AmenityModelFluentValidator());
    }
}

