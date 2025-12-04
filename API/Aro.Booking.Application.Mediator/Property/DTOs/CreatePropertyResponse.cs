using Aro.Booking.Domain.Shared;

namespace Aro.Booking.Application.Mediator.Property.DTOs;

public record CreatePropertyResponse(
    Guid PropertyId,
    Guid GroupId
);
