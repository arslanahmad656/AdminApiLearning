using Aro.Booking.Domain.Shared;

namespace Aro.Booking.Application.Mediator.Property.DTOs;

public record CreatePropertyRequest(
    Guid? GroupId,
    string PropertyName,
    PropertyTypes PropertyTypes,
    int StarRating,
    string Currency,
    string Description
);
