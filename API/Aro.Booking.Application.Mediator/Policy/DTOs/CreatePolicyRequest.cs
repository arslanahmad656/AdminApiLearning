namespace Aro.Booking.Application.Mediator.Policy.DTOs;

public record CreatePolicyRequest(
    string Title,
    string Description,
    bool IsActive
);

