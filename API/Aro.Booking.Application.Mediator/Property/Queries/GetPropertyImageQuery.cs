using Aro.Booking.Application.Mediator.Common.DTOs;
using MediatR;

namespace Aro.Booking.Application.Mediator.Property.Queries;

public record GetPropertyImageQuery(GetGroupEntityImageRequest Request) : IRequest<GetGroupEntityImageResponse>;

