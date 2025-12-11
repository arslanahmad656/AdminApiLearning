namespace Aro.Booking.Application.Services.Policy;

public record PolicyPatchDto(
    Guid Id,
    string? Title,
    string? Description,
    bool? IsActive
);

