namespace Aro.UI.Application.DTOs;

public record CreatePolicyRequest(
    Guid PropertyId,
    string Title,
    string Description,
    bool IsActive
);
