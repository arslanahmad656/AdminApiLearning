using Aro.Booking.Domain.Shared;
using Aro.Common.Domain.Entities;

namespace Aro.Booking.Domain.Entities;

/// <summary>
/// Property aggregate root representing a hotel, apartment, or other accommodation
/// </summary>
public class Property : IEntity
{
    #region Core Identity
    public Guid Id { get; set; }
    public Guid? GroupId { get; set; }
    #endregion

    #region Step 1: Property Information
    public string PropertyName { get; set; } = string.Empty;
    public PropertyTypes PropertyTypes { get; set; }
    public int StarRating { get; set; }
    public string Currency { get; set; } = string.Empty; // ISO currency code (e.g., "USD", "EUR", "DKK")
    public string Description { get; set; } = string.Empty;
    #endregion

    #region Step 2: Address & Primary Contact
    // Address
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string WebsiteUrl { get; set; } = string.Empty;

    // Primary Contact
    public string PrimaryContactName { get; set; } = string.Empty;
    public string PrimaryContactEmail { get; set; } = string.Empty;
    public string PrimaryContactRole { get; set; } = "Property Manager"; // Default role
    #endregion

    #region Step 3: Key Selling Points (Collection - up to 4)
    /// <summary>
    /// Collection of key selling points (up to 4, each max 30 characters)
    /// Stored as owned entities to maintain ordering
    /// </summary>
    public List<PropertySellingPoint> SellingPoints { get; set; } = new();
    #endregion

    #region Step 4: Media Files
    /// <summary>
    /// Collection of property images
    /// </summary>
    public List<PropertyMedia> MediaFiles { get; set; } = new();

    /// <summary>
    /// URL or path to property logo (optional)
    /// </summary>
    public string? LogoUrl { get; set; }

    /// <summary>
    /// URL or path to property favicon (optional)
    /// </summary>
    public string? FaviconUrl { get; set; }
    #endregion

    #region Step 5: Marketing & SEO
    /// <summary>
    /// H1 heading / Meta title (max 60 characters, optional)
    /// </summary>
    public string? MetaTitle { get; set; }

    /// <summary>
    /// Meta description for SEO (max 320 characters, optional)
    /// </summary>
    public string? MetaDescription { get; set; }
    #endregion

    #region Workflow Management
    /// <summary>
    /// Indicates if this is a draft (true) or published property (false)
    /// </summary>
    public bool IsDraft { get; set; } = true;

    /// <summary>
    /// Tracks which step of the wizard was last completed (1-6)
    /// </summary>
    public int CurrentStep { get; set; } = 0;
    #endregion

    #region Audit & Status
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    #endregion

    #region Navigation Properties
    public Group? Group { get; set; }
    #endregion

    #region Domain Methods
    /// <summary>
    /// Marks the property as published (no longer a draft)
    /// </summary>
    public void Publish()
    {
        IsDraft = false;
        CurrentStep = 6;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the current wizard step
    /// </summary>
    public void UpdateCurrentStep(int step)
    {
        if (step < 0 || step > 6)
            throw new ArgumentException("Step must be between 0 and 6", nameof(step));

        CurrentStep = Math.Max(CurrentStep, step);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds a selling point if limit not exceeded
    /// </summary>
    public void AddSellingPoint(string text, int displayOrder)
    {
        if (SellingPoints.Count >= 4)
            throw new InvalidOperationException("Cannot add more than 4 selling points");

        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Selling point text cannot be empty", nameof(text));

        if (text.Length > 30)
            throw new ArgumentException("Selling point text cannot exceed 30 characters", nameof(text));

        SellingPoints.Add(new PropertySellingPoint
        {
            Id = Guid.NewGuid(),
            PropertyId = Id,
            Text = text.Trim(),
            DisplayOrder = displayOrder
        });

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Clears all selling points
    /// </summary>
    public void ClearSellingPoints()
    {
        SellingPoints.Clear();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds a media file
    /// </summary>
    public void AddMediaFile(string url, PropertyMediaType mediaType, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Media URL cannot be empty", nameof(url));

        MediaFiles.Add(new PropertyMedia
        {
            Id = Guid.NewGuid(),
            PropertyId = Id,
            Url = url,
            MediaType = mediaType,
            DisplayOrder = displayOrder,
            UploadedAt = DateTime.UtcNow
        });

        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Validates that all required fields for publishing are filled
    /// </summary>
    public bool CanPublish()
    {
        // Step 1 validation
        if (string.IsNullOrWhiteSpace(PropertyName)) return false;
        if (PropertyTypes == PropertyTypes.None) return false;
        if (StarRating < 1 || StarRating > 5) return false;
        if (string.IsNullOrWhiteSpace(Currency)) return false;
        if (string.IsNullOrWhiteSpace(Description)) return false;

        // Step 2 validation
        if (string.IsNullOrWhiteSpace(AddressLine1)) return false;
        if (string.IsNullOrWhiteSpace(City)) return false;
        if (string.IsNullOrWhiteSpace(Country)) return false;
        if (string.IsNullOrWhiteSpace(PostalCode)) return false;
        if (string.IsNullOrWhiteSpace(PhoneNumber)) return false;
        if (string.IsNullOrWhiteSpace(WebsiteUrl)) return false;
        if (string.IsNullOrWhiteSpace(PrimaryContactName)) return false;
        if (string.IsNullOrWhiteSpace(PrimaryContactEmail)) return false;

        // Step 3: Selling points are optional

        // Step 4: At least one media file required
        if (MediaFiles.Count == 0) return false;

        // Step 5: SEO fields are optional

        return true;
    }
    #endregion
}
