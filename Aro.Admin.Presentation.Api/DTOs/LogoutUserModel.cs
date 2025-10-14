namespace Aro.Admin.Presentation.Api.DTOs;

public record LogoutUserModel
{
    public Guid UserId { get; init; }
    public string RefreshToken { get; init; } = string.Empty;
}
