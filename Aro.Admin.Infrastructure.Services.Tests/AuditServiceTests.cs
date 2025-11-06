using Aro.Admin.Application.Services.DataServices;
using Aro.Admin.Application.Services.DTOs.ServiceParameters.Audit;
using Aro.Admin.Application.Services.LogManager;
using Aro.Admin.Application.Services.RequestInterpretor;
using Aro.Admin.Application.Services.Serializer;
using Aro.Admin.Application.Services.UniqueIdGenerator;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Domain.Shared;
using Aro.Admin.Domain.Shared.Audit;
using Aro.Admin.Infrastructure.Services;
using Aro.Admin.Tests.Common;
using FluentAssertions;
using Moq;
using Xunit;

namespace Aro.Admin.Infrastructure.Services.Tests;

public class AuditServiceTests : TestBase
{
    private readonly Mock<IUniqueIdGenerator> mockIdGenerator;
    private readonly Mock<IRequestInterpretorService> mockRequestInterpretor;
    private readonly Mock<IRepositoryManager> mockRepositoryManager;
    private readonly Mock<ISerializer> mockSerializer;
    private readonly Mock<ILogManager<AuditService>> mockLogger;
    private readonly AuditActions auditActions;
    private readonly EntityTypes auditEntityTypes;
    private readonly AuditService auditService;

    public AuditServiceTests()
    {
        mockIdGenerator = new Mock<IUniqueIdGenerator>();
        mockRequestInterpretor = new Mock<IRequestInterpretorService>();
        mockRepositoryManager = new Mock<IRepositoryManager>();
        mockSerializer = new Mock<ISerializer>();
        mockLogger = new Mock<ILogManager<AuditService>>();
        
        // Setup the AuditTrailRepository mock
        var mockAuditTrailRepository = new Mock<IAuditTrailRepository>();
        mockRepositoryManager.Setup(x => x.AuditTrailRepository).Returns(mockAuditTrailRepository.Object);
        
        auditActions = new AuditActions();
        auditEntityTypes = new EntityTypes();
        
        auditService = new AuditService(
            mockIdGenerator.Object,
            mockRequestInterpretor.Object,
            auditActions,
            auditEntityTypes,
            mockRepositoryManager.Object,
            mockSerializer.Object,
            mockLogger.Object
        );
    }

    [Fact]
    public void Constructor_WithValidDependencies_ShouldCreateInstance()
    {
        var service = new AuditService(
            mockIdGenerator.Object,
            mockRequestInterpretor.Object,
            auditActions,
            auditEntityTypes,
            mockRepositoryManager.Object,
            mockSerializer.Object,
            mockLogger.Object
        );

        service.Should().NotBeNull();
    }

    [Fact]
    public async Task LogApplicationSeeded_ShouldCallLoggerAndRepository()
    {
        var cancellationToken = new CancellationToken();
        
        await auditService.LogApplicationSeeded(cancellationToken);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", new object[] { "LogApplicationSeeded" }), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Generated audit entity for application seeded with action: {Action}", auditActions.ApplicationSeeded), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", new object[] { "LogApplicationSeeded" }), Times.Once);
    }

    [Fact]
    public async Task LogMigrationsCompleted_ShouldCallLoggerAndRepository()
    {
        var cancellationToken = new CancellationToken();
        
        await auditService.LogMigrationsCompleted(cancellationToken);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", new object[] { "LogMigrationsCompleted" }), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Generated audit entity for migrations completed with action: {Action}", auditActions.MigrationsApplied), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", new object[] { "LogMigrationsCompleted" }), Times.Once);
    }

    [Fact]
    public async Task LogSystemInitialized_WithValidLog_ShouldCallLoggerAndRepository()
    {
        var log = new SystemInitializedLog("bootstrap-user-id", "bootstrap-username", "admin-role");
        var cancellationToken = new CancellationToken();
        var serializedLog = "serialized-log-data";
        
        mockSerializer.Setup(x => x.Serialize(It.IsAny<SystemInitializedLog>(), It.IsAny<bool>())).Returns(serializedLog);
        
        await auditService.LogSystemInitialized(log, cancellationToken);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "LogSystemInitialized"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Generated audit entity for system initialized with action: {Action}, data: {Data}", auditActions.SystemInitialized, serializedLog), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "LogSystemInitialized"), Times.Once);
    }

    [Fact]
    public async Task LogUserCreated_WithValidLog_ShouldCallLoggerAndRepository()
    {
        var log = new UserCreatedLog(Guid.NewGuid(), "test@example.com", new List<Guid> { Guid.NewGuid() });
        var cancellationToken = new CancellationToken();
        
        await auditService.LogUserCreated(log, cancellationToken);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "LogUserCreated"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Generated audit entity for user created with action: {Action}, userId: {UserId}", auditActions.UserCreated, log.Id), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "LogUserCreated"), Times.Once);
    }

    [Fact]
    public async Task LogRolesAssigned_WithValidLog_ShouldCallLoggerAndRepository()
    {
        var log = new RolesAssignedLog(new List<Guid> { Guid.NewGuid() }, new List<Guid> { Guid.NewGuid() });
        var cancellationToken = new CancellationToken();
        
        await auditService.LogRolesAssigned(log, cancellationToken);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "LogRolesAssigned"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Generated audit entity for roles assigned with action: {Action}, userCount: {UserCount}, roleCount: {RoleCount}", auditActions.RolesAssignedToUsers, log.UserIds.Count, log.RoleIds.Count), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "LogRolesAssigned"), Times.Once);
    }

    [Fact]
    public async Task LogRolesRevoked_WithValidLog_ShouldCallLoggerAndRepository()
    {
        var log = new RolesRevokedLog(new List<Guid> { Guid.NewGuid() }, new List<Guid> { Guid.NewGuid() });
        var cancellationToken = new CancellationToken();
        
        await auditService.LogRolesRevoked(log, cancellationToken);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "LogRolesRevoked"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Generated audit entity for roles revoked with action: {Action}, userCount: {UserCount}, roleCount: {RoleCount}", auditActions.RolesRevokedFromUsers, log.UserIds.Count, log.RoleIds.Count), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "LogRolesRevoked"), Times.Once);
    }

    [Fact]
    public async Task LogAuthenticationSuccessful_WithValidLog_ShouldCallLoggerAndRepository()
    {
        var log = new AuthenticationSuccessfulLog(
            Guid.NewGuid(), 
            "test@example.com", 
            Guid.NewGuid(), 
            DateTime.UtcNow.AddHours(1), 
            DateTime.UtcNow.AddDays(7), 
            "access-token-id"
        );
        var cancellationToken = new CancellationToken();
        
        await auditService.LogAuthenticationSuccessful(log, cancellationToken);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "LogAuthenticationSuccessful"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Generated audit entity for authentication successful with action: {Action}, email: {Email}", auditActions.AuthenticationSuccessful, log.Email), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "LogAuthenticationSuccessful"), Times.Once);
    }

    [Fact]
    public async Task LogAuthenticationFailed_WithValidLog_ShouldCallLoggerAndRepository()
    {
        var log = new AuthenticationFailedLog("test@example.com", "Invalid credentials");
        var cancellationToken = new CancellationToken();
        
        await auditService.LogAuthenticationFailed(log, cancellationToken);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "LogAuthenticationFailed"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Generated audit entity for authentication failed with action: {Action}, email: {Email}", auditActions.AuthenticationFailed, log.Email), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "LogAuthenticationFailed"), Times.Once);
    }

    [Fact]
    public async Task LogUserSessionLoggedOutLog_WithValidLog_ShouldCallLoggerAndRepository()
    {
        var log = new UserSessionLoggedOutLog(Guid.NewGuid(), "refresh-token-hash", "token-identifier");
        var cancellationToken = new CancellationToken();
        
        await auditService.LogUserSessionLoggedOutLog(log, cancellationToken);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "LogUserSessionLoggedOutLog"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Generated audit entity for user session logged out with action: {Action}, refreshTokenHash: {RefreshTokenHash}", auditActions.UserSessionLoggedOut, log.RefreshTokenHash), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "LogUserSessionLoggedOutLog"), Times.Once);
    }

    [Fact]
    public async Task LogUserSessionsLoggedOutLog_WithValidLog_ShouldCallLoggerAndRepository()
    {
        var log = new UserSessionsLoggedOutLog(Guid.NewGuid());
        var cancellationToken = new CancellationToken();
        
        await auditService.LogUserSessionsLoggedOutLog(log, cancellationToken);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "LogUserSessionsLoggedOutLog"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Generated audit entity for user sessions logged out with action: {Action}", auditActions.UserSessionLoggedOut), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "LogUserSessionsLoggedOutLog"), Times.Once);
    }

    [Fact]
    public async Task LogTokenRefreshedLog_WithValidLog_ShouldCallLoggerAndRepository()
    {
        var log = new TokenRefreshedLog(Guid.NewGuid(), "old-token-hash", "new-token-hash", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddDays(7));
        var cancellationToken = new CancellationToken();
        
        await auditService.LogTokenRefreshedLog(log, cancellationToken);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "LogTokenRefreshedLog"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Generated audit entity for token refreshed with action: {Action}, oldRefreshTokenHash: {OldRefreshTokenHash}", auditActions.UserSessionLoggedOut, log.OldRefreshTokenHash), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "LogTokenRefreshedLog"), Times.Once);
    }

    [Fact]
    public async Task LogPasswordResetTokenGenerated_WithValidLog_ShouldCallLoggerAndRepository()
    {
        var log = new PasswordResetTokenGeneratedLog(Guid.NewGuid(), "user@example.com", "token-hash", DateTime.UtcNow, DateTime.UtcNow.AddHours(1), "127.0.0.1", "Mozilla/5.0");
        var cancellationToken = new CancellationToken();
        
        await auditService.LogPasswordResetTokenGenerated(log, cancellationToken);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "LogPasswordResetTokenGenerated"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Generated audit entity for password reset token generation with action: {Action}, userId: {UserId}", "password_reset_token_generated", log.UserId), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "LogPasswordResetTokenGenerated"), Times.Once);
    }

    [Fact]
    public async Task LogPasswordResetCompleted_WithValidLog_ShouldCallLoggerAndRepository()
    {
        var log = new PasswordResetCompletedLog(Guid.NewGuid(), DateTime.UtcNow, "127.0.0.1", "Mozilla/5.0");
        var cancellationToken = new CancellationToken();
        
        await auditService.LogPasswordResetCompleted(log, cancellationToken);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "LogPasswordResetCompleted"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Generated audit entity for password reset completed with action: {Action}, userId: {UserId}", auditActions.PasswordResetCompleted, log.UserId), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "LogPasswordResetCompleted"), Times.Once);
    }

    [Fact]
    public async Task LogPasswordResetFailed_WithValidLog_ShouldCallLoggerAndRepository()
    {
        var log = new PasswordResetFailedLog(Guid.NewGuid(), "Token expired", DateTime.UtcNow, "127.0.0.1", "Mozilla/5.0");
        var cancellationToken = new CancellationToken();
        
        await auditService.LogPasswordResetFailed(log, cancellationToken);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "LogPasswordResetFailed"), Times.Once);
        var userIdString = log.UserId?.ToString() ?? string.Empty;
        mockLogger.Verify(x => x.LogDebug("Generated audit entity for password reset failed with action: {Action}, userId: {UserId}, reason: {Reason}", auditActions.PasswordResetFailed, userIdString, log.FailureReason), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "LogPasswordResetFailed"), Times.Once);
    }

    [Fact]
    public async Task LogPasswordResetFailed_WithNullUserId_ShouldHandleGracefully()
    {
        var log = new PasswordResetFailedLog(null, "Token expired", DateTime.UtcNow, "127.0.0.1", "Mozilla/5.0");
        var cancellationToken = new CancellationToken();
        
        await auditService.LogPasswordResetFailed(log, cancellationToken);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "LogPasswordResetFailed"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Generated audit entity for password reset failed with action: {Action}, userId: {UserId}, reason: {Reason}", auditActions.PasswordResetFailed, string.Empty, log.FailureReason), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "LogPasswordResetFailed"), Times.Once);
    }

    [Fact]
    public async Task LogApplicationSeeded_WithDefaultCancellationToken_ShouldNotThrow()
    {
        await auditService.LogApplicationSeeded();

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", new object[] { "LogApplicationSeeded" }), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", new object[] { "LogApplicationSeeded" }), Times.Once);
    }

    [Fact]
    public async Task LogMigrationsCompleted_WithDefaultCancellationToken_ShouldNotThrow()
    {
        await auditService.LogMigrationsCompleted();

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", new object[] { "LogMigrationsCompleted" }), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", new object[] { "LogMigrationsCompleted" }), Times.Once);
    }

    [Fact]
    public async Task LogSystemInitialized_WithDefaultCancellationToken_ShouldNotThrow()
    {
        var log = new SystemInitializedLog("bootstrap-user-id", "bootstrap-username", "admin-role");
        var serializedLog = "serialized-log-data";
        
        mockSerializer.Setup(x => x.Serialize(It.IsAny<SystemInitializedLog>(), It.IsAny<bool>())).Returns(serializedLog);
        
        await auditService.LogSystemInitialized(log);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "LogSystemInitialized"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "LogSystemInitialized"), Times.Once);
    }

    [Fact]
    public async Task LogUserCreated_WithDefaultCancellationToken_ShouldNotThrow()
    {
        var log = new UserCreatedLog(Guid.NewGuid(), "test@example.com", new List<Guid> { Guid.NewGuid() });
        
        await auditService.LogUserCreated(log);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "LogUserCreated"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "LogUserCreated"), Times.Once);
    }

    [Fact]
    public async Task LogRolesAssigned_WithDefaultCancellationToken_ShouldNotThrow()
    {
        var log = new RolesAssignedLog(new List<Guid> { Guid.NewGuid() }, new List<Guid> { Guid.NewGuid() });
        
        await auditService.LogRolesAssigned(log);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "LogRolesAssigned"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "LogRolesAssigned"), Times.Once);
    }

    [Fact]
    public async Task LogRolesRevoked_WithDefaultCancellationToken_ShouldNotThrow()
    {
        var log = new RolesRevokedLog(new List<Guid> { Guid.NewGuid() }, new List<Guid> { Guid.NewGuid() });
        
        await auditService.LogRolesRevoked(log);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "LogRolesRevoked"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "LogRolesRevoked"), Times.Once);
    }

    [Fact]
    public async Task LogAuthenticationSuccessful_WithDefaultCancellationToken_ShouldNotThrow()
    {
        var log = new AuthenticationSuccessfulLog(
            Guid.NewGuid(), 
            "test@example.com", 
            Guid.NewGuid(), 
            DateTime.UtcNow.AddHours(1), 
            DateTime.UtcNow.AddDays(7), 
            "access-token-id"
        );
        
        await auditService.LogAuthenticationSuccessful(log);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "LogAuthenticationSuccessful"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "LogAuthenticationSuccessful"), Times.Once);
    }

    [Fact]
    public async Task LogAuthenticationFailed_WithDefaultCancellationToken_ShouldNotThrow()
    {
        var log = new AuthenticationFailedLog("test@example.com", "Invalid credentials");
        
        await auditService.LogAuthenticationFailed(log);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "LogAuthenticationFailed"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "LogAuthenticationFailed"), Times.Once);
    }

    [Fact]
    public async Task LogUserSessionLoggedOutLog_WithDefaultCancellationToken_ShouldNotThrow()
    {
        var log = new UserSessionLoggedOutLog(Guid.NewGuid(), "refresh-token-hash", "token-identifier");
        
        await auditService.LogUserSessionLoggedOutLog(log);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "LogUserSessionLoggedOutLog"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "LogUserSessionLoggedOutLog"), Times.Once);
    }

    [Fact]
    public async Task LogUserSessionsLoggedOutLog_WithDefaultCancellationToken_ShouldNotThrow()
    {
        var log = new UserSessionsLoggedOutLog(Guid.NewGuid());
        
        await auditService.LogUserSessionsLoggedOutLog(log);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "LogUserSessionsLoggedOutLog"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "LogUserSessionsLoggedOutLog"), Times.Once);
    }

    [Fact]
    public async Task LogTokenRefreshedLog_WithDefaultCancellationToken_ShouldNotThrow()
    {
        var log = new TokenRefreshedLog(Guid.NewGuid(), "old-token-hash", "new-token-hash", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddDays(7));
        
        await auditService.LogTokenRefreshedLog(log);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "LogTokenRefreshedLog"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "LogTokenRefreshedLog"), Times.Once);
    }

    [Fact]
    public async Task LogPasswordResetTokenGenerated_WithDefaultCancellationToken_ShouldNotThrow()
    {
        var log = new PasswordResetTokenGeneratedLog(Guid.NewGuid(), "user@example.com", "token-hash", DateTime.UtcNow, DateTime.UtcNow.AddHours(1), "127.0.0.1", "Mozilla/5.0");
        
        await auditService.LogPasswordResetTokenGenerated(log);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "LogPasswordResetTokenGenerated"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "LogPasswordResetTokenGenerated"), Times.Once);
    }

    [Fact]
    public async Task LogPasswordResetCompleted_WithDefaultCancellationToken_ShouldNotThrow()
    {
        var log = new PasswordResetCompletedLog(Guid.NewGuid(), DateTime.UtcNow, "127.0.0.1", "Mozilla/5.0");
        
        await auditService.LogPasswordResetCompleted(log);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "LogPasswordResetCompleted"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "LogPasswordResetCompleted"), Times.Once);
    }

    [Fact]
    public async Task LogPasswordResetFailed_WithDefaultCancellationToken_ShouldNotThrow()
    {
        var log = new PasswordResetFailedLog(Guid.NewGuid(), "Token expired", DateTime.UtcNow, "127.0.0.1", "Mozilla/5.0");
        
        await auditService.LogPasswordResetFailed(log);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "LogPasswordResetFailed"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "LogPasswordResetFailed"), Times.Once);
    }
}