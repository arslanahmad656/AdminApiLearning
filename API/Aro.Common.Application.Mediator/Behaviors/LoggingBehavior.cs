using Aro.Common.Application.Services.LogManager;
using MediatR;

namespace Aro.Common.Application.Mediator.Behaviors;

public class LoggingBehavior<TRequest, TResponse>(ILogManager<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        
        logger.LogDebug("Starting {RequestName}", requestName);
        
        try
        {
            var response = await next(cancellationToken).ConfigureAwait(false);
            
            logger.LogDebug("Completed {RequestName} successfully", requestName);
            
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed {RequestName} with exception", requestName);
            throw;
        }
    }
}
