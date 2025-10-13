namespace Aro.Admin.Application.Mediator.User.DTOs;

public record CreateUserResponse
{
    public Guid Id { get; init; }
    public string? Email { get; init; }
    public IList<Guid>? AssignedRoles { get; init; }
}

