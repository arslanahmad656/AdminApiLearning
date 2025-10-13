namespace Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;

public record UserCreatedLog
{
    public Guid Id { get; init; }
    public string? Email { get; init; }
    public IList<Guid>? AssignedRoles { get; init; }
}