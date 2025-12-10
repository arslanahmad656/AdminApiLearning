namespace Aro.Booking.Application.Services.Policy;

public record GetPolicyDto(
    Guid Id,
    string? Include
);

