using Aro.Booking.Domain.Shared;
using static Aro.Booking.Application.Services.Property.UpdatePropertyDto;

namespace Aro.Booking.Application.Services.Property;

public record UpdatePropertyDto(
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
    List<string>? KeySellingPoints,
    string? MarketingTitle,
    string? MarketingDescription,
    List<FileData>? Files,
    bool SetContactSameAsGroupContact,
    bool SetAddressSameAsGroupAddress,
    List<Guid>? DeletedFileIds
)
{
    public record FileData(
        string FileName,
        Stream Content
    );
}
