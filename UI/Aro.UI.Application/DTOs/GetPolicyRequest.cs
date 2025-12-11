namespace Aro.UI.Application.DTOs;

public record GetPolicyRequest(
    Guid Id,
    string? Include = null
);
