using Aro.Booking.Application.Mediator.Common.DTOs;
using Aro.Booking.Application.Mediator.Group.Queries;
using Aro.Booking.Application.Mediator.Property.Queries;
using Aro.Common.Application.Services.FileResource;
using Aro.Common.Application.Services.LogManager;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aro.Booking.Application.Mediator.Group.Handlers;

public class GetGroupImageQueryHandler(ILogManager<GetGroupImageQueryHandler> logger, IFileResourceService fileResourceService) : IRequestHandler<GetGroupImageQuery, GetGroupEntityImageResponse>
{
    public async Task<GetGroupEntityImageResponse> Handle(GetGroupImageQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Handling GetGroupImageQuery for ImageId: {ImageId}", request.Request.ImageId);

        var serviceResponse = await fileResourceService.ReadFileById(request.Request.ImageId, cancellationToken).ConfigureAwait(false);

        return new(serviceResponse.Id, serviceResponse.Description, serviceResponse.Content);
    }
}
