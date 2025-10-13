namespace Aro.Admin.Application.Mediator.Shared.DTOs;

public record GetRoleResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool IsBuiltin { get; init; }
}


