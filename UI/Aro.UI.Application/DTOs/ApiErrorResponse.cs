namespace Aro.UI.Application.DTOs;

public record ApiErrorResponse(
    string ErrorCode,
    string ErrorMessage
);
