namespace Aro.Common.Application.Services.FileResource;

public record FileResourceInfoDto(
    Guid Id,
    string Name,
    string Uri,
    string Description,
    string Metadata,
    DateTime CreatedOn,
    DateTime? UpdatedOn
);

