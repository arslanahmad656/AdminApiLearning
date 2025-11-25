namespace Aro.Booking.Domain.Shared;

/// <summary>
/// Types of media files that can be associated with a property
/// </summary>
public enum PropertyMediaType
{
    /// <summary>
    /// Main banner image displayed prominently
    /// </summary>
    Banner = 1,

    /// <summary>
    /// Thumbnail image for listings/previews
    /// </summary>
    Thumbnail = 2,

    /// <summary>
    /// Gallery image for property slideshow
    /// </summary>
    Gallery = 3,

    /// <summary>
    /// Room-specific image
    /// </summary>
    Room = 4,

    /// <summary>
    /// Facility or amenity image
    /// </summary>
    Facility = 5,

    /// <summary>
    /// Exterior view of property
    /// </summary>
    Exterior = 6,

    /// <summary>
    /// Other miscellaneous images
    /// </summary>
    Other = 99
}
