using Aro.Booking.Application.Mediator.Property.DTOs;
using MediatR;

namespace Aro.Booking.Application.Mediator.Property.Queries;

public record GetPropertyImageQuery(GetPropertyImageRequest Request) : IRequest<GetPropertyImageResponse>;

