using Aro.Booking.Application.Mediator.Room.DTOs;
using MediatR;

namespace Aro.Booking.Application.Mediator.Room.Commands;

public record ActivateRoomCommand(ActivateRoomRequest ActivateRoomRequest) : IRequest<ActivateRoomResponse>;

