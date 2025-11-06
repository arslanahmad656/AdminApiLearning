namespace Aro.Booking.Application.Mediator.Group.DTOs;

public record GetGroupRequest(
    Guid Id,
    string? Include
);

