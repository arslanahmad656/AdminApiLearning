using Aro.Admin.Application.Mediator.PasswordReset.DTOs;
using MediatR;

namespace Aro.Admin.Application.Mediator.PasswordReset.Notifications;

public record PasswordResetTokenValidatedNotification(PasswordResetTokenValidatedNotificationData Data) : INotification;
