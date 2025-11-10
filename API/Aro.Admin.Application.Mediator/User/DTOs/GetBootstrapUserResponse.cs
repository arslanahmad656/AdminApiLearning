namespace Aro.Admin.Application.Mediator.User.DTOs;

public record GetBootstrapUserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}
