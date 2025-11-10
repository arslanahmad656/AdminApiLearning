namespace Aro.Admin.Presentation.Api.DTOs;

public record LogoutUserModel(Guid UserId, string RefreshToken);
