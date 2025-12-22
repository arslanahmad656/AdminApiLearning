using Aro.Common.Application.Mediator.Country.DTOs;
using MediatR;

namespace Aro.Common.Application.Mediator.Country.Notifications;

public record CountriesCreatedNotification(CountriesCreatedNotificationData Data) : INotification;

