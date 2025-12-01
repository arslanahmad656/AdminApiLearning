namespace Aro.UI.Application.DTOs;

/// <summary>
/// DTO for a media file (uploaded to blob storage)
/// </summary>
public record MediaFileDto(
    string Url,
    string? FileName,
    long? FileSizeBytes,
    string? ContentType,
    int DisplayOrder
);
