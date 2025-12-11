namespace Aro.Booking.Application.Mediator.Policy.DTOs;

public record CreatePolicyRequest(
    Guid PropertyId,
    string Title,
    string Description,
    bool IsActive
);

