using Aro.Booking.Domain.Shared;
using static Aro.Booking.Application.Mediator.Property.DTOs.CreatePropertyRequest;

namespace Aro.Booking.Application.Mediator.Property.DTOs;

public record CreatePropertyRequest(
    Guid GroupId,
    string PropertyName,
    List<PropertyTypes> PropertyTypes,
    int StarRating,
    string Currency,
    string Description,
    bool SetAddressSameAsGroupAddress,
    string AddressLine1,
    string AddressLine2,
    string City,
    string Country,
    string PostalCode,
    string PhoneNumber,
    string Website,
    bool SetContactSameAsGroupContact,
    string ContactName,
    string ContactEmail,
    List<string>? KeySellingPoints,
    string? MarketingTitle,
    string? MarketingDescription,
    List<FileData> Files
)
{
    public record FileData(string FileName, Stream Content);
}
