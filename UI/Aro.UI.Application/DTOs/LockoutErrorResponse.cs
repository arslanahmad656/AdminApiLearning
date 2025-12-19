namespace Aro.UI.Application.DTOs;

public record LockoutErrorResponse(
    string ErrorCode,
    string ErrorMessage,
    DateTime LockoutEnd
);
