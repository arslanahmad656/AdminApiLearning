using Aro.Booking.Application.Mediator.Group.DTOs;
using MediatR;

namespace Aro.Booking.Application.Mediator.Group.Notifications;

public record GroupCreatedNotification(CreateGroupResponse CreateGroupResponse) : INotification;

