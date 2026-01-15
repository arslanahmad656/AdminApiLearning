using Aro.UI.Application.DTOs.Room;
using Aro.UI.Infrastructure.Services;
using FluentValidation;

namespace Aro.Admin.Presentation.UI.Validators;

public class RoomModelFluentValidator : AbstractValidator<RoomModel>
{
    private readonly IRoomService _roomService;

    public RoomModelFluentValidator(IRoomService roomService)
    {
        _roomService = roomService;

        RuleFor(x => x.RoomName)
            .NotEmpty()
            .Length(2, 100);

        RuleFor(m => m.RoomCode)
            .NotEmpty()
            .Matches("^[A-Z0-9-]+$")
            .WithMessage("Room code can only contain uppercase letters, numbers, and hyphens.")
            .Length(2, 20)
            .MustAsync(async (model, roomCode, cancellation) =>
            {
                if (string.IsNullOrEmpty(roomCode)) return false;

                var room = await _roomService.RoomCodeExists(new(model.PropertyId, roomCode));

                return !room;
            })
            .WithMessage("A room with this room code already exists.");

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
            .GreaterThanOrEqualTo(129)
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

