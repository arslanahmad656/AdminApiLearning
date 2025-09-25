using Aro.Admin.Application.Mediator.SystemSettings.DTOs;
using MediatR;

namespace Aro.Admin.Application.Mediator.SystemSettings.Commands;

/// <summary>
/// Initializes the system with an initial administrator.
/// </summary>
public record InitializeSystemCommand(BootstrapUser User) : IRequest<InitializeSystemResponse>;
