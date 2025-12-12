namespace Aro.UI.Application.DTOs;

public record PolicyDto(
    Guid Id,
    Guid PropertyId,
    string Title,
    string Description,
    bool IsActive
);
