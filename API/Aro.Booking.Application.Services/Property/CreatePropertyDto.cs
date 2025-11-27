using Aro.Booking.Domain.Shared;

namespace Aro.Booking.Application.Services.Property;

public record CreatePropertyDto(
    Guid? GroupId,
    string PropertyName,
    PropertyTypes PropertyTypes,
    int StarRating,
    string Currency,
    string Description
);
