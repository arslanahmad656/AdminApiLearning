using Aro.Common.Application.Mediator.FileResource.Commands;
using Aro.Common.Application.Mediator.FileResource.DTOs;
using Aro.Common.Application.Mediator.FileResource.Notifications;
using Aro.Common.Application.Services.FileResource;
using Aro.Common.Application.Services.LogManager;
using MediatR;

namespace Aro.Common.Application.Mediator.FileResource.Handlers;

public class CreateFileCommandHandler(
    IFileResourceService fileResourceService,
    IMediator mediator,
    ILogManager<CreateFileCommandHandler> logger
) : IRequestHandler<CreateFileCommand, CreateFileResponse>
{
    public async Task<CreateFileResponse> Handle(CreateFileCommand request, CancellationToken cancellationToken)
    {
        logger.LogInfo("Starting file creation. FileName: {FileName}, SubDirectory: {SubDirectory}",
            request.Request.FileName,
            request.Request.SubDirectory ?? string.Empty);

        var serviceResponse = await fileResourceService.CreateFile(
            new CreateFileResourceDto(
                request.Request.FileName,
                request.Request.Content,
                request.Request.Description,
                request.Request.Metadata,
                request.Request.SubDirectory
            ),
            cancellationToken
        ).ConfigureAwait(false);

        logger.LogInfo("Successfully created file. Id: {Id}, Uri: {Uri}",
            serviceResponse.Id,
            serviceResponse.Uri);

        await mediator.Publish(
            new FileCreatedNotification(
                serviceResponse.Id,
                serviceResponse.Name,
                serviceResponse.Uri
            ),
            cancellationToken
        ).ConfigureAwait(false);

        return new CreateFileResponse(serviceResponse.Uri);
    }
}

