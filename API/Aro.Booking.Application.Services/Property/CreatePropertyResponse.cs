using Aro.Booking.Domain.Shared;

namespace Aro.Booking.Application.Services.Property;

public record CreatePropertyResponse(
    Guid PropertyId,
    Guid GroupId,
    string PropertyName
    //List<PropertyTypes> PropertyTypes,
    //int StarRating,
    //string Currency,
    //string Description,
    //bool SetAddressSameAsGroupAddress,
    //string AddressLine1,
    //string AddressLine2,
    //string City,
    //string Country,
    //string PostalCode,
    //string PhoneNumber,
    //string Website,
    //bool SetContactSameAsPrimaryContact,
    //string ContactName,
    //string ContactEmail,
    //List<string> KeySellingPoints,
    //string MarketingTitle,
    //string MarketingDescription,
    //Dictionary<string, List<string>> FileUrls
);
