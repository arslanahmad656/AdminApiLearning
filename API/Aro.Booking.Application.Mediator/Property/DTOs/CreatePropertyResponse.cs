using Aro.Booking.Domain.Shared;

namespace Aro.Booking.Application.Mediator.Property.DTOs;

public record CreatePropertyResponse(
    Guid Id,
    Guid? GroupId,
    string PropertyName,
    PropertyTypes PropertyTypes,
    int StarRating,
    string Currency,
    string Description
);
