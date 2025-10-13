namespace Aro.Admin.Application.Services.DTOs.ServiceResponses;

public record GetUserRolesResponse
{
    public Guid RoleId { get; init; }
    public string RoleName { get; init; } = string.Empty;
}

