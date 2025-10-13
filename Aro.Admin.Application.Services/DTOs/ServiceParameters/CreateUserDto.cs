namespace Aro.Admin.Application.Services.DTOs.ServiceParameters;

public record CreateUserDto
{
    public string Email { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public string Password { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public ICollection<string> AssignedRoles { get; init; } = new List<string>();
}

