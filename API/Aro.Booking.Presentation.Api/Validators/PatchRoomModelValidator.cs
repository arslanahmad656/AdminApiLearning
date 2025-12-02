using Aro.Booking.Presentation.Api.DTOs;
using FluentValidation;

namespace Aro.Booking.Presentation.Api.Validators;

public class PatchRoomModelValidator : AbstractValidator<PatchRoomModel>
{
    public PatchRoomModelValidator()
    {
        RuleFor(m => m.RoomName)
            .Length(2, 100)
            .When(m => m.RoomName != null);

        RuleFor(m => m.RoomCode)
            .Length(1, 20)
            .Matches("^[A-Z0-9-]+$")
            .When(m => m.RoomCode != null);

        RuleFor(m => m.Description)
            .MaximumLength(800);

        RuleFor(m => m.MaxOccupancy)
            .GreaterThan(0)
            .LessThanOrEqualTo(20)
            .When(m => m.MaxOccupancy.HasValue);

        RuleFor(m => m.MaxAdults)
            .GreaterThanOrEqualTo(0)
            .When(m => m.MaxAdults.HasValue);

        RuleFor(m => m.MaxChildren)
            .GreaterThanOrEqualTo(0)
            .When(m => m.MaxChildren.HasValue);

        RuleFor(m => m.RoomSizeSQM)
            .GreaterThan(0)
            .When(m => m.RoomSizeSQM.HasValue);

        RuleFor(m => m.RoomSizeSQFT)
            .GreaterThan(0)
            .When(m => m.RoomSizeSQFT.HasValue);

        RuleFor(m => m.AmenityIds)
            .Must(ids => ids == null || ids.Distinct().Count() == ids.Count)
            .WithMessage("Amenities list contains duplicate values.");
    }
}
