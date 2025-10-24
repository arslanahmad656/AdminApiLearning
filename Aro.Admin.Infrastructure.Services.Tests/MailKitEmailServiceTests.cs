using Aro.Admin.Application.Services;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Email;
using Aro.Admin.Application.Shared.Options;
using Aro.Admin.Domain.Shared.Exceptions;
using Aro.Admin.Tests.Common;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Aro.Admin.Infrastructure.Services.Tests;

public class MailKitEmailServiceTests : TestBase
{
    private readonly Mock<IOptionsSnapshot<EmailSettings>> mockEmailSettings;
    private readonly Mock<ILogManager<MailKitEmailService>> mockLogger;
    private readonly ErrorCodes errorCodes;
    private readonly MailKitEmailService service;

    public MailKitEmailServiceTests()
    {
        mockEmailSettings = new Mock<IOptionsSnapshot<EmailSettings>>();
        mockLogger = new Mock<ILogManager<MailKitEmailService>>();
        errorCodes = new ErrorCodes();

        var emailSettings = new EmailSettings(
            "smtp.example.com",
            587,
            false,
            true,
            "test@example.com",
            "password",
            "Test Sender",
            "test@example.com"
        );
        mockEmailSettings.Setup(x => x.Value).Returns(emailSettings);

        service = new MailKitEmailService(mockEmailSettings.Object, mockLogger.Object, errorCodes);
    }

    [Fact]
    public async Task SendEmail_WithNullParameters_ShouldThrowArgumentNullException()
    {
        SendEmailParameters? parameters = null;

        var action = async () => await service.SendEmail(parameters!);

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task SendEmail_WithEmptyTo_ShouldThrowArgumentException()
    {
        var parameters = new SendEmailParameters(
            string.Empty,
            "Test Subject",
            "Test Body",
            false
        );

        var action = async () => await service.SendEmail(parameters);

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task SendEmail_WithWhitespaceTo_ShouldThrowArgumentException()
    {
        var parameters = new SendEmailParameters(
            "   ",
            "Test Subject",
            "Test Body",
            false
        );

        var action = async () => await service.SendEmail(parameters);

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task SendEmail_WithEmptySubject_ShouldThrowArgumentException()
    {
        var parameters = new SendEmailParameters(
            "recipient@example.com",
            string.Empty,
            "Test Body",
            false
        );

        var action = async () => await service.SendEmail(parameters);

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task SendEmail_WithWhitespaceSubject_ShouldThrowArgumentException()
    {
        var parameters = new SendEmailParameters(
            "recipient@example.com",
            "   ",
            "Test Body",
            false
        );

        var action = async () => await service.SendEmail(parameters);

        await action.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task SendEmail_WithValidParameters_ShouldAttemptToSendEmail()
    {
        var parameters = new SendEmailParameters(
            "recipient@example.com",
            "Test Subject",
            "Test Body",
            false
        );

        var action = async () => await service.SendEmail(parameters);

        await action.Should().ThrowAsync<AroEmailException>()
            .Where(ex => ex.ErrorCode == errorCodes.EMAIL_SENDING_ERROR);
    }

    [Fact]
    public async Task SendEmail_WithHtmlBody_ShouldAttemptToSendEmail()
    {
        var parameters = new SendEmailParameters(
            "recipient@example.com",
            "Test Subject",
            "<html><body>Test Body</body></html>",
            true
        );

        var action = async () => await service.SendEmail(parameters);

        await action.Should().ThrowAsync<AroEmailException>()
            .Where(ex => ex.ErrorCode == errorCodes.EMAIL_SENDING_ERROR);
    }

    [Fact]
    public async Task SendEmail_WithTextBody_ShouldAttemptToSendEmail()
    {
        var parameters = new SendEmailParameters(
            "recipient@example.com",
            "Test Subject",
            "Test Body",
            false
        );

        var action = async () => await service.SendEmail(parameters);

        await action.Should().ThrowAsync<AroEmailException>()
            .Where(ex => ex.ErrorCode == errorCodes.EMAIL_SENDING_ERROR);
    }

    [Fact]
    public async Task SendEmail_WithCcAndBcc_ShouldAttemptToSendEmail()
    {
        var ccEmails = new[] { "cc1@example.com", "cc2@example.com" };
        var bccEmails = new[] { "bcc1@example.com", "bcc2@example.com" };
        var parameters = new SendEmailParameters(
            "recipient@example.com",
            "Test Subject",
            "Test Body",
            false,
            ccEmails,
            bccEmails
        );

        var action = async () => await service.SendEmail(parameters);

        await action.Should().ThrowAsync<AroEmailException>()
            .Where(ex => ex.ErrorCode == errorCodes.EMAIL_SENDING_ERROR);
    }

    [Fact]
    public async Task SendEmail_WithAttachments_ShouldAttemptToSendEmail()
    {
        var attachments = new[]
        {
            new EmailAttachement("test1.txt", new byte[] { 1, 2, 3 }, "text/plain"),
            new EmailAttachement("test2.pdf", new byte[] { 4, 5, 6 }, "application/pdf")
        };
        var parameters = new SendEmailParameters(
            "recipient@example.com",
            "Test Subject",
            "Test Body",
            false,
            null,
            null,
            attachments
        );

        var action = async () => await service.SendEmail(parameters);

        await action.Should().ThrowAsync<AroEmailException>()
            .Where(ex => ex.ErrorCode == errorCodes.EMAIL_SENDING_ERROR);
    }

    [Fact]
    public async Task SendEmail_WithNullCcAndBcc_ShouldAttemptToSendEmail()
    {
        var parameters = new SendEmailParameters(
            "recipient@example.com",
            "Test Subject",
            "Test Body",
            false,
            null,
            null
        );

        var action = async () => await service.SendEmail(parameters);

        await action.Should().ThrowAsync<AroEmailException>()
            .Where(ex => ex.ErrorCode == errorCodes.EMAIL_SENDING_ERROR);
    }

    [Fact]
    public async Task SendEmail_WithNullAttachments_ShouldAttemptToSendEmail()
    {
        var parameters = new SendEmailParameters(
            "recipient@example.com",
            "Test Subject",
            "Test Body",
            false,
            null,
            null,
            null
        );

        var action = async () => await service.SendEmail(parameters);

        await action.Should().ThrowAsync<AroEmailException>()
            .Where(ex => ex.ErrorCode == errorCodes.EMAIL_SENDING_ERROR);
    }

    [Fact]
    public async Task SendEmail_WithEmptyCcAndBcc_ShouldAttemptToSendEmail()
    {
        var parameters = new SendEmailParameters(
            "recipient@example.com",
            "Test Subject",
            "Test Body",
            false,
            [],
            []
        );

        var action = async () => await service.SendEmail(parameters);

        await action.Should().ThrowAsync<AroEmailException>()
            .Where(ex => ex.ErrorCode == errorCodes.EMAIL_SENDING_ERROR);
    }

    [Fact]
    public async Task SendEmail_WithEmptyAttachments_ShouldAttemptToSendEmail()
    {
        var parameters = new SendEmailParameters(
            "recipient@example.com",
            "Test Subject",
            "Test Body",
            false,
            null,
            null,
            []
        );

        var action = async () => await service.SendEmail(parameters);

        await action.Should().ThrowAsync<AroEmailException>()
            .Where(ex => ex.ErrorCode == errorCodes.EMAIL_SENDING_ERROR);
    }

    [Fact]
    public async Task SendEmail_WithComplexEmailParameters_ShouldAttemptToSendEmail()
    {
        var ccEmails = new[] { "cc1@example.com", "cc2@example.com", "cc3@example.com" };
        var bccEmails = new[] { "bcc1@example.com", "bcc2@example.com" };
        var attachments = new[]
        {
            new EmailAttachement("test1.txt", new byte[] { 1, 2, 3 }, "text/plain"),
            new EmailAttachement("test2.pdf", new byte[] { 4, 5, 6 }, "application/pdf")
        };
        var parameters = new SendEmailParameters(
            "recipient@example.com",
            "Test Subject",
            "Test Body",
            true,
            ccEmails,
            bccEmails,
            attachments
        );

        var action = async () => await service.SendEmail(parameters);

        await action.Should().ThrowAsync<AroEmailException>()
            .Where(ex => ex.ErrorCode == errorCodes.EMAIL_SENDING_ERROR);
    }

    [Fact]
    public async Task SendEmail_WithCancelledToken_ShouldThrowAroEmailExceptionWithOperationCanceledInnerException()
    {
        var parameters = new SendEmailParameters(
            "recipient@example.com",
            "Test Subject",
            "Test Body",
            false
        );
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        var action = async () => await service.SendEmail(parameters, cancellationTokenSource.Token);

        await action.Should().ThrowAsync<AroEmailException>()
            .Where(ex => ex.ErrorCode == errorCodes.EMAIL_SENDING_ERROR && 
                        ex.InnerException is OperationCanceledException);
    }

    [Fact]
    public async Task SendEmail_WithInvalidHost_ShouldLogErrorAndThrowAroEmailException()
    {
        var emailSettings = new EmailSettings(
            "invalid-host",
            587,
            false,
            true,
            "test@example.com",
            "password",
            "Test Sender",
            "test@example.com"
        );
        mockEmailSettings.Setup(x => x.Value).Returns(emailSettings);

        var parameters = new SendEmailParameters(
            "recipient@example.com",
            "Test Subject",
            "Test Body",
            false
        );

        var action = async () => await service.SendEmail(parameters);

        await action.Should().ThrowAsync<AroEmailException>()
            .Where(ex => ex.ErrorCode == errorCodes.EMAIL_SENDING_ERROR);

        mockLogger.Verify(x => x.LogError(It.IsAny<Exception>(), "Failed to send email. To={To}, Subject={Subject}", parameters.To, parameters.Subject), Times.Once);
    }

    [Fact]
    public async Task SendEmail_WithInvalidPort_ShouldLogErrorAndThrowAroEmailException()
    {
        var emailSettings = new EmailSettings(
            "smtp.example.com",
            -1,
            false,
            true,
            "test@example.com",
            "password",
            "Test Sender",
            "test@example.com"
        );
        mockEmailSettings.Setup(x => x.Value).Returns(emailSettings);

        var parameters = new SendEmailParameters(
            "recipient@example.com",
            "Test Subject",
            "Test Body",
            false
        );

        var action = async () => await service.SendEmail(parameters);

        await action.Should().ThrowAsync<AroEmailException>()
            .Where(ex => ex.ErrorCode == errorCodes.EMAIL_SENDING_ERROR);

        mockLogger.Verify(x => x.LogError(It.IsAny<Exception>(), "Failed to send email. To={To}, Subject={Subject}", parameters.To, parameters.Subject), Times.Once);
    }

    [Fact]
    public async Task SendEmail_WithInvalidCredentials_ShouldLogErrorAndThrowAroEmailException()
    {
        var emailSettings = new EmailSettings(
            "smtp.example.com",
            587,
            false,
            true,
            "invalid-user",
            "invalid-password",
            "Test Sender",
            "test@example.com"
        );
        mockEmailSettings.Setup(x => x.Value).Returns(emailSettings);

        var parameters = new SendEmailParameters(
            "recipient@example.com",
            "Test Subject",
            "Test Body",
            false
        );

        var action = async () => await service.SendEmail(parameters);

        await action.Should().ThrowAsync<AroEmailException>()
            .Where(ex => ex.ErrorCode == errorCodes.EMAIL_SENDING_ERROR);

        mockLogger.Verify(x => x.LogError(It.IsAny<Exception>(), "Failed to send email. To={To}, Subject={Subject}", parameters.To, parameters.Subject), Times.Once);
    }

    [Fact]
    public async Task SendEmail_WithNetworkException_ShouldLogErrorAndThrowAroEmailException()
    {
        var emailSettings = new EmailSettings(
            "non-existent-host",
            587,
            false,
            true,
            "test@example.com",
            "password",
            "Test Sender",
            "test@example.com"
        );
        mockEmailSettings.Setup(x => x.Value).Returns(emailSettings);

        var parameters = new SendEmailParameters(
            "recipient@example.com",
            "Test Subject",
            "Test Body",
            false
        );

        var action = async () => await service.SendEmail(parameters);

        await action.Should().ThrowAsync<AroEmailException>()
            .Where(ex => ex.ErrorCode == errorCodes.EMAIL_SENDING_ERROR);

        mockLogger.Verify(x => x.LogError(It.IsAny<Exception>(), "Failed to send email. To={To}, Subject={Subject}", parameters.To, parameters.Subject), Times.Once);
    }

    [Fact]
    public async Task SendEmail_WithGenericException_ShouldLogErrorAndThrowAroEmailException()
    {
        var emailSettings = new EmailSettings(
            "smtp.example.com",
            587,
            false,
            true,
            "test@example.com",
            "password",
            "Test Sender",
            "test@example.com"
        );
        mockEmailSettings.Setup(x => x.Value).Returns(emailSettings);

        var parameters = new SendEmailParameters(
            "recipient@example.com",
            "Test Subject",
            "Test Body",
            false
        );

        var action = async () => await service.SendEmail(parameters);

        await action.Should().ThrowAsync<AroEmailException>()
            .Where(ex => ex.ErrorCode == errorCodes.EMAIL_SENDING_ERROR);

        mockLogger.Verify(x => x.LogError(It.IsAny<Exception>(), "Failed to send email. To={To}, Subject={Subject}", parameters.To, parameters.Subject), Times.Once);
    }
}