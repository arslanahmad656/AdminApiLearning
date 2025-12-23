namespace Aro.Booking.Application.Services.Property;

public record UpdatePropertyResponse(
    Guid PropertyId,
    Guid GroupId
);
