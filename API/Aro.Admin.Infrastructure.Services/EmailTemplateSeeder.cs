using Aro.Admin.Application.Services.Email;
using Aro.Admin.Domain.Shared;

namespace Aro.Admin.Infrastructure.Services;

public class EmailTemplateSeeder(IEmailTemplateService emailTemplateService, SharedKeys sharedKeys) : IEmailTemplateSeeder
{
    // TODO: This method has become repetitive. Next time while adding a new template, refactor to reduce redundancy.
    public async Task Seed(string templatesDirectory, CancellationToken ct = default)
    {
        if (!Directory.Exists(templatesDirectory))
        {
            throw new DirectoryNotFoundException($"The specified root directory '{templatesDirectory}' does not exist.");
        }

        var resetEmailTemplatesFile = Path.Combine(templatesDirectory, "password-reset-email.html");
        if (!File.Exists(resetEmailTemplatesFile))
        {
            throw new FileNotFoundException($"The password reset email template file '{resetEmailTemplatesFile}' was not found.");
        }

        var bodyContent = await File.ReadAllTextAsync(resetEmailTemplatesFile, ct).ConfigureAwait(false);
        await emailTemplateService.Add(new(sharedKeys.PASSWORD_RESET_LINK_TEMPLATE, "Reset Your Password", bodyContent, true), ct).ConfigureAwait(false);

        var resetSuccesfulTemplateFile = Path.Combine(templatesDirectory, "password-reset-success-email.html");
        if (!File.Exists(resetSuccesfulTemplateFile))
        {
            throw new FileNotFoundException($"The password reset email template file '{resetEmailTemplatesFile}' was not found.");
        }

        bodyContent = await File.ReadAllTextAsync(resetSuccesfulTemplateFile, ct).ConfigureAwait(false);
        await emailTemplateService.Add(new(sharedKeys.PASSWORD_RESET_SUCCESSFUL_TEMPLATE, "Password Reset Successful", bodyContent, true), ct).ConfigureAwait(false);
    }
}
