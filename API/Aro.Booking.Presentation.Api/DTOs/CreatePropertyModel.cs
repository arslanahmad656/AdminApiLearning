using Aro.Booking.Domain.Shared;

namespace Aro.Booking.Presentation.Api.DTOs;

/// <summary>
/// Represents the data model for creating a new property.
/// </summary>
/// <param name="GroupId">The unique identifier of the group this property belongs to. Can be null if not associated with a group.</param>
/// <param name="PropertyName">The name of the property.</param>
/// <param name="PropertyTypes">The type(s) of the property. Multiple types can be combined as this is a flags enum.</param>
/// <param name="StarRating">The star rating of the property.</param>
/// <param name="Currency">The currency code used by the property.</param>
/// <param name="Description">A description of the property.</param>
public record CreatePropertyModel(
    Guid? GroupId,
    string PropertyName,
    PropertyTypes PropertyTypes,
    int StarRating,
    string Currency,
    string Description
);