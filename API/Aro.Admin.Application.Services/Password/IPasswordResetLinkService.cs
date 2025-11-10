namespace Aro.Admin.Application.Services.Password;

public interface IPasswordResetLinkService
{
    Task<Uri> GenerateLink(GenerateLinkParameters parameters, CancellationToken ct = default);
}
