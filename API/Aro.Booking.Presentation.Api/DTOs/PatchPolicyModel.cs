namespace Aro.Booking.Presentation.Api.DTOs;

public record PatchPolicyModel(
    string? Title,
    string? Description,
    bool? IsActive
);

