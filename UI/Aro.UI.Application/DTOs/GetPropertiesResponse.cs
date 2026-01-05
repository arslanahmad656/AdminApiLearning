namespace Aro.UI.Application.DTOs;

/// <summary>
/// Response DTO for property list from API.
/// </summary>
public record PropertyListItemResponse(
    Guid PropertyId,
    Guid GroupId,
    string PropertyName,
    List<string> PropertyTypes,
    int StarRating,
    string Currency,
    string? Description,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? Country,
    string? PostalCode,
    string? PhoneNumber,
    string? Website,
    string? ContactName,
    string? ContactEmail,
    List<string> KeySellingPoints,
    string? MarketingTitle,
    string? MarketingDescription,
    Dictionary<string, Guid> FileIds,
    bool IsActive
);
