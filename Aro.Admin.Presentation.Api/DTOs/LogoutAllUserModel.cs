namespace Aro.Admin.Presentation.Api.DTOs;

public record LogoutAllUserModel
{
    public Guid UserId { get; init; }
}
