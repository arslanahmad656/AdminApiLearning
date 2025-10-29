using Aro.Admin.Domain.Shared.Extensions;
using Aro.Admin.Tests.Common;
using AutoFixture;
using FluentAssertions;
using Xunit;

namespace Aro.Admin.Domain.Shared.Tests;

public class EnumerableExtensionsTests : TestBase
{
    [Fact]
    public void Merge_WhenBothCollectionsAreNull_ShouldReturnEmptyCollection()
    {
        IEnumerable<string>? collection1 = null;
        IEnumerable<string>? collection2 = null;

        var result = collection1.Merge(collection2);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public void Merge_WhenFirstCollectionIsNull_ShouldReturnSecondCollection()
    {
        IEnumerable<string>? collection1 = null;
        var collection2 = fixture.CreateMany<string>(3).ToArray();

        var result = collection1.Merge(collection2);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(collection2);
        result.Should().HaveCount(3);
    }

    [Fact]
    public void Merge_WhenSecondCollectionIsNull_ShouldReturnFirstCollection()
    {
        var collection1 = fixture.CreateMany<string>(3).ToArray();
        IEnumerable<string>? collection2 = null;

        var result = collection1.Merge(collection2);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(collection1);
        result.Should().HaveCount(3);
    }

    [Fact]
    public void Merge_WhenBothCollectionsAreValid_ShouldReturnConcatenatedCollection()
    {
        var collection1 = fixture.CreateMany<string>(2).ToArray();
        var collection2 = fixture.CreateMany<string>(2).ToArray();

        var result = collection1.Merge(collection2);

        result.Should().NotBeNull();
        result.Should().HaveCount(4);
        result.Should().BeEquivalentTo(collection1.Concat(collection2));
    }

    [Fact]
    public void Merge_WhenFirstCollectionIsEmpty_ShouldReturnSecondCollection()
    {
        var collection1 = new string[0];
        var collection2 = fixture.CreateMany<string>(2).ToArray();

        var result = collection1.Merge(collection2);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(collection2);
        result.Should().HaveCount(2);
    }

    [Fact]
    public void Merge_WhenSecondCollectionIsEmpty_ShouldReturnFirstCollection()
    {
        var collection1 = fixture.CreateMany<string>(2).ToArray();
        var collection2 = new string[0];

        var result = collection1.Merge(collection2);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(collection1);
        result.Should().HaveCount(2);
    }

    [Fact]
    public void Merge_WhenBothCollectionsAreEmpty_ShouldReturnEmptyCollection()
    {
        var collection1 = new string[0];
        var collection2 = new string[0];

        var result = collection1.Merge(collection2);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public void Merge_WithParamsArray_WhenFirstCollectionIsNull_ShouldReturnParamsArray()
    {
        IEnumerable<string>? collection1 = null;
        var paramsArray = fixture.CreateMany<string>(3).ToArray();

        var result = collection1.Merge(paramsArray);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(paramsArray);
        result.Should().HaveCount(3);
    }

    [Fact]
    public void Merge_WithParamsArray_WhenFirstCollectionIsValid_ShouldReturnConcatenatedCollection()
    {
        var collection1 = fixture.CreateMany<string>(2).ToArray();
        var paramsArray = fixture.CreateMany<string>(2).ToArray();

        var result = collection1.Merge(paramsArray);

        result.Should().NotBeNull();
        result.Should().HaveCount(4);
        result.Should().BeEquivalentTo(collection1.Concat(paramsArray));
    }

    [Fact]
    public void Merge_WithParamsArray_WhenParamsArrayIsEmpty_ShouldReturnFirstCollection()
    {
        var collection1 = fixture.CreateMany<string>(2).ToArray();
        var paramsArray = new string[0];

        var result = collection1.Merge(paramsArray);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(collection1);
        result.Should().HaveCount(2);
    }

    [Fact]
    public void Merge_WithParamsArray_WhenFirstCollectionIsEmpty_ShouldReturnParamsArray()
    {
        var collection1 = new string[0];
        var paramsArray = fixture.CreateMany<string>(2).ToArray();

        var result = collection1.Merge(paramsArray);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(paramsArray);
        result.Should().HaveCount(2);
    }

    [Fact]
    public void Merge_WithParamsArray_WhenBothAreEmpty_ShouldReturnEmptyCollection()
    {
        var collection1 = new string[0];
        var paramsArray = new string[0];

        var result = collection1.Merge(paramsArray);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public void Merge_WithDifferentTypes_ShouldWorkCorrectly()
    {
        var collection1 = fixture.CreateMany<int>(3).ToArray();
        var collection2 = fixture.CreateMany<int>(3).ToArray();

        var result = collection1.Merge(collection2);

        result.Should().NotBeNull();
        result.Should().HaveCount(6);
        result.Should().BeEquivalentTo(collection1.Concat(collection2));
    }

    [Fact]
    public void Merge_WithComplexObjects_ShouldWorkCorrectly()
    {
        var person1 = new { Name = fixture.Create<string>(), Age = fixture.Create<int>() };
        var person2 = new { Name = fixture.Create<string>(), Age = fixture.Create<int>() };
        var collection1 = new[] { person1 };
        var collection2 = new[] { person2 };

        var result = collection1.Merge(collection2);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(person1);
        result.Should().Contain(person2);
    }

    [Fact]
    public void Merge_WithLargeCollections_ShouldWorkCorrectly()
    {
        var collection1 = fixture.CreateMany<int>(1000).ToArray();
        var collection2 = fixture.CreateMany<int>(1000).ToArray();

        var result = collection1.Merge(collection2);

        result.Should().NotBeNull();
        result.Should().HaveCount(2000);
        result.Should().BeEquivalentTo(collection1.Concat(collection2));
    }

    [Fact]
    public void Merge_WithNullParamsArray_ShouldReturnFirstCollection()
    {
        var collection1 = fixture.CreateMany<string>(2).ToArray();
        string[]? paramsArray = null;

        var result = collection1.Merge(paramsArray!);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(collection1);
        result.Should().HaveCount(2);
    }

    [Fact]
    public void Merge_WithNullFirstCollectionAndNullParamsArray_ShouldReturnEmptyCollection()
    {
        IEnumerable<string>? collection1 = null;
        string[]? paramsArray = null;

        var result = collection1.Merge(paramsArray!);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public void Merge_WithSingleItemCollections_ShouldWorkCorrectly()
    {
        var collection1 = new[] { fixture.Create<string>() };
        var collection2 = new[] { fixture.Create<string>() };

        var result = collection1.Merge(collection2);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(collection1.Concat(collection2));
    }

    [Fact]
    public void Merge_WithDuplicateItems_ShouldPreserveDuplicates()
    {
        var duplicateItem = fixture.Create<string>();
        var collection1 = new[] { fixture.Create<string>(), duplicateItem };
        var collection2 = new[] { duplicateItem, fixture.Create<string>() };

        var result = collection1.Merge(collection2);

        result.Should().NotBeNull();
        result.Should().HaveCount(4);
        result.Should().BeEquivalentTo(collection1.Concat(collection2));
    }

    [Fact]
    public void Merge_WithNullItemsInCollection_ShouldPreserveNullItems()
    {
        var collection1 = new[] { fixture.Create<string>(), null, fixture.Create<string>() };
        var collection2 = new[] { fixture.Create<string>(), null, fixture.Create<string>() };

        var result = collection1.Merge(collection2);

        result.Should().NotBeNull();
        result.Should().HaveCount(6);
        result.Should().BeEquivalentTo(collection1.Concat(collection2));
    }
}