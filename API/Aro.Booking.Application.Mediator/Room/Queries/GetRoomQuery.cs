using Aro.Booking.Application.Mediator.Room.DTOs;
using MediatR;

namespace Aro.Booking.Application.Mediator.Room.Queries;

public record GetRoomQuery(GetRoomRequest Data) : IRequest<GetRoomResponse>;
