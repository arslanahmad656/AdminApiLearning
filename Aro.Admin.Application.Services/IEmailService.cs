using Aro.Admin.Application.Services.DTOs.ServiceParameters.Email;

namespace Aro.Admin.Application.Services;

public interface IEmailService
{
    Task SendEmail(SendEmailParameters parameters, CancellationToken cancellationToken = default);
}
