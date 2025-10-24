using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.PasswordReset;
using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Domain.Shared.Exceptions;
using Aro.Admin.Infrastructure.Repository.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Aro.Admin.Infrastructure.Services;

public class EmailTemplateService(IRepositoryManager repositoryManager, ILogManager<EmailTemplateService> logger, ErrorCodes errorCodes) : IEmailTemplateService
{
    public async Task<EmailTemplateDto> GetByIdentifier(string identifier, CancellationToken cancellationToken = default)
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
