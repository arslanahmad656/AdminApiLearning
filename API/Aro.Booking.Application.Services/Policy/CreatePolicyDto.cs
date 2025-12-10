namespace Aro.Booking.Application.Services.Policy;

public record CreatePolicyDto(
    string Title,
    string Description,
    bool IsActive
);

