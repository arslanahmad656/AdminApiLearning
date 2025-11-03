namespace Aro.Admin.Presentation.UI.Models;

public record AuthenticationResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiry,
    DateTime RefreshTokenExpiry
);
