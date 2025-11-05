namespace Aro.Admin.Application.Mediator.Group.DTOs;

public record CreateGroupResponse(
    Guid Id,
    string? GroupName
);

