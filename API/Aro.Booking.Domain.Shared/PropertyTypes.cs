namespace Aro.Booking.Domain.Shared;

/// <summary>
/// Types of properties (accommodation types)
/// Flags enum allows multiple types to be selected
/// </summary>
[Flags]
public enum PropertyTypes
{
    None = 0,
    Apartment = 1,      // 1 << 0
    BAndB = 2,          // 1 << 1
    Guesthouse = 4,     // 1 << 2
    Hostel = 8,         // 1 << 3
    Hotel = 16,         // 1 << 4
    Lodge = 32,         // 1 << 5
    Resort = 64,        // 1 << 6
    Villa = 128         // 1 << 7 (added to match UI)
}
