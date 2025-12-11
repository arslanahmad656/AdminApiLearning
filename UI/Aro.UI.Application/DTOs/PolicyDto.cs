namespace Aro.UI.Application.DTOs;

public record PolicyDto(
    Guid Id,
    string Title,
    string Description,
    bool IsActive
);
