namespace Aro.Booking.Application.Mediator.Policy.DTOs;

public record GetPoliciesByPropertyRequest(
    Guid PropertyId,
    string? Include
);

