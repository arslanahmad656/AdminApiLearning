using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Domain.Shared;

namespace Aro.Admin.Infrastructure.Services;

public class EmailTemplateSeeder(IEmailTemplateService emailTemplateService, SharedKeys sharedKeys) : IEmailTemplateSeeder
{
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
    }
}
