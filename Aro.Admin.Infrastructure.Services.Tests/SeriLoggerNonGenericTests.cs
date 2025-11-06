using Aro.Admin.Infrastructure.Services;
using Aro.Admin.Tests.Common;
using Aro.Common.Application.Services.LogManager;
using FluentAssertions;
using Moq;
using Serilog;
using Xunit;

namespace Aro.Admin.Infrastructure.Services.Tests;

public class SeriLoggerNonGenericTests : TestBase
{
    private readonly Mock<ILogger> mockLogger;
    private readonly SeriLogger seriLogger;

    public SeriLoggerNonGenericTests()
    {
        mockLogger = new Mock<ILogger>();
        seriLogger = new SeriLogger();
        
        // We need to mock the static Log.Logger call
        // This is a limitation of testing static dependencies
        // In a real scenario, you might want to refactor to use dependency injection
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        var logger = new SeriLogger();
        
        logger.Should().NotBeNull();
        logger.Should().BeAssignableTo<ILogManager>();
    }

    [Fact]
    public void Constructor_ShouldCreateMultipleInstances()
    {
        var logger1 = new SeriLogger();
        var logger2 = new SeriLogger();

        logger1.Should().NotBeNull();
        logger2.Should().NotBeNull();
        logger1.Should().NotBeSameAs(logger2);
    }

    #endregion

    #region LogDebug Tests

    [Fact]
    public void LogDebug_WithSimpleMessage_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogDebug("Simple debug message");

        act.Should().NotThrow();
    }

    [Fact]
    public void LogDebug_WithMessageTemplate_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogDebug("Debug message with {Parameter}", "value");

        act.Should().NotThrow();
    }

    [Fact]
    public void LogDebug_WithMultipleParameters_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogDebug("Debug message with {Param1} and {Param2}", "value1", "value2");

        act.Should().NotThrow();
    }

    [Fact]
    public void LogDebug_WithNullMessage_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogDebug(null!);

        act.Should().NotThrow();
    }

    [Fact]
    public void LogDebug_WithEmptyMessage_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogDebug("");

        act.Should().NotThrow();
    }

    [Fact]
    public void LogDebug_WithSpecialCharacters_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogDebug("Debug message with special chars: {Special}", "test\n\r\t");

        act.Should().NotThrow();
    }

    [Fact]
    public void LogDebug_WithNoParameters_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogDebug("Message without parameters");

        act.Should().NotThrow();
    }

    [Fact]
    public void LogDebug_WithNullParameters_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogDebug("Message with {Param}", (object)null!);

        act.Should().NotThrow();
    }

    [Fact]
    public void LogDebug_WithEmptyParametersArray_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogDebug("Message with {Param}", new object[0]);

        act.Should().NotThrow();
    }

    [Fact]
    public void LogDebug_WithVariousDataTypes_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogDebug("Message with int: {Int}, string: {String}, bool: {Bool}, date: {Date}", 
            42, "test", true, DateTime.UtcNow);

        act.Should().NotThrow();
    }

    [Fact]
    public void LogDebug_WithComplexObjects_ShouldNotThrow()
    {
        var list = new List<string> { "item1", "item2" };
        var dict = new Dictionary<string, int> { { "key1", 1 }, { "key2", 2 } };
        
        Action act = () => seriLogger.LogDebug("Message with list: {List} and dict: {Dict}", list, dict);

        act.Should().NotThrow();
    }

    #endregion

    #region LogInfo Tests

    [Fact]
    public void LogInfo_WithSimpleMessage_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogInfo("Simple info message");

        act.Should().NotThrow();
    }

    [Fact]
    public void LogInfo_WithMessageTemplate_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogInfo("Info message with {Parameter}", "value");

        act.Should().NotThrow();
    }

    [Fact]
    public void LogInfo_WithMultipleParameters_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogInfo("Info message with {Param1} and {Param2}", "value1", "value2");

        act.Should().NotThrow();
    }

    [Fact]
    public void LogInfo_WithNullMessage_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogInfo(null!);

        act.Should().NotThrow();
    }

    [Fact]
    public void LogInfo_WithEmptyMessage_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogInfo("");

        act.Should().NotThrow();
    }

    [Fact]
    public void LogInfo_WithComplexObject_ShouldNotThrow()
    {
        var complexObject = new { Id = 1, Name = "Test", Items = new[] { "item1", "item2" } };
        
        Action act = () => seriLogger.LogInfo("Info message with complex object: {Object}", complexObject);

        act.Should().NotThrow();
    }

    [Fact]
    public void LogInfo_WithBusinessContext_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogInfo("User {UserId} performed action {Action} at {Timestamp}", 
            123, "login", DateTime.UtcNow);

        act.Should().NotThrow();
    }

    #endregion

    #region LogWarn Tests

    [Fact]
    public void LogWarn_WithSimpleMessage_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogWarn("Simple warning message");

        act.Should().NotThrow();
    }

    [Fact]
    public void LogWarn_WithMessageTemplate_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogWarn("Warning message with {Parameter}", "value");

        act.Should().NotThrow();
    }

    [Fact]
    public void LogWarn_WithMultipleParameters_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogWarn("Warning message with {Param1} and {Param2}", "value1", "value2");

        act.Should().NotThrow();
    }

    [Fact]
    public void LogWarn_WithNullMessage_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogWarn(null!);

        act.Should().NotThrow();
    }

    [Fact]
    public void LogWarn_WithEmptyMessage_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogWarn("");

        act.Should().NotThrow();
    }

    [Fact]
    public void LogWarn_WithExceptionContext_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogWarn("Warning in context of {Context}", "UserService");

        act.Should().NotThrow();
    }

    [Fact]
    public void LogWarn_WithPerformanceWarning_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogWarn("Performance warning: {Operation} took {Duration}ms", "DatabaseQuery", 5000);

        act.Should().NotThrow();
    }

    #endregion

    #region LogError Tests

    [Fact]
    public void LogError_WithSimpleMessage_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogError("Simple error message");

        act.Should().NotThrow();
    }

    [Fact]
    public void LogError_WithMessageTemplate_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogError("Error message with {Parameter}", "value");

        act.Should().NotThrow();
    }

    [Fact]
    public void LogError_WithMultipleParameters_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogError("Error message with {Param1} and {Param2}", "value1", "value2");

        act.Should().NotThrow();
    }

    [Fact]
    public void LogError_WithNullMessage_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogError(null!);

        act.Should().NotThrow();
    }

    [Fact]
    public void LogError_WithEmptyMessage_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogError("");

        act.Should().NotThrow();
    }

    [Fact]
    public void LogError_WithExceptionAndMessage_ShouldNotThrow()
    {
        var exception = new InvalidOperationException("Test exception");
        
        Action act = () => seriLogger.LogError(exception, "Error occurred: {Message}", "Something went wrong");

        act.Should().NotThrow();
    }

    [Fact]
    public void LogError_WithExceptionAndSimpleMessage_ShouldNotThrow()
    {
        var exception = new ArgumentNullException("param");
        
        Action act = () => seriLogger.LogError(exception, "Error occurred");

        act.Should().NotThrow();
    }

    [Fact]
    public void LogError_WithNullException_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogError((Exception)null!, "Error message");

        act.Should().NotThrow();
    }

    [Fact]
    public void LogError_WithExceptionAndNullMessage_ShouldNotThrow()
    {
        var exception = new Exception("Test");
        
        Action act = () => seriLogger.LogError(exception, null!);

        act.Should().NotThrow();
    }

    [Fact]
    public void LogError_WithBusinessException_ShouldNotThrow()
    {
        var businessException = new BusinessException("User not found", "USER_NOT_FOUND");
        
        Action act = () => seriLogger.LogError(businessException, "Business error occurred: {ErrorCode}", businessException.ErrorCode);

        act.Should().NotThrow();
    }

    #endregion

    #region Message Template Tests

    [Fact]
    public void LogDebug_WithValidTemplate_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogDebug("User {UserId} performed action {Action} at {Timestamp}", 
            123, "login", DateTime.UtcNow);

        act.Should().NotThrow();
    }

    [Fact]
    public void LogDebug_WithInvalidTemplate_ShouldNotThrow()
    {
        // Serilog is generally forgiving with template mismatches
        Action act = () => seriLogger.LogDebug("Message with {MissingParam}", "value1", "value2");

        act.Should().NotThrow();
    }

    [Fact]
    public void LogDebug_WithEscapedCharacters_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogDebug("Message with quotes: \"{Value}\" and braces: {{Value}}", "test");

        act.Should().NotThrow();
    }

    [Fact]
    public void LogDebug_WithSpecialTemplateCharacters_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogDebug("Message with @{Property} and {#Count} items", "value", 5);

        act.Should().NotThrow();
    }

    [Fact]
    public void LogInfo_WithStructuredLogging_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogInfo("Processing request {RequestId} for user {UserId} with status {Status}", 
            Guid.NewGuid(), 123, "pending");

        act.Should().NotThrow();
    }

    #endregion

    #region Interface Implementation Tests

    [Fact]
    public void SeriLogger_ShouldImplementILogManager()
    {
        var logger = new SeriLogger();
        
        logger.Should().BeAssignableTo<ILogManager>();
    }

    [Fact]
    public void SeriLogger_ShouldImplementIService()
    {
        var logger = new SeriLogger();
        
        logger.Should().BeAssignableTo<IService>();
    }

    [Fact]
    public void SeriLogger_ShouldHaveAllRequiredMethods()
    {
        var logger = new SeriLogger();
        
        // Test that all interface methods exist and are callable
        Action debugAct = () => logger.LogDebug("test");
        Action infoAct = () => logger.LogInfo("test");
        Action warnAct = () => logger.LogWarn("test");
        Action errorAct = () => logger.LogError("test");
        Action errorWithExceptionAct = () => logger.LogError(new Exception(), "test");

        debugAct.Should().NotThrow();
        infoAct.Should().NotThrow();
        warnAct.Should().NotThrow();
        errorAct.Should().NotThrow();
        errorWithExceptionAct.Should().NotThrow();
    }

    #endregion

    #region Edge Cases and Error Handling

    [Fact]
    public void LogDebug_WithVeryLongMessage_ShouldNotThrow()
    {
        var longMessage = new string('A', 10000);
        
        Action act = () => seriLogger.LogDebug("Long message: {Message}", longMessage);

        act.Should().NotThrow();
    }

    [Fact]
    public void LogDebug_WithVeryLongTemplate_ShouldNotThrow()
    {
        var template = "Message with " + string.Join(", ", Enumerable.Range(1, 100).Select(i => $"{{Param{i}}}"));
        var parameters = Enumerable.Range(1, 100).Select(i => (object)$"value{i}").ToArray();
        
        Action act = () => seriLogger.LogDebug(template, parameters);

        act.Should().NotThrow();
    }

    [Fact]
    public void LogError_WithAggregateException_ShouldNotThrow()
    {
        var innerException1 = new InvalidOperationException("Inner 1");
        var innerException2 = new ArgumentException("Inner 2");
        var aggregateException = new AggregateException("Multiple errors", innerException1, innerException2);
        
        Action act = () => seriLogger.LogError(aggregateException, "Multiple errors occurred");

        act.Should().NotThrow();
    }

    [Fact]
    public void LogError_WithCustomException_ShouldNotThrow()
    {
        var customException = new CustomTestException("Custom error message");
        
        Action act = () => seriLogger.LogError(customException, "Custom exception occurred");

        act.Should().NotThrow();
    }

    [Fact]
    public void LogError_WithNestedException_ShouldNotThrow()
    {
        var innerException = new InvalidOperationException("Inner exception");
        var outerException = new ApplicationException("Outer exception", innerException);
        
        Action act = () => seriLogger.LogError(outerException, "Nested exception occurred");

        act.Should().NotThrow();
    }

    #endregion

    #region Performance Tests

    [Fact]
    public void LogDebug_MultipleCalls_ShouldNotThrow()
    {
        Action act = () =>
        {
            for (int i = 0; i < 1000; i++)
            {
                seriLogger.LogDebug("Message {Index}", i);
            }
        };

        act.Should().NotThrow();
    }

    [Fact]
    public void LogInfo_MultipleCalls_ShouldNotThrow()
    {
        Action act = () =>
        {
            for (int i = 0; i < 1000; i++)
            {
                seriLogger.LogInfo("Message {Index}", i);
            }
        };

        act.Should().NotThrow();
    }

    [Fact]
    public void LogWarn_MultipleCalls_ShouldNotThrow()
    {
        Action act = () =>
        {
            for (int i = 0; i < 1000; i++)
            {
                seriLogger.LogWarn("Message {Index}", i);
            }
        };

        act.Should().NotThrow();
    }

    [Fact]
    public void LogError_MultipleCalls_ShouldNotThrow()
    {
        Action act = () =>
        {
            for (int i = 0; i < 1000; i++)
            {
                seriLogger.LogError("Message {Index}", i);
            }
        };

        act.Should().NotThrow();
    }

    [Fact]
    public void LogError_WithException_MultipleCalls_ShouldNotThrow()
    {
        var exception = new Exception("Test exception");
        
        Action act = () =>
        {
            for (int i = 0; i < 1000; i++)
            {
                seriLogger.LogError(exception, "Message {Index}", i);
            }
        };

        act.Should().NotThrow();
    }

    #endregion

    #region Real-world Usage Scenarios

    [Fact]
    public void LogInfo_WithApplicationStartup_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogInfo("Application started at {StartupTime} with version {Version}", 
            DateTime.UtcNow, "1.0.0");

        act.Should().NotThrow();
    }

    [Fact]
    public void LogInfo_WithUserAction_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogInfo("User {UserId} {Action} {Resource} at {Timestamp}", 
            123, "accessed", "dashboard", DateTime.UtcNow);

        act.Should().NotThrow();
    }

    [Fact]
    public void LogWarn_WithConfigurationIssue_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogWarn("Configuration issue: {Setting} is not set, using default value {DefaultValue}", 
            "DatabaseConnectionString", "localhost");

        act.Should().NotThrow();
    }

    [Fact]
    public void LogError_WithDatabaseError_ShouldNotThrow()
    {
        var dbException = new InvalidOperationException("Database connection failed");
        
        Action act = () => seriLogger.LogError(dbException, "Database error occurred while {Operation} for user {UserId}", 
            "updating profile", 123);

        act.Should().NotThrow();
    }

    [Fact]
    public void LogDebug_WithPerformanceMetrics_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogDebug("Performance: {Operation} completed in {Duration}ms with {MemoryUsage}MB memory", 
            "DataProcessing", 150, 256);

        act.Should().NotThrow();
    }

    #endregion

    #region Test Classes

    public class BusinessException : Exception
    {
        public string ErrorCode { get; }

        public BusinessException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }

        public BusinessException(string message, string errorCode, Exception innerException) : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }

    public class CustomTestException : Exception
    {
        public CustomTestException(string message) : base(message) { }
        public CustomTestException(string message, Exception innerException) : base(message, innerException) { }
    }

    #endregion
}
