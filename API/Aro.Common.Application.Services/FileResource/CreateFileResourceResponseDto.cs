namespace Aro.Common.Application.Services.FileResource;

public record CreateFileResourceResponseDto(
    Guid Id,
    string Name,
    string Uri
);

