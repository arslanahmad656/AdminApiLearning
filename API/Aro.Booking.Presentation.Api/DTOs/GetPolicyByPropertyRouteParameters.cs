namespace Aro.Booking.Presentation.Api.DTOs;

public record GetPolicyByPropertyRouteParameters(
    Guid PropertyId,
    Guid PolicyId
);

