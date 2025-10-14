namespace Aro.Admin.Application.Mediator.User.DTOs;

public record CreateUserRequest
{
    public string Email { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public string Password { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public ICollection<string> AssignedRoles { get; init; } = new List<string>();
}
