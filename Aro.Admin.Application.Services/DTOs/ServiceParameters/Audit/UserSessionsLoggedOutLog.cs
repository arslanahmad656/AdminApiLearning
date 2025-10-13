namespace Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;

public record UserSessionsLoggedOutLog
{
    public Guid UserId { get; init; }
}

