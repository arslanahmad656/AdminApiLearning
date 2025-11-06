using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.Email;
using Aro.Admin.Application.Services.LogManager;
using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Domain.Shared;
using Aro.Admin.Domain.Shared.Exceptions;
using Aro.Admin.Infrastructure.Services;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Aro.Admin.Infrastructure.Services.Tests;

public class EmailTemplateServiceTests
{
    private readonly Fixture _fixture;
    private readonly Mock<IRepositoryManager> _mockRepositoryManager;
    private readonly Mock<IEmailTemplateRepository> _mockEmailTemplateRepository;
    private readonly Mock<ILogManager<EmailTemplateService>> _mockLogger;
    private readonly ErrorCodes _errorCodes;
    private readonly SharedKeys _sharedKeys;
    private readonly EmailTemplateService _service;

    public EmailTemplateServiceTests()
    {
        _fixture = new Fixture();
        _mockRepositoryManager = new Mock<IRepositoryManager>();
        _mockEmailTemplateRepository = new Mock<IEmailTemplateRepository>();
        _mockLogger = new Mock<ILogManager<EmailTemplateService>>();
        _errorCodes = new ErrorCodes();
        _sharedKeys = new SharedKeys();

        _mockRepositoryManager.Setup(rm => rm.EmailTemplateRepository).Returns(_mockEmailTemplateRepository.Object);

        _service = new EmailTemplateService(
            _mockRepositoryManager.Object,
            _mockLogger.Object,
            _errorCodes,
            _sharedKeys);
    }

    [Fact]
    public async Task Add_WithValidEmailTemplate_ShouldCreateAndReturnTemplate()
    {
        var emailTemplate = _fixture.Create<EmailTemplateDto>();

        _mockEmailTemplateRepository.Setup(etr => etr.Create(It.IsAny<EmailTemplate>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockRepositoryManager.Setup(rm => rm.SaveChanges(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _service.Add(emailTemplate);

        result.Should().NotBeNull();
        result.Identifier.Should().Be(emailTemplate.Identifier);
        result.Subject.Should().Be(emailTemplate.Subject);
        result.Body.Should().Be(emailTemplate.Body);
        result.IsHtml.Should().Be(emailTemplate.IsHtml);

        _mockEmailTemplateRepository.Verify(etr => etr.Create(It.Is<EmailTemplate>(et => 
            et.Identifier == emailTemplate.Identifier &&
            et.Subject == emailTemplate.Subject &&
            et.Body == emailTemplate.Body &&
            et.IsHTML == emailTemplate.IsHtml), It.IsAny<CancellationToken>()), Times.Once);
        _mockRepositoryManager.Verify(rm => rm.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Add_WithRepositoryException_ShouldPropagateException()
    {
        var emailTemplate = _fixture.Create<EmailTemplateDto>();
        var exception = _fixture.Create<Exception>();

        _mockEmailTemplateRepository.Setup(etr => etr.Create(It.IsAny<EmailTemplate>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        var action = async () => await _service.Add(emailTemplate);

        await action.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task AddRange_WithValidEmailTemplates_ShouldCreateAndReturnTemplates()
    {
        var emailTemplates = _fixture.CreateMany<EmailTemplateDto>(3).ToList();

        _mockEmailTemplateRepository.Setup(etr => etr.Create(It.IsAny<EmailTemplate>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockRepositoryManager.Setup(rm => rm.SaveChanges(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _service.AddRange(emailTemplates);

        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().AllSatisfy(template => 
        {
            template.Identifier.Should().NotBeNullOrEmpty();
            template.Subject.Should().NotBeNullOrEmpty();
            template.Body.Should().NotBeNullOrEmpty();
        });

        _mockEmailTemplateRepository.Verify(etr => etr.Create(It.IsAny<EmailTemplate>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        _mockRepositoryManager.Verify(rm => rm.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddRange_WithEmptyCollection_ShouldReturnEmptyCollection()
    {
        var emailTemplates = new List<EmailTemplateDto>();

        var result = await _service.AddRange(emailTemplates);

        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _mockEmailTemplateRepository.Verify(etr => etr.Create(It.IsAny<EmailTemplate>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockRepositoryManager.Verify(rm => rm.SaveChanges(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddRange_WithRepositoryException_ShouldPropagateException()
    {
        var emailTemplates = _fixture.CreateMany<EmailTemplateDto>(2).ToList();
        var exception = _fixture.Create<Exception>();

        _mockEmailTemplateRepository.Setup(etr => etr.Create(It.IsAny<EmailTemplate>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        var action = async () => await _service.AddRange(emailTemplates);

        await action.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task AddRange_WithNullEmailTemplates_ShouldThrowArgumentNullException()
    {
        var action = async () => await _service.AddRange(null!);

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task Add_WithCancellationToken_ShouldPassCancellationToken()
    {
        var emailTemplate = _fixture.Create<EmailTemplateDto>();
        var cancellationToken = _fixture.Create<CancellationToken>();

        _mockEmailTemplateRepository.Setup(etr => etr.Create(It.IsAny<EmailTemplate>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockRepositoryManager.Setup(rm => rm.SaveChanges(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _service.Add(emailTemplate, cancellationToken);

        result.Should().NotBeNull();
        _mockEmailTemplateRepository.Verify(etr => etr.Create(It.IsAny<EmailTemplate>(), cancellationToken), Times.Once);
        _mockRepositoryManager.Verify(rm => rm.SaveChanges(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task AddRange_WithCancellationToken_ShouldPassCancellationToken()
    {
        var emailTemplates = _fixture.CreateMany<EmailTemplateDto>(2).ToList();
        var cancellationToken = _fixture.Create<CancellationToken>();

        _mockEmailTemplateRepository.Setup(etr => etr.Create(It.IsAny<EmailTemplate>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockRepositoryManager.Setup(rm => rm.SaveChanges(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _service.AddRange(emailTemplates, cancellationToken);

        result.Should().NotBeNull();
        _mockEmailTemplateRepository.Verify(etr => etr.Create(It.IsAny<EmailTemplate>(), cancellationToken), Times.Exactly(2));
        _mockRepositoryManager.Verify(rm => rm.SaveChanges(cancellationToken), Times.Once);
    }

    // Note: GetPasswordResetLinkEmail tests are skipped due to complex async queryable mocking requirements
    // These tests would require a more sophisticated mocking approach for Entity Framework async operations
}