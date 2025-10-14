namespace Aro.Admin.Application.Mediator.Authentication.DTOs;

public record UserLoggedOutAllNotificationData
{
    public Guid UserId { get; init; }
}

