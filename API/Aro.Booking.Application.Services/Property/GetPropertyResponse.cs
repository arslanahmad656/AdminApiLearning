using Aro.Booking.Domain.Shared;

namespace Aro.Booking.Application.Services.Property;

public record GetPropertyResponse(
    Guid Id,
    Guid? GroupId,
    string PropertyName,
    PropertyTypes PropertyTypes,
    int StarRating,
    string Currency,
    string Description,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool IsActive
);
