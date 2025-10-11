namespace Aro.Admin.Presentation.Api.DTOs;

public record LogoutAllUserModel(Guid UserId, string RefreshToken);
