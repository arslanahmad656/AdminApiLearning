using Aro.Booking.Presentation.Api.DTOs;
using FluentValidation;

namespace Aro.Booking.Presentation.Api.Validators;

public class CreateRoomModelValidator : AbstractValidator<CreateRoomModel>
{
    public CreateRoomModelValidator()
    {
        RuleFor(m => m.RoomName)
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
            .GreaterThan(0)
            .When(m => m.RoomSizeSQM.HasValue);

        RuleFor(m => m.RoomSizeSQFT)
            .GreaterThan(0)
            .When(m => m.RoomSizeSQFT.HasValue);

        RuleFor(m => m.BedConfig)
            .NotEmpty();

        RuleFor(m => m.AmenityIds)
            .Must(ids => ids?.Distinct().Count() == ids?.Count)
            .WithMessage("Amenities list contains duplicate values.");

        //RuleFor(m => m.IsActive);
    }
}
