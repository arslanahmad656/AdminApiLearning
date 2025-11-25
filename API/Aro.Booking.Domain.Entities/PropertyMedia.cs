using Aro.Booking.Domain.Shared;
using Aro.Common.Domain.Entities;

namespace Aro.Booking.Domain.Entities;

/// <summary>
/// Represents a media file (image, banner, etc.) for a property
/// Owned entity - part of Property aggregate
/// </summary>
public class PropertyMedia : IEntity
{
    public Guid Id { get; set; }

    /// <summary>
    /// Foreign key to parent Property
    /// </summary>
    public Guid PropertyId { get; set; }

    /// <summary>
    /// URL or path to the media file
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Type of media (Banner, Thumbnail, Gallery, etc.)
    /// </summary>
    public PropertyMediaType MediaType { get; set; }

    /// <summary>
    /// Display order for sorting images
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Original filename if needed for display
    /// </summary>
    public string? OriginalFileName { get; set; }

    /// <summary>
    /// File size in bytes
    /// </summary>
    public long? FileSizeBytes { get; set; }

    /// <summary>
    /// MIME type (e.g., "image/png", "image/jpeg", "image/svg+xml")
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// When the file was uploaded
    /// </summary>
    public DateTime UploadedAt { get; set; }

    /// <summary>
    /// Navigation property to parent Property
    /// </summary>
    public Property Property { get; set; } = null!;
}
