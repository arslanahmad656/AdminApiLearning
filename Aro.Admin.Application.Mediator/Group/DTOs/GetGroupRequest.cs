namespace Aro.Admin.Application.Mediator.Group.DTOs;

public record GetGroupRequest(
    Guid Id,
    string? Include
);

