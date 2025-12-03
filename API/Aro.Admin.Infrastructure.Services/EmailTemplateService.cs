using Aro.Admin.Application.Services.DateFormatter;
using Aro.Admin.Application.Services.Email;
using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Shared;
using Aro.Common.Application.Repository;
using Aro.Common.Application.Services.LogManager;
using Aro.Common.Domain.Shared;
using System.Text;

namespace Aro.Admin.Infrastructure.Services;

public partial class EmailTemplateService(Application.Repository.IRepositoryManager repositoryManager, IUnitOfWork unitOfWork, ILogManager<EmailTemplateService> logger, ErrorCodes errorCodes, SharedKeys sharedKeys, IMultiFormatter dateFormatter) : IEmailTemplateService
{
    public async Task<EmailTemplateDto> GetPasswordResetLinkEmail(string name, string resetUrl, int tokenExpiryMinutes, CancellationToken cancellationToken = default)
    {
        var rawTemplate = await GetByIdentifier(sharedKeys.PASSWORD_RESET_LINK_TEMPLATE, cancellationToken).ConfigureAwait(false);
        var sb = new StringBuilder(rawTemplate.Body);

        sb.Replace("{{FirstName}}", name)
            .Replace("{{ResetUrl}}", resetUrl)
            .Replace("{{TokenExpiryMinutes}}", tokenExpiryMinutes.ToString());

        var finalTemplate = rawTemplate with { Body = sb.ToString() };

        return finalTemplate;
    }

    public async Task<EmailTemplateDto> GetPasswordResetSuccesfulEmail(string name, string loginUrl, DateTime resetTime, CancellationToken cancellationToken = default)
    {
        var rawTemplate = await GetByIdentifier(sharedKeys.PASSWORD_RESET_SUCCESSFUL_TEMPLATE, cancellationToken).ConfigureAwait(false);
        var sb = new StringBuilder(rawTemplate.Body);

        sb.Replace("{{FirstName}}", name)
            .Replace("{{LoginUrl}}", loginUrl)
            .Replace("{{ResetDate}}", dateFormatter.FormatDate(DateOnly.FromDateTime(resetTime)))
            .Replace("{{ResetTime}}", dateFormatter.FormatTime(TimeOnly.FromDateTime(resetTime)));

        var finalTemplate = rawTemplate with { Body = sb.ToString() };

        return finalTemplate;
    }

    public async Task<EmailTemplateDto> Add(EmailTemplateDto emailTemplate, CancellationToken cancellationToken = default)
    {
        logger.LogInfo("Creating new email template with identifier: {Identifier}", emailTemplate.Identifier);

        var entity = new EmailTemplate
        {
            Id = Guid.NewGuid(),
            Identifier = emailTemplate.Identifier,
            Subject = emailTemplate.Subject,
            Body = emailTemplate.Body,
            IsHTML = emailTemplate.IsHtml
        };

        await repositoryManager.EmailTemplateRepository.Create(entity, cancellationToken);
        await unitOfWork.SaveChanges(cancellationToken);

        logger.LogInfo("Successfully created email template with identifier: {Identifier}", emailTemplate.Identifier);

        return new EmailTemplateDto(
            entity.Identifier,
            entity.Subject,
            entity.Body,
            entity.IsHTML
        );
    }

    public async Task<IEnumerable<EmailTemplateDto>> AddRange(IEnumerable<EmailTemplateDto> emailTemplates, CancellationToken cancellationToken = default)
    {
        var templatesList = emailTemplates.ToList();
        logger.LogInfo("Creating {Count} email templates", templatesList.Count);

        var entities = templatesList.Select(dto => new EmailTemplate
        {
            Id = Guid.NewGuid(),
            Identifier = dto.Identifier,
            Subject = dto.Subject,
            Body = dto.Body,
            IsHTML = dto.IsHtml
        }).ToList();

        foreach (var entity in entities)
        {
            await repositoryManager.EmailTemplateRepository.Create(entity, cancellationToken);
        }
        await unitOfWork.SaveChanges(cancellationToken);

        logger.LogInfo("Successfully created {Count} email templates", templatesList.Count);

        return entities.Select(entity => new EmailTemplateDto(
            entity.Identifier,
            entity.Subject,
            entity.Body,
            entity.IsHTML
        ));
    }
}
