using Aro.Booking.Application.Mediator.Policy.DTOs;
using MediatR;

namespace Aro.Booking.Application.Mediator.Policy.Notifications;

public record PolicyCreatedNotification(CreatePolicyResponse CreatePolicyResponse) : INotification;

