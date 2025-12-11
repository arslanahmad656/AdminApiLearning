namespace Aro.Booking.Application.Services.Policy;

public record GetPoliciesByPropertyDto(
    Guid PropertyId,
    string? Include
);

