namespace Aro.Admin.Application.Services.DTOs.ServiceResponses;

public record CreateGroupResponse(
    Guid Id,
    string? GroupName
);
