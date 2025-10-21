using Aro.Admin.Application.Mediator.PasswordReset.DTOs;
using MediatR;

namespace Aro.Admin.Application.Mediator.PasswordReset.Notifications;

public record PasswordResetTokenGeneratedNotification(PasswordResetTokenGeneratedNotificationData Data) : INotification;
