namespace Aro.Booking.Application.Mediator.Policy.DTOs;

public record GetPolicyRequest(
    Guid Id,
    string? Include
);

