namespace Aro.UI.Application.DTOs;

/// <summary>
/// Unified DTO for saving property data across all wizard steps (1-6)
/// Supports progressive data entry with draft state management
/// </summary>
public record SavePropertyRequest
{
    // Core Identity & State Management
    public Guid? PropertyId { get; init; }
    public Guid? GroupId { get; init; }
    public bool IsDraft { get; init; } = true;
    public int CurrentStep { get; init; }

    // Step 1: Property Information
    public string? PropertyName { get; init; }
    public int? PropertyTypes { get; init; }  // Flags enum as int
    public int? StarRating { get; init; }
    public string? Currency { get; init; }
    public string? Description { get; init; }

    // Step 2: Address & Primary Contact
    public string? AddressLine1 { get; init; }
    public string? AddressLine2 { get; init; }
    public string? City { get; init; }
    public string? Country { get; init; }
    public string? PostalCode { get; init; }
    public string? PhoneNumber { get; init; }
    public string? WebsiteUrl { get; init; }
    public string? PrimaryContactName { get; init; }
    public string? PrimaryContactEmail { get; init; }
    public string? PrimaryContactRole { get; init; }

    // Step 3: Key Selling Points
    public List<SellingPointDto>? SellingPoints { get; init; }

    // Step 4: Media Files
    public List<MediaFileDto>? MediaFiles { get; init; }
    public string? LogoUrl { get; init; }
    public string? FaviconUrl { get; init; }

    // Step 5: Marketing & SEO
    public string? MetaTitle { get; init; }
    public string? MetaDescription { get; init; }
}
