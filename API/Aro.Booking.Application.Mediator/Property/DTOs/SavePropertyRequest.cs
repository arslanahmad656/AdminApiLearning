using Aro.Booking.Domain.Shared;

namespace Aro.Booking.Application.Mediator.Property.DTOs;

/// <summary>
/// Unified DTO for saving property data across all wizard steps (1-6)
/// Supports progressive data entry with draft state management
/// </summary>
public record SavePropertyRequest
{
    #region Core Identity & State Management
    /// <summary>
    /// Property ID (null for new property, Guid for updating existing draft)
    /// </summary>
    public Guid? PropertyId { get; init; }

    /// <summary>
    /// Group ID (optional, can be set later)
    /// </summary>
    public Guid? GroupId { get; init; }

    /// <summary>
    /// Indicates if this is a draft (true) or published property (false)
    /// </summary>
    public bool IsDraft { get; init; } = true;

    /// <summary>
    /// Current wizard step being saved (1-6)
    /// </summary>
    public int CurrentStep { get; init; }
    #endregion

    #region Step 1: Property Information
    /// <summary>
    /// Property name (required for Step 1+)
    /// </summary>
    public string? PropertyName { get; init; }

    /// <summary>
    /// Property types as flags enum (required for Step 1+)
    /// </summary>
    public PropertyTypes? PropertyTypes { get; init; }

    /// <summary>
    /// Star rating 1-5 (required for Step 1+)
    /// </summary>
    public int? StarRating { get; init; }

    /// <summary>
    /// Currency code ISO 4217 (e.g., "USD", "EUR", "DKK") (required for Step 1+)
    /// </summary>
    public string? Currency { get; init; }

    /// <summary>
    /// Property description (required for Step 1+)
    /// </summary>
    public string? Description { get; init; }
    #endregion

    #region Step 2: Address & Primary Contact
    /// <summary>
    /// Address line 1 (required for Step 2+)
    /// </summary>
    public string? AddressLine1 { get; init; }

    /// <summary>
    /// Address line 2 (optional)
    /// </summary>
    public string? AddressLine2 { get; init; }

    /// <summary>
    /// City (required for Step 2+)
    /// </summary>
    public string? City { get; init; }

    /// <summary>
    /// Country (required for Step 2+)
    /// </summary>
    public string? Country { get; init; }

    /// <summary>
    /// Postal code (required for Step 2+)
    /// </summary>
    public string? PostalCode { get; init; }

    /// <summary>
    /// Phone number with country code (required for Step 2+)
    /// </summary>
    public string? PhoneNumber { get; init; }

    /// <summary>
    /// Website URL (required for Step 2+)
    /// </summary>
    public string? WebsiteUrl { get; init; }

    /// <summary>
    /// Primary contact name (required for Step 2+)
    /// </summary>
    public string? PrimaryContactName { get; init; }

    /// <summary>
    /// Primary contact email (required for Step 2+)
    /// </summary>
    public string? PrimaryContactEmail { get; init; }

    /// <summary>
    /// Primary contact role (defaults to "Property Manager")
    /// </summary>
    public string? PrimaryContactRole { get; init; }
    #endregion

    #region Step 3: Key Selling Points
    /// <summary>
    /// List of key selling points (up to 4, each max 30 characters)
    /// Optional - can be empty
    /// </summary>
    public List<SellingPointDto>? SellingPoints { get; init; }
    #endregion

    #region Step 4: Media Files
    /// <summary>
    /// List of media file URLs (uploaded to blob storage)
    /// At least one required for publishing
    /// </summary>
    public List<MediaFileDto>? MediaFiles { get; init; }

    /// <summary>
    /// Logo URL (optional)
    /// </summary>
    public string? LogoUrl { get; init; }

    /// <summary>
    /// Favicon URL (optional)
    /// </summary>
    public string? FaviconUrl { get; init; }
    #endregion

    #region Step 5: Marketing & SEO
    /// <summary>
    /// H1 heading / Meta title (max 60 characters, optional)
    /// </summary>
    public string? MetaTitle { get; init; }

    /// <summary>
    /// Meta description for SEO (max 320 characters, optional)
    /// </summary>
    public string? MetaDescription { get; init; }
    #endregion
}

/// <summary>
/// DTO for a single selling point
/// </summary>
public record SellingPointDto
{
    /// <summary>
    /// Selling point text (max 30 characters)
    /// </summary>
    public string Text { get; init; } = string.Empty;

    /// <summary>
    /// Display order (0-3 for up to 4 points)
    /// </summary>
    public int DisplayOrder { get; init; }
}

/// <summary>
/// DTO for a media file
/// </summary>
public record MediaFileDto
{
    /// <summary>
    /// URL to the uploaded file in blob storage
    /// </summary>
    public string Url { get; init; } = string.Empty;

    /// <summary>
    /// Original filename
    /// </summary>
    public string? FileName { get; init; }

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long? FileSizeBytes { get; init; }

    /// <summary>
    /// MIME content type (e.g., "image/png", "image/jpeg")
    /// </summary>
    public string? ContentType { get; init; }

    /// <summary>
    /// Display order for sorting
    /// </summary>
    public int DisplayOrder { get; init; }
}
