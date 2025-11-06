using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.Email;
using Aro.Admin.Application.Services.LogManager;
using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Domain.Shared;
using Aro.Admin.Domain.Shared.Exceptions;
using Aro.Admin.Infrastructure.Repository.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Aro.Admin.Infrastructure.Services;

public partial class EmailTemplateService(IRepositoryManager repositoryManager, ILogManager<EmailTemplateService> logger, ErrorCodes errorCodes, SharedKeys sharedKeys) : IEmailTemplateService
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
        await repositoryManager.SaveChanges(cancellationToken);

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
        await repositoryManager.SaveChanges(cancellationToken);

        logger.LogInfo("Successfully created {Count} email templates", templatesList.Count);

        return entities.Select(entity => new EmailTemplateDto(
            entity.Identifier,
            entity.Subject,
            entity.Body,
            entity.IsHTML
        ));
    }
}
