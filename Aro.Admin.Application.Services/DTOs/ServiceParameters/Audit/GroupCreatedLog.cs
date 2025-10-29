namespace Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;

public record GroupCreatedLog(
    Guid Id,
    string GroupName
);