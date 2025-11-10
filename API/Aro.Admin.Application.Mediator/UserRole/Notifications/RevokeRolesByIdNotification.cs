using Aro.Admin.Application.Mediator.UserRole.DTOs;
using MediatR;

namespace Aro.Admin.Application.Mediator.UserRole.Notifications;

public record RevokeRolesByIdNotification(RevokeRolesByIdResponse Data) : INotification;

