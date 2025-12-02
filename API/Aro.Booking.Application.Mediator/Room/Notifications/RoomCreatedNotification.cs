using Aro.Booking.Application.Mediator.Room.DTOs;
using MediatR;

namespace Aro.Booking.Application.Mediator.Room.Notifications;

public record RoomCreatedNotification(CreateRoomResponse CreateRoomResponse) : INotification;

