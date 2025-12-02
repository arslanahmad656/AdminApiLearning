using Aro.Booking.Application.Mediator.Room.DTOs;
using MediatR;

namespace Aro.Booking.Application.Mediator.Room.Queries;

public record GetRoomsQuery(GetRoomsRequest Data) : IRequest<GetRoomsResponse>;
