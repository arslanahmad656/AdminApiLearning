using Aro.Booking.Application.Mediator.Policy.DTOs;
using MediatR;

namespace Aro.Booking.Application.Mediator.Policy.Notifications;

public record PolicyDeletedNotification(DeletePolicyResponse DeletePolicyResponse) : INotification;

