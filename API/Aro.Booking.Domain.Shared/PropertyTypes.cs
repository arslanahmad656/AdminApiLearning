namespace Aro.Booking.Domain.Shared;

[Flags]
public enum PropertyTypes
{
    None = 0,
    Apartment = 1,
    BAndB = 2,
    Guesthouse = 4,
    Hostel = 8,
    Hotel = 16,
    Lodge = 32,
    Resort = 64
}
