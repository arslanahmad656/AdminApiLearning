using Aro.Admin.Application.Mediator.Group.DTOs;
using MediatR;

namespace Aro.Admin.Application.Mediator.Group.Notifications;

public record GroupPatchedNotification(PatchGroupResponse PatchGroupResponse) : INotification;

