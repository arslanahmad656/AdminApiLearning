using Aro.Booking.Application.Mediator.Common.DTOs;
using MediatR;

namespace Aro.Booking.Application.Mediator.Group.Queries;

public record GetGroupImageQuery(GetGroupEntityImageRequest Request) : IRequest<GetGroupEntityImageResponse>;
