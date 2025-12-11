namespace Aro.Booking.Application.Services.Policy;

public record CreatePolicyDto(
    Guid PropertyId,
    string Title,
    string Description,
    bool IsActive
);

