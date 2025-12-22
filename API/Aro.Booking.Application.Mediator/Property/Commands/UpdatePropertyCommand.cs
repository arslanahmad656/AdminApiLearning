using Aro.Booking.Application.Mediator.Property.DTOs;
using MediatR;

namespace Aro.Booking.Application.Mediator.Property.Commands;

public record UpdatePropertyCommand(UpdatePropertyRequest Request) : IRequest<UpdatePropertyResponse>;
