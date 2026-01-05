using Aro.Booking.Domain.Shared;

namespace Aro.Booking.Application.Services.Property;

public record GetPropertyResponse(
    Guid PropertyId,
    Guid GroupId,
    string PropertyName,
    List<PropertyTypes> PropertyTypes,
    int StarRating,
    string Currency,
    string Description,
    string AddressLine1,
    string AddressLine2,
    string City,
    string Country,
    string PostalCode,
    string PhoneNumber,
    string Website,
    string ContactName,
    string ContactEmail,
    List<string> KeySellingPoints,
    string MarketingTitle,
    string MarketingDescription,
    Dictionary<string, Guid> FileIds,
    bool IsActive
);
