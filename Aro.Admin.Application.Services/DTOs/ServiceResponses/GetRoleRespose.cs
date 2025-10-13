namespace Aro.Admin.Application.Services.DTOs.ServiceResponses;

public record GetRoleRespose
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool IsBuiltin { get; init; }
}

