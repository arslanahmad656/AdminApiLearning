namespace Aro.Booking.Application.Mediator.Policy.DTOs;

public record PolicyDto(
    Guid Id,
    string Title,
    string Description,
    bool IsActive
);

