namespace Aro.Admin.Presentation.UI.Models;

public class PasswordResetResponse
{
    public bool Success { get; set; }
    public string? ErrorCode { get; set; }
    public string Message { get; set; } = string.Empty;
}
