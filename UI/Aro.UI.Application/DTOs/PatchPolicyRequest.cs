namespace Aro.UI.Application.DTOs;

public record PatchPolicyRequest(
    Guid Id,
    string? Title,
    string? Description,
    bool? IsActive
);
