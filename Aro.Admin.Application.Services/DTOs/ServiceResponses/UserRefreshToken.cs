namespace Aro.Admin.Application.Services.DTOs.ServiceResponses;

public record UserRefreshToken : RefreshToken
{
    public Guid UserId { get; init; }
}
