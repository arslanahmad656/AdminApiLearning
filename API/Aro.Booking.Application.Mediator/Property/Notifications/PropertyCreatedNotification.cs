using Aro.Booking.Domain.Shared;
using MediatR;

namespace Aro.Booking.Application.Mediator.Property.Notifications;

public record PropertyCreatedNotification(
    Guid PropertyId,
    Guid GroupId,
    string PropertyName
) : INotification;
