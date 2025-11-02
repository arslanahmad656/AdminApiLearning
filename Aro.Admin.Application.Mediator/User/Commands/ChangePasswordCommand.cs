using Aro.Admin.Application.Mediator.User.DTOs;
using MediatR;

namespace Aro.Admin.Application.Mediator.User.Commands;

public record ChangePasswordCommand(ChangePasswordRequest Request) : IRequest;
