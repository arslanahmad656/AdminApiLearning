namespace Aro.UI.Application.DTOs;

/// <summary>
/// Response after saving property data
/// </summary>
public record SavePropertyResponse(
    Guid PropertyId,
    Guid? GroupId,
    string PropertyName,
    bool IsDraft,
    int CurrentStep,
    bool Success,
    string? Message,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
