using Aro.Booking.Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Aro.Booking.Presentation.Api.DTOs;

public record CreatePropertyModel(
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
    bool SetContactSameAsPrimaryContact,
    string ContactName,
    string ContactEmail,
    List<string>? KeySellingPoints,
    string? MarketingTitle,
    string? MarketingDescription,
    CreatePropertyFilesModel Files
);

public record CreatePropertyFilesModel
{
    public  IFormFile? Favicon { get; init; }
    public required IFormFile Banner1 { get; set; }
    public required IFormFile Banner2 { get; set; }
}