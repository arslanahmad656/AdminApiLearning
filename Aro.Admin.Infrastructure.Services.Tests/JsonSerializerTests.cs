using Aro.Admin.Application.Services;
using Aro.Admin.Infrastructure.Services;
using Aro.Admin.Tests.Common;
using AutoFixture;
using FluentAssertions;
using Moq;
using System.Text.Json;

namespace Aro.Admin.Infrastructure.Services.Tests;

public class JsonSerializerTests : TestBase
{
    private readonly Mock<ILogManager<JsonSerializer>> mockLogger;
    private readonly JsonSerializer serializer;

    public JsonSerializerTests()
    {
        mockLogger = new Mock<ILogManager<JsonSerializer>>();
        serializer = new JsonSerializer(mockLogger.Object);
    }

    #region Serialize Tests

    [Fact]
    public void Serialize_WithValidObject_ShouldReturnJsonString()
    {
        var testObject = new TestClass
        {
            Id = 1,
            Name = "Test",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = serializer.Serialize(testObject);

        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("\"Id\":1");
        result.Should().Contain("\"Name\":\"Test\"");
        result.Should().Contain("\"IsActive\":true");
    }

    [Fact]
    public void Serialize_WithPrettyFormat_ShouldReturnIndentedJson()
    {
        var testObject = new TestClass
        {
            Id = 1,
            Name = "Test",
            IsActive = true
        };

        var result = serializer.Serialize(testObject, pretty: true);

        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("\n");
        result.Should().Contain("  ");
    }

    [Fact]
    public void Serialize_WithDefaultFormat_ShouldReturnCompactJson()
    {
        var testObject = new TestClass
        {
            Id = 1,
            Name = "Test",
            IsActive = true
        };

        var result = serializer.Serialize(testObject, pretty: false);

        result.Should().NotBeNullOrEmpty();
        result.Should().NotContain("\n");
    }

    [Fact]
    public void Serialize_WithNullObject_ShouldThrowArgumentNullException()
    {
        Action act = () => serializer.Serialize<object>(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("obj");
    }

    [Fact]
    public void Serialize_WithNullObject_ShouldLogWarning()
    {
        try
        {
            serializer.Serialize<object>(null!);
        }
        catch (ArgumentNullException)
        {
        }

        mockLogger.Verify(x => x.LogWarn("Attempted to serialize null object"), Times.Once);
    }

    [Fact]
    public void Serialize_WithComplexObject_ShouldSerializeCorrectly()
    {
        var complexObject = new ComplexTestClass
        {
            Id = 1,
            Name = "Complex Test",
            NestedObject = new TestClass
            {
                Id = 2,
                Name = "Nested",
                IsActive = true
            },
            Items = new List<string> { "item1", "item2", "item3" },
            Tags = new Dictionary<string, string>
            {
                { "tag1", "value1" },
                { "tag2", "value2" }
            }
        };

        var result = serializer.Serialize(complexObject);

        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("\"Id\":1");
        result.Should().Contain("\"Name\":\"Complex Test\"");
        result.Should().Contain("\"NestedObject\"");
        result.Should().Contain("\"Items\"");
        result.Should().Contain("\"Tags\"");
    }

    [Fact]
    public void Serialize_WithSpecialCharacters_ShouldEscapeCorrectly()
    {
        var testObject = new TestClass
        {
            Id = 1,
            Name = "Test with \"quotes\" and \n newlines and \t tabs",
            IsActive = true
        };

        var result = serializer.Serialize(testObject);

        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("\\\"quotes\\\"");
        result.Should().Contain("\\n");
        result.Should().Contain("\\t");
    }

    [Fact]
    public void Serialize_ShouldLogDebugMessages()
    {
        var testObject = new TestClass { Id = 1, Name = "Test" };

        serializer.Serialize(testObject);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "Serialize"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Using {OptionsType} options for serialization", "default"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Object serialized successfully, resultLength: {Length}", It.IsAny<int>()), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "Serialize"), Times.Once);
    }

    [Fact]
    public void Serialize_WithPrettyFormat_ShouldLogCorrectOptionsType()
    {
        var testObject = new TestClass { Id = 1, Name = "Test" };

        serializer.Serialize(testObject, pretty: true);

        mockLogger.Verify(x => x.LogDebug("Using {OptionsType} options for serialization", "pretty"), Times.Once);
    }

    #endregion

    #region Deserialize Tests

    [Fact]
    public void Deserialize_WithValidJson_ShouldReturnObject()
    {
        var json = """{"Id":1,"Name":"Test","IsActive":true,"CreatedAt":"2023-01-01T00:00:00Z"}""";

        var result = serializer.Deserialize<TestClass>(json);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Test");
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Deserialize_WithCaseInsensitivePropertyNames_ShouldWork()
    {
        var json = """{"id":1,"name":"Test","isactive":true}""";

        var result = serializer.Deserialize<TestClass>(json);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Test");
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Deserialize_WithNullJson_ShouldThrowArgumentException()
    {
        Action act = () => serializer.Deserialize<TestClass>(null!);

        act.Should().Throw<ArgumentException>()
            .WithMessage("JSON string cannot be null or empty*")
            .WithParameterName("json");
    }

    [Fact]
    public void Deserialize_WithEmptyJson_ShouldThrowArgumentException()
    {
        Action act = () => serializer.Deserialize<TestClass>("");

        act.Should().Throw<ArgumentException>()
            .WithMessage("JSON string cannot be null or empty*")
            .WithParameterName("json");
    }

    [Fact]
    public void Deserialize_WithWhitespaceJson_ShouldThrowArgumentException()
    {
        Action act = () => serializer.Deserialize<TestClass>("   ");

        act.Should().Throw<ArgumentException>()
            .WithMessage("JSON string cannot be null or empty*")
            .WithParameterName("json");
    }

    [Fact]
    public void Deserialize_WithInvalidJson_ShouldThrowJsonException()
    {
        var invalidJson = """{"Id":1,"Name":"Test","IsActive":}""";

        Action act = () => serializer.Deserialize<TestClass>(invalidJson);

        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void Deserialize_WithComplexJson_ShouldDeserializeCorrectly()
    {
        var json = """{"Id":1,"Name":"Complex Test","NestedObject":{"Id":2,"Name":"Nested","IsActive":true},"Items":["item1","item2","item3"],"Tags":{"tag1":"value1","tag2":"value2"}}""";

        var result = serializer.Deserialize<ComplexTestClass>(json);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Complex Test");
        result.NestedObject.Should().NotBeNull();
        result.NestedObject!.Id.Should().Be(2);
        result.NestedObject.Name.Should().Be("Nested");
        result.Items.Should().HaveCount(3);
        result.Tags.Should().HaveCount(2);
    }

    [Fact]
    public void Deserialize_WithSpecialCharacters_ShouldDeserializeCorrectly()
    {
        var json = """{"Id":1,"Name":"Test with \"quotes\" and \n newlines and \t tabs","IsActive":true}""";

        var result = serializer.Deserialize<TestClass>(json);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Test with \"quotes\" and \n newlines and \t tabs");
    }

    [Fact]
    public void Deserialize_ShouldLogDebugMessages()
    {
        var json = """{"Id":1,"Name":"Test"}""";

        serializer.Deserialize<TestClass>(json);

        mockLogger.Verify(x => x.LogDebug("Starting {MethodName}", "Deserialize"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("JSON deserialized successfully to type: {Type}", "TestClass"), Times.Once);
        mockLogger.Verify(x => x.LogDebug("Completed {MethodName}", "Deserialize"), Times.Once);
    }

    [Fact]
    public void Deserialize_WithNullJson_ShouldLogWarning()
    {
        try
        {
            serializer.Deserialize<TestClass>(null!);
        }
        catch (ArgumentException)
        {
        }

        mockLogger.Verify(x => x.LogWarn("Attempted to deserialize null or empty JSON string"), Times.Once);
    }

    [Fact]
    public void Deserialize_WithEmptyJson_ShouldLogWarning()
    {
        try
        {
            serializer.Deserialize<TestClass>("");
        }
        catch (ArgumentException)
        {
        }

        mockLogger.Verify(x => x.LogWarn("Attempted to deserialize null or empty JSON string"), Times.Once);
    }

    #endregion

    #region Round-trip Tests

    [Fact]
    public void SerializeAndDeserialize_ShouldPreserveData()
    {
        var originalObject = new TestClass
        {
            Id = 42,
            Name = "Round Trip Test",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var json = serializer.Serialize(originalObject);
        var deserializedObject = serializer.Deserialize<TestClass>(json);

        deserializedObject.Should().NotBeNull();
        deserializedObject!.Id.Should().Be(originalObject.Id);
        deserializedObject.Name.Should().Be(originalObject.Name);
        deserializedObject.IsActive.Should().Be(originalObject.IsActive);
        deserializedObject.CreatedAt.Should().BeCloseTo(originalObject.CreatedAt, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void SerializeAndDeserialize_WithComplexObject_ShouldPreserveData()
    {
        var originalObject = new ComplexTestClass
        {
            Id = 1,
            Name = "Complex Round Trip",
            NestedObject = new TestClass
            {
                Id = 2,
                Name = "Nested",
                IsActive = true
            },
            Items = new List<string> { "item1", "item2" },
            Tags = new Dictionary<string, string>
            {
                { "key1", "value1" },
                { "key2", "value2" }
            }
        };

        var json = serializer.Serialize(originalObject);
        var deserializedObject = serializer.Deserialize<ComplexTestClass>(json);

        deserializedObject.Should().NotBeNull();
        deserializedObject!.Id.Should().Be(originalObject.Id);
        deserializedObject.Name.Should().Be(originalObject.Name);
        deserializedObject.NestedObject.Should().NotBeNull();
        deserializedObject.NestedObject!.Id.Should().Be(originalObject.NestedObject.Id);
        deserializedObject.Items.Should().BeEquivalentTo(originalObject.Items);
        deserializedObject.Tags.Should().BeEquivalentTo(originalObject.Tags);
    }

    #endregion

    #region Edge Cases

    [Fact]
    public void Serialize_WithEmptyObject_ShouldSerializeCorrectly()
    {
        var emptyObject = new TestClass();

        var result = serializer.Serialize(emptyObject);

        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("\"Id\":0");
        result.Should().Contain("\"Name\":null");
        result.Should().Contain("\"IsActive\":false");
    }

    [Fact]
    public void Serialize_WithNullValues_ShouldSerializeCorrectly()
    {
        var objectWithNulls = new TestClass
        {
            Id = 1,
            Name = null,
            IsActive = false
        };

        var result = serializer.Serialize(objectWithNulls);

        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("\"Name\":null");
    }

    [Fact]
    public void Deserialize_WithMissingProperties_ShouldUseDefaultValues()
    {
        var json = """{"Id":1}""";

        var result = serializer.Deserialize<TestClass>(json);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().BeNull();
        result.IsActive.Should().BeFalse();
        result.CreatedAt.Should().Be(default(DateTime));
    }

    [Fact]
    public void Serialize_WithVariousDataTypes_ShouldSerializeCorrectly()
    {
        var objectWithVariousTypes = new VariousTypesClass
        {
            IntValue = 42,
            LongValue = 123456789L,
            DoubleValue = 3.14159,
            DecimalValue = 99.99m,
            BoolValue = true,
            CharValue = 'A',
            DateTimeValue = DateTime.UtcNow,
            GuidValue = Guid.NewGuid(),
            EnumValue = TestEnum.Value2
        };

        var result = serializer.Serialize(objectWithVariousTypes);

        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("\"IntValue\":42");
        result.Should().Contain("\"LongValue\":123456789");
        result.Should().Contain("\"DoubleValue\":3.14159");
        result.Should().Contain("\"DecimalValue\":99.99");
        result.Should().Contain("\"BoolValue\":true");
        result.Should().Contain("\"CharValue\":\"A\"");
        result.Should().Contain("\"EnumValue\":1");
    }

    #endregion

    #region Test Classes

    public class TestClass
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ComplexTestClass
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public TestClass? NestedObject { get; set; }
        public List<string> Items { get; set; } = new();
        public Dictionary<string, string> Tags { get; set; } = new();
    }

    public class VariousTypesClass
    {
        public int IntValue { get; set; }
        public long LongValue { get; set; }
        public double DoubleValue { get; set; }
        public decimal DecimalValue { get; set; }
        public bool BoolValue { get; set; }
        public char CharValue { get; set; }
        public DateTime DateTimeValue { get; set; }
        public Guid GuidValue { get; set; }
        public TestEnum EnumValue { get; set; }
    }

    public enum TestEnum
    {
        Value1 = 0,
        Value2 = 1,
        Value3 = 2
    }

    #endregion
}
