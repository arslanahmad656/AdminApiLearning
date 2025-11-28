using Aro.Common.Application.Mediator.FileResource.DTOs;
using MediatR;

namespace Aro.Common.Application.Mediator.FileResource.Commands;

public record CreateFileCommand(CreateFileRequest Request) : IRequest<CreateFileResponse>;

