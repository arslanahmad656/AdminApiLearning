namespace Aro.Booking.Application.Services.Policy;

public record PolicyDto(
    Guid Id,
    Guid PropertyId,
    string Title,
    string Description,
    bool IsActive
);

