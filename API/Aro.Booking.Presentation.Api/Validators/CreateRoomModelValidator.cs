using Aro.Booking.Presentation.Api.DTOs.Room;
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
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(m => m.BedConfig)
            .IsInEnum();

        RuleFor(m => m.Amenities)
            .Must(a => a?.Distinct().Count() == a?.Count)
            .WithMessage("Amenities list contains duplicate values.");

        //RuleFor(m => m.IsActive);
    }
}
