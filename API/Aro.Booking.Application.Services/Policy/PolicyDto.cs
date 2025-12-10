namespace Aro.Booking.Application.Services.Policy;

public record PolicyDto(
    Guid Id,
    string Title,
    string Description,
    bool IsActive
);

