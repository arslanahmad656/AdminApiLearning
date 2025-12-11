namespace Aro.Booking.Application.Mediator.Policy.DTOs;

public record PolicyPatchDto(
    Guid Id,
    string? Title,
    string? Description,
    bool? IsActive
);

