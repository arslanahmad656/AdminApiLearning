namespace Aro.Common.Application.Services.FileResource;

public record ReadFileResourceResponseDto(
    Guid Id,
    string Name,
    string Uri,
    string Description,
    string Metadata,
    DateTime CreatedOn,
    DateTime? UpdatedOn,
    Stream Content
);

