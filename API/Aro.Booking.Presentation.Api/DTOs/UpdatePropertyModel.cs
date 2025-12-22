using Aro.Booking.Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Aro.Booking.Presentation.Api.DTOs;

public record UpdatePropertyModel(
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
    UpdatePropertyFilesModel? Files
);

public record UpdatePropertyFilesModel
{
    public IFormFile? Favicon { get; init; }
    public IFormFile? Banner1 { get; init; }
    public IFormFile? Banner2 { get; init; }
}
