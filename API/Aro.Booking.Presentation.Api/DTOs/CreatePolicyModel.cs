namespace Aro.Booking.Presentation.Api.DTOs;

public record CreatePolicyModel(
    string Title,
    string Description,
    bool IsActive
);

