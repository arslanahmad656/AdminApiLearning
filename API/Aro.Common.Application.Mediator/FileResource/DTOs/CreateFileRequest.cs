namespace Aro.Common.Application.Mediator.FileResource.DTOs;

public record CreateFileRequest(
    string FileName,
    Stream Content,
    string? Description,
    string? Metadata,
    string? SubDirectory
);

