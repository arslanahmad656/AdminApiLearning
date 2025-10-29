using Aro.Admin.Application.Mediator.PasswordReset.DTOs;
using MediatR;

namespace Aro.Admin.Application.Mediator.PasswordReset.Commands;

public record ResetPasswordCommand(ResetPasswordRequest Data) : IRequest<ResetPasswordResponse>;
