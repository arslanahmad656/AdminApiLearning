using System.Diagnostics.CodeAnalysis;

namespace Aro.Admin.Application.Services.DTOs.ServiceResponses.PasswordComplexity;

public record PasswordComplexityValidationResult(bool Success, IList<string>? Errors);
