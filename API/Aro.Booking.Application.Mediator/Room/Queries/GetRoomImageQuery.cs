using Aro.Booking.Application.Mediator.Common.DTOs;
using MediatR;

namespace Aro.Booking.Application.Mediator.Room.Queries;

public record GetRoomImageQuery(Guid RoomId, Guid ImageId) : IRequest<GetGroupEntityImageResponse>;
