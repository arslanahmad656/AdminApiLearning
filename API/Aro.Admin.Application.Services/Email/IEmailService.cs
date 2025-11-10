namespace Aro.Admin.Application.Services.Email;

public interface IEmailService
{
    Task SendEmail(SendEmailParameters parameters, CancellationToken cancellationToken = default);
}
