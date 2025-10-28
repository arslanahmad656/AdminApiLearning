using Aro.Admin.Application.Services.SystemContext;
using Aro.Admin.Infrastructure.Services.SystemContext;
using Aro.Admin.Tests.Common;
using FluentAssertions;
using Xunit;
using SystemContextClass = Aro.Admin.Infrastructure.Services.SystemContext.SystemContext;

namespace Aro.Admin.Infrastructure.Services.Tests;

public class SystemContextTests : TestBase
{
    [Fact]
    public void Constructor_WhenNoSystemContextExists_ShouldEnableSystemContext()
    {
        using var systemContext = new SystemContextClass();
        
        systemContext.IsEnabled.Should().BeTrue();
    }

    [Fact]
    public void Constructor_WhenSystemContextAlreadyEnabled_ShouldThrowInvalidOperationException()
    {
        using var firstContext = new SystemContextClass();
        
        var action = () => new SystemContextClass();
        
        action.Should().Throw<InvalidOperationException>();;
    }

    [Fact]
    public void IsEnabled_WhenSystemContextIsActive_ShouldReturnTrue()
    {
        using var systemContext = new SystemContextClass();
        
        systemContext.IsEnabled.Should().BeTrue();
    }

    [Fact]
    public void IsEnabled_WhenSystemContextIsDisposed_ShouldReturnFalse()
    {
        var systemContext = new SystemContextClass();
        systemContext.Dispose();
        
        systemContext.IsEnabled.Should().BeFalse();
    }

    [Fact]
    public void Dispose_WhenCalledOnce_ShouldDisableSystemContext()
    {
        var systemContext = new SystemContextClass();
        systemContext.IsEnabled.Should().BeTrue();
        
        systemContext.Dispose();
        
        systemContext.IsEnabled.Should().BeFalse();
    }

    [Fact]
    public void Dispose_WhenCalledMultipleTimes_ShouldNotThrowException()
    {
        var systemContext = new SystemContextClass();
        
        var action = () =>
        {
            systemContext.Dispose();
            systemContext.Dispose();
            systemContext.Dispose();
        };
        
        action.Should().NotThrow();
        systemContext.IsEnabled.Should().BeFalse();
    }

    [Fact]
    public void Dispose_WhenCalledMultipleTimes_ShouldOnlyDisableOnce()
    {
        var systemContext = new SystemContextClass();
        systemContext.IsEnabled.Should().BeTrue();
        
        systemContext.Dispose();
        systemContext.IsEnabled.Should().BeFalse();
        
        systemContext.Dispose();
        systemContext.IsEnabled.Should().BeFalse();
    }

    [Fact]
    public void UsingStatement_ShouldAutomaticallyDisposeAndDisableSystemContext()
    {
        SystemContextClass? contextAfterUsing = null;
        
        using (var systemContext = new SystemContextClass())
        {
            systemContext.IsEnabled.Should().BeTrue();
            contextAfterUsing = systemContext;
        }
        
        contextAfterUsing!.IsEnabled.Should().BeFalse();
    }

    [Fact]
    public async Task MultipleInstancesInDifferentAsyncContexts_ShouldWorkIndependently()
    {
        var task1 = Task.Run(() =>
        {
            using var context1 = new SystemContextClass();
            return context1.IsEnabled;
        });

        var task2 = Task.Run(() =>
        {
            using var context2 = new SystemContextClass();
            return context2.IsEnabled;
        });

        await Task.WhenAll(task1, task2);
        
        task1.Result.Should().BeTrue();
        task2.Result.Should().BeTrue();
    }

    [Fact]
    public void SystemContext_ShouldImplementISystemContext()
    {
        using var systemContext = new SystemContextClass();
        
        systemContext.Should().BeAssignableTo<ISystemContext>();
    }

    [Fact]
    public void SystemContext_ShouldImplementISystemContextEnabler()
    {
        using var systemContext = new SystemContextClass();
        
        systemContext.Should().BeAssignableTo<ISystemContextEnabler>();
    }

    [Fact]
    public void SystemContext_ShouldImplementIDisposable()
    {
        using var systemContext = new SystemContextClass();
        
        systemContext.Should().BeAssignableTo<IDisposable>();
    }

    [Fact]
    public void IsEnabled_ShouldReflectCurrentAsyncLocalState()
    {
        using var systemContext = new SystemContextClass();
        
        systemContext.IsEnabled.Should().BeTrue();
        
        systemContext.Dispose();
        
        systemContext.IsEnabled.Should().BeFalse();
    }

    [Fact]
    public void Constructor_ShouldSetAsyncLocalValueToTrue()
    {
        using var systemContext = new SystemContextClass();
        
        systemContext.IsEnabled.Should().BeTrue();
    }

    [Fact]
    public void Dispose_ShouldSetAsyncLocalValueToFalse()
    {
        var systemContext = new SystemContextClass();
        systemContext.IsEnabled.Should().BeTrue();
        
        systemContext.Dispose();
        
        systemContext.IsEnabled.Should().BeFalse();
    }

    [Fact]
    public void SystemContext_ShouldBeSealed()
    {
        typeof(SystemContextClass).IsSealed.Should().BeTrue();
    }
}