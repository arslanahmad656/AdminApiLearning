namespace Aro.Booking.Application.Mediator.Group.DTOs;

public record GetGroupsResponse(
    List<GroupDto> Groups,
    int TotalCount
);

