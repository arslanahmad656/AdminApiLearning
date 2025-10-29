using Aro.Admin.Application.Services;
using Aro.Admin.Infrastructure.Services;
using Aro.Admin.Tests.Common;
using FluentAssertions;
using Moq;

namespace Aro.Admin.Infrastructure.Services.Tests;

public class GuidGeneratorTests : TestBase
{
    private readonly Mock<ILogManager<GuidGenerator>> mockLogger;
    private readonly GuidGenerator guidGenerator;

    public GuidGeneratorTests()
    {
        mockLogger = new Mock<ILogManager<GuidGenerator>>();
        guidGenerator = new GuidGenerator(mockLogger.Object);
    }

    [Fact]
    public void Generate_ShouldReturnValidGuid()
    {
        var result = guidGenerator.Generate();

        result.Should().NotBeEmpty();
        result.Should().NotBe(Guid.Empty);
        Guid.TryParse(result.ToString(), out _).Should().BeTrue();
    }

    [Fact]
    public void Generate_ShouldReturnUniqueGuids()
    {
        var guid1 = guidGenerator.Generate();
        var guid2 = guidGenerator.Generate();
        var guid3 = guidGenerator.Generate();

        guid1.Should().NotBe(guid2);
        guid1.Should().NotBe(guid3);
        guid2.Should().NotBe(guid3);
    }

    [Fact]
    public void Generate_ShouldLogDebugMessages()
    {
        guidGenerator.Generate();

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "Generate"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Generated GUID: {Guid}", It.IsAny<Guid>()), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "Generate"), Times.Once);
    }

    [Fact]
    public void Generate_ShouldLogCorrectGuid()
    {
        var result = guidGenerator.Generate();

        mockLogger.Verify(x => x.LogDebug("Generated GUID: {Guid}", result), Times.Once);
    }

    [Fact]
    public void Generate_MultipleCalls_ShouldLogEachCall()
    {
        guidGenerator.Generate();
        guidGenerator.Generate();
        guidGenerator.Generate();

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "Generate"), Times.Exactly(3));
        mockLogger.Verify(x => x.LogDebug("Generated GUID: {Guid}", It.IsAny<Guid>()), Times.Exactly(3));
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "Generate"), Times.Exactly(3));
    }

    [Fact]
    public void Generate_ShouldReturnGuidWithCorrectFormat()
    {
        var result = guidGenerator.Generate();

        result.ToString().Should().MatchRegex(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$");
    }

    [Fact]
    public void Generate_ShouldReturnVersion4Guid()
    {
        var result = guidGenerator.Generate();

        result.ToString().Should().MatchRegex(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-4[0-9a-fA-F]{3}-[89abAB][0-9a-fA-F]{3}-[0-9a-fA-F]{12}$");
    }

    [Fact]
    public void Generate_ShouldThrowWhenLoggerThrows()
    {
        mockLogger.Setup(x => x.LogDebug(It.IsAny<string>(), It.IsAny<object[]>()))
            .Throws(new InvalidOperationException("Logger failure"));

        Action act = () => guidGenerator.Generate();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Logger failure");
    }

    [Fact]
    public void Generate_ShouldThrowWhenLoggerHasNullReference()
    {
        mockLogger.Setup(x => x.LogDebug(It.IsAny<string>(), It.IsAny<object[]>()))
            .Throws(new NullReferenceException("Logger null reference"));

        Action act = () => guidGenerator.Generate();

        act.Should().Throw<NullReferenceException>()
            .WithMessage("Logger null reference");
    }

    [Fact]
    public void Generate_ShouldThrowOnEachCallWhenLoggerFails()
    {
        mockLogger.Setup(x => x.LogDebug(It.IsAny<string>(), It.IsAny<object[]>()))
            .Throws(new Exception("Logger failure"));

        Action act1 = () => guidGenerator.Generate();
        Action act2 = () => guidGenerator.Generate();

        act1.Should().Throw<Exception>()
            .WithMessage("Logger failure");
        act2.Should().Throw<Exception>()
            .WithMessage("Logger failure");
    }


    [Fact]
    public void Generate_ShouldLogWithCorrectMethodName()
    {
        guidGenerator.Generate();

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "Generate"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "Generate"), Times.Once);
    }

    [Fact]
    public void Generate_ShouldNotThrowException()
    {
        Action act = () => guidGenerator.Generate();

        act.Should().NotThrow();
    }
}
