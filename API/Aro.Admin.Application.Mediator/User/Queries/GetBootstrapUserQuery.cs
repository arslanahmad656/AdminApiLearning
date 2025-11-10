using Aro.Admin.Application.Mediator.User.DTOs;
using MediatR;

namespace Aro.Admin.Application.Mediator.User.Queries;

public record GetBootstrapUserQuery(string BootstrapPassword) : IRequest<GetBootstrapUserResponse>;
