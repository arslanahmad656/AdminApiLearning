namespace Aro.Common.Application.Services.FileResource;

public record CreateFileResourceDto(
    string FileName,
    Stream Content,
    string? Description,
    string? Metadata,
    string? SubDirectory
);

