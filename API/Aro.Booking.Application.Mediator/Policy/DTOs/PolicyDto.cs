namespace Aro.Booking.Application.Mediator.Policy.DTOs;

public record PolicyDto(
    Guid Id,
    Guid PropertyId,
    string Title,
    string Description,
    bool IsActive,
    int DisplayOrder
);

