namespace Aro.Booking.Application.Mediator.Property.DTOs;

/// <summary>
/// Response after saving property data
/// </summary>
public record SavePropertyResponse
{
    /// <summary>
    /// Property ID (created or updated)
    /// </summary>
    public Guid PropertyId { get; init; }

    /// <summary>
    /// Group ID associated with the property
    /// </summary>
    public Guid? GroupId { get; init; }

    /// <summary>
    /// Property name
    /// </summary>
    public string PropertyName { get; init; } = string.Empty;

    /// <summary>
    /// Indicates if this is still a draft (true) or published (false)
    /// </summary>
    public bool IsDraft { get; init; }

    /// <summary>
    /// The wizard step that was last completed/saved (1-6)
    /// </summary>
    public int CurrentStep { get; init; }

    /// <summary>
    /// Success flag
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// Optional message (e.g., "Draft saved successfully", "Property published")
    /// </summary>
    public string? Message { get; init; }

    /// <summary>
    /// Timestamp when the property was created
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Timestamp when the property was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; init; }
}
