namespace Aro.Booking.Application.Mediator.Policy.DTOs;

public record GetPolicyByPropertyRequest(
    Guid PropertyId,
    Guid PolicyId,
    string? Include
);

