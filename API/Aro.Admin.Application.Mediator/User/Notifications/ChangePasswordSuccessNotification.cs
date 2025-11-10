using Aro.Admin.Application.Mediator.User.DTOs;
using MediatR;

namespace Aro.Admin.Application.Mediator.User.Notifications;

public record ChangePasswordSuccessNotification(ChangePasswordSuccessNotificationData Data) : INotification;
