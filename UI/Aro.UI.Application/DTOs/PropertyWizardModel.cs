namespace Aro.UI.Application.DTOs;

/// <summary>
/// Unified model for storing all property wizard steps data in local storage.
/// This model is serialized to JSON and stored in browser's localStorage.
/// </summary>
public class PropertyWizardModel
{
    public const string LocalStorageKeyPrefix = "PropertyWizardData";

    /// <summary>
    /// Gets the localStorage key for a specific group.
    /// Returns "PropertyWizardData_{groupId}" for group-specific data,
    /// or "PropertyWizardData" for data without a group.
    /// </summary>
    public static string GetLocalStorageKey(Guid? groupId)
    {
        return groupId.HasValue && groupId.Value != Guid.Empty
            ? $"{LocalStorageKeyPrefix}_{groupId.Value}"
            : LocalStorageKeyPrefix;
    }

    // Group association
    public Guid? GroupId { get; set; }

    // Step 1: Property Information
    public string PropertyName { get; set; } = string.Empty;
    public PropertyTypesModel PropertyTypes { get; set; } = new();
    public int StarRating { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Step 2: Address & Primary Contact
    public bool SetAddressSameAsGroupAddress { get; set; }
    public string AddressLine1 { get; set; } = string.Empty;
    public string AddressLine2 { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string PhoneCountryCode { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public bool SetContactSameAsPrimaryContact { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string ContactEmail { get; set; } = string.Empty;

    // Step 3: Key Selling Points
    public List<string> KeySellingPoints { get; set; } = new();

    // Step 4: Media Files (stored as base64 for local storage)
    public PropertyFileModel? Favicon { get; set; }
    public PropertyFileModel? Banner1 { get; set; }
    public PropertyFileModel? Banner2 { get; set; }

    // Step 5: Marketing & SEO
    public string MarketingTitle { get; set; } = string.Empty;
    public string MarketingDescription { get; set; } = string.Empty;

    // Track current step for navigation
    public int CurrentStep { get; set; } = 1;
}

/// <summary>
/// Property type selection model with boolean flags for each type.
/// Matches API enum: None, Apartment, BAndB, Guesthouse, Hostel, Hotel, Lodge, Resort
/// </summary>
public class PropertyTypesModel
{
    public bool Hotel { get; set; }
    public bool Guesthouse { get; set; }
    public bool BnB { get; set; }
    public bool Apartment { get; set; }
    public bool Hostel { get; set; }
    public bool Resort { get; set; }
    public bool Lodge { get; set; }

    public bool HasAnySelected()
    {
        return Hotel || Guesthouse || BnB || Apartment || Hostel || Resort || Lodge;
    }

    /// <summary>
    /// Converts boolean properties to a list of property type strings for API.
    /// Values must match the API enum exactly.
    /// </summary>
    public List<string> ToList()
    {
        var types = new List<string>();
        if (Hotel) types.Add("Hotel");
        if (Guesthouse) types.Add("Guesthouse");
        if (BnB) types.Add("BAndB");
        if (Apartment) types.Add("Apartment");
        if (Hostel) types.Add("Hostel");
        if (Resort) types.Add("Resort");
        if (Lodge) types.Add("Lodge");
        return types;
    }
}

/// <summary>
/// Model for storing file data in local storage (base64 encoded).
/// </summary>
public class PropertyFileModel
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string Base64Data { get; set; } = string.Empty;

    /// <summary>
    /// Converts base64 data to byte array for API upload.
    /// </summary>
    public byte[] GetBytes()
    {
        if (string.IsNullOrEmpty(Base64Data))
            return Array.Empty<byte>();

        // Remove data URL prefix if present (e.g., "data:image/png;base64,")
        var base64 = Base64Data;
        if (base64.Contains(","))
        {
            base64 = base64.Substring(base64.IndexOf(",") + 1);
        }

        return Convert.FromBase64String(base64);
    }
}
