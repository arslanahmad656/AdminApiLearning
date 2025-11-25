using Aro.Common.Domain.Entities;

namespace Aro.Booking.Domain.Entities;

/// <summary>
/// Represents a key selling point for a property
/// Owned entity - part of Property aggregate
/// </summary>
public class PropertySellingPoint : IEntity
{
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to parent Property
    /// </summary>
    public Guid PropertyId { get; set; }

    /// <summary>
    /// The selling point text (max 30 characters)
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Display order for sorting (0-3 for up to 4 points)
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Navigation property to parent Property
    /// </summary>
    public Property Property { get; set; } = null!;
}
