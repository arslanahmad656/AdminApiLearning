namespace Aro.Booking.Presentation.Api.DTOs;

public record CreatePolicyModel(
    Guid PropertyId,
    string Title,
    string Description,
    bool IsActive
);

