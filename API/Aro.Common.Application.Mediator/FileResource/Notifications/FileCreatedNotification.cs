using MediatR;

namespace Aro.Common.Application.Mediator.FileResource.Notifications;

public record FileCreatedNotification(
    Guid Id,
    string Name,
    string Uri
) : INotification;

