using Aro.Admin.Application.Services.Email;
using Aro.Common.Domain.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Services;

public partial class EmailTemplateService
{
    private async Task<EmailTemplateDto> GetByIdentifier(string identifier, CancellationToken cancellationToken = default)
    {
        logger.LogInfo("Retrieving email template by identifier: {Identifier}", identifier);

        var template = await repositoryManager.EmailTemplateRepository
            .GetByIdentifier(identifier)
            .FirstOrDefaultAsync(cancellationToken);

        if (template == null)
        {
            logger.LogWarn("Email template not found for identifier: {Identifier}", identifier);
            throw new AroException(errorCodes.EMAIL_TEMPLATE_NOT_FOUND, $"Email template with identifier '{identifier}' not found.");
        }

        logger.LogInfo("Successfully retrieved email template for identifier: {Identifier}", identifier);

        return new EmailTemplateDto(
            template.Identifier,
            template.Subject,
            template.Body,
            template.IsHTML
        );
    }
}
