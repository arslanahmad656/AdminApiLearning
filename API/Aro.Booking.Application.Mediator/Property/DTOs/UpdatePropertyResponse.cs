namespace Aro.Booking.Application.Mediator.Property.DTOs;

public record UpdatePropertyResponse(
    Guid PropertyId,
    Guid GroupId
);
