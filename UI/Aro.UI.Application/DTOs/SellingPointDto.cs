namespace Aro.UI.Application.DTOs;

/// <summary>
/// DTO for a single selling point (up to 4, max 30 chars each)
/// </summary>
public record SellingPointDto(
    string Text,
    int DisplayOrder
);
