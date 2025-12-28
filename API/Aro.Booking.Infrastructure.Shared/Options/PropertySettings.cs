namespace Aro.Booking.Infrastructure.Shared.Options;

public record PropertySettings
{
    public string DefaultContactPassword { get; init; } = string.Empty;
}
