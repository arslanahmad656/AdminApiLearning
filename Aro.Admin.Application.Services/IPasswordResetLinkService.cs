using Aro.Admin.Application.Services.DTOs.ServiceParameters.PasswordLink;

namespace Aro.Admin.Application.Services;

public interface IPasswordResetLinkService
{
    Task<Uri> GenerateLink(GenerateLinkParameters parameters, CancellationToken ct = default);
}
