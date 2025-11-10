using Aro.Admin.Application.Mediator.Authentication.DTOs;
using MediatR;

namespace Aro.Admin.Application.Mediator.Authentication.Notifications;

public record UserLoggedOutAllNotification(UserLoggedOutAllNotificationData Data) : INotification;

