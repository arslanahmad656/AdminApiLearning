using Aro.Admin.Application.Services.LogManager;
using Aro.Admin.Infrastructure.Services;
using Aro.Admin.Tests.Common;
using FluentAssertions;
using Moq;
using Serilog;
using Xunit;

namespace Aro.Admin.Infrastructure.Services.Tests;

public class SeriLoggerTests : TestBase
{
    private readonly Mock<ILogger> mockLogger;
    private readonly SeriLogger<TestService> seriLogger;

    public SeriLoggerTests()
    {
        mockLogger = new Mock<ILogger>();
        seriLogger = new SeriLogger<TestService>();
        
        // We need to mock the static Log.ForContext<T>() call
        // This is a limitation of testing static dependencies
        // In a real scenario, you might want to refactor to use dependency injection
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        var logger = new SeriLogger<TestService>();
        
        logger.Should().NotBeNull();
        logger.Should().BeAssignableTo<ILogManager<TestService>>();
    }

    [Fact]
    public void Constructor_ShouldCreateInstanceWithDifferentTypes()
    {
        var stringLogger = new SeriLogger<string>();
        var intLogger = new SeriLogger<int>();
        var objectLogger = new SeriLogger<object>();

        stringLogger.Should().NotBeNull();
        intLogger.Should().NotBeNull();
        objectLogger.Should().NotBeNull();
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

    #endregion

    #region Parameter Handling Tests

    [Fact]
    public void LogDebug_WithNoParameters_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogDebug("Message without parameters");

        act.Should().NotThrow();
    }

    [Fact]
    public void LogDebug_WithNullParameters_ShouldNotThrow()
    {
        Action act = () => seriLogger.LogDebug("Message with {Param}", (object?)null!);

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

    #endregion

    #region Type Context Tests

    [Fact]
    public void Constructor_WithDifferentGenericTypes_ShouldCreateDifferentInstances()
    {
        var stringLogger = new SeriLogger<string>();
        var intLogger = new SeriLogger<int>();
        var objectLogger = new SeriLogger<object>();

        stringLogger.Should().NotBeSameAs(intLogger);
        stringLogger.Should().NotBeSameAs(objectLogger);
        intLogger.Should().NotBeSameAs(objectLogger);
    }

    [Fact]
    public void LogDebug_WithTypeSpecificContext_ShouldNotThrow()
    {
        var userServiceLogger = new SeriLogger<UserService>();
        var orderServiceLogger = new SeriLogger<OrderService>();

        Action act1 = () => userServiceLogger.LogDebug("User service debug message");
        Action act2 = () => orderServiceLogger.LogDebug("Order service debug message");

        act1.Should().NotThrow();
        act2.Should().NotThrow();
    }

    #endregion

    #region Interface Implementation Tests

    [Fact]
    public void SeriLogger_ShouldImplementILogManager()
    {
        var logger = new SeriLogger<TestService>();
        
        logger.Should().BeAssignableTo<ILogManager<TestService>>();
    }

    [Fact]
    public void SeriLogger_ShouldImplementIService()
    {
        var logger = new SeriLogger<TestService>();
        
        logger.Should().BeAssignableTo<IService>();
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

    #endregion

    #region Test Classes

    public class TestService
    {
        public string Name { get; set; } = string.Empty;
    }

    public class UserService
    {
        public void DoSomething() { }
    }

    public class OrderService
    {
        public void DoSomething() { }
    }

    public class CustomTestException : Exception
    {
        public CustomTestException(string message) : base(message) { }
        public CustomTestException(string message, Exception innerException) : base(message, innerException) { }
    }

    #endregion
}
