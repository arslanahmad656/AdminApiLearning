using Aro.Admin.Application.Mediator.SystemSettings.DTOs;
using MediatR;

namespace Aro.Admin.Application.Mediator.SystemSettings.Notifications;

public record SystemInitializedNotification(InitializeSystemResponse InitializeSystemResponse) : INotification;

