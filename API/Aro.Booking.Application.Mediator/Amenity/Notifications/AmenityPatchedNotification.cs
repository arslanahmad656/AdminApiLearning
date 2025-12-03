using Aro.Booking.Application.Mediator.Amenity.DTOs;
using MediatR;

namespace Aro.Booking.Application.Mediator.Amenity.Notifications;

public record AmenityPatchedNotification(PatchAmenityResponse PatchAmenityResponse) : INotification;

