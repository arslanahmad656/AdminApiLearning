using Aro.Booking.Application.Mediator.Property.DTOs;
using Aro.Booking.Application.Mediator.Property.Queries;
using Aro.Common.Application.Services.FileResource;
using Aro.Common.Application.Services.LogManager;
using MediatR;

namespace Aro.Booking.Application.Mediator.Property.Handlers;

public class GetPropertyImageQueryHandler(ILogManager<GetPropertyImageQueryHandler> logger, IFileResourceService fileResourceService) : IRequestHandler<GetPropertyImageQuery, GetPropertyImageResponse>
{
    public async Task<GetPropertyImageResponse> Handle(GetPropertyImageQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Handling GetPropertyImageQuery for ImageId: {ImageId}", request.Request.ImageId);

        var serviceResponse = await fileResourceService.ReadFileById(request.Request.ImageId, cancellationToken).ConfigureAwait(false);

        return new(serviceResponse.Id, serviceResponse.Description, serviceResponse.Content);
    }
}
