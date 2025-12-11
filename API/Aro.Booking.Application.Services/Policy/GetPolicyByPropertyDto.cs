namespace Aro.Booking.Application.Services.Policy;

public record GetPolicyByPropertyDto(
    Guid PropertyId,
    Guid PolicyId,
    string? Include
);

