using Aro.Booking.Application.Mediator.Room.DTOs;
using MediatR;

namespace Aro.Booking.Application.Mediator.Room.Commands;

public record PatchRoomCommand(PatchRoomRequest PatchRoomRequest) : IRequest<PatchRoomResponse>;

