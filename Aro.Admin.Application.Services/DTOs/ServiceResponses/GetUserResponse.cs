namespace Aro.Admin.Application.Services.DTOs.ServiceResponses;

public record GetUserResponse
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string PasswordHash { get; init; } = string.Empty;
    public IEnumerable<GetRoleRespose> Roles { get; init; } = Array.Empty<GetRoleRespose>();
}

