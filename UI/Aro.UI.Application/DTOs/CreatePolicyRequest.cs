namespace Aro.UI.Application.DTOs;

public record CreatePolicyRequest(
    string Title,
    string Description,
    bool IsActive
);
