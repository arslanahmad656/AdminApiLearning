using Aro.Admin.Domain.Entities;
using Aro.Admin.Domain.Repository;
using Aro.Admin.Infrastructure.Repository.Context;
using Aro.Admin.Infrastructure.Repository.Repositories;
using Aro.Admin.Tests.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Aro.Admin.Infrastructure.Repository.Tests;

public class RepositoryManagerTests : TestBase
{
    private AroAdminApiDbContext dbContext = null!;
    private RepositoryManager repositoryManager = null!;

    public RepositoryManagerTests()
    {
        SetupDatabase();
    }

    private void SetupDatabase()
    {
        var services = new ServiceCollection()
            .AddDbContext<AroAdminApiDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()))
            .BuildServiceProvider();

        dbContext = services.GetRequiredService<AroAdminApiDbContext>();
        repositoryManager = new RepositoryManager(dbContext);
    }

    [Fact]
    public void Constructor_WithValidDbContext_ShouldCreateInstance()
    {
        var services = new ServiceCollection()
            .AddDbContext<AroAdminApiDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()))
            .BuildServiceProvider();

        var context = services.GetRequiredService<AroAdminApiDbContext>();

        var manager = new RepositoryManager(context);

        manager.Should().NotBeNull();
    }


    [Fact]
    public void AuditTrailRepository_ShouldReturnCorrectInstance()
    {
        var repository = repositoryManager.AuditTrailRepository;

        repository.Should().NotBeNull();
        repository.Should().BeOfType<AuditTrailRepository>();
    }

    [Fact]
    public void IIdempotencyRecordRepository_ShouldReturnCorrectInstance()
    {
        var repository = repositoryManager.IIdempotencyRecordRepository;

        repository.Should().NotBeNull();
        repository.Should().BeAssignableTo<IIdempotencyRecordRepository>();
    }

    [Fact]
    public void PermissionRepository_ShouldReturnCorrectInstance()
    {
        var repository = repositoryManager.PermissionRepository;

        repository.Should().NotBeNull();
        repository.Should().BeOfType<PermissionRepository>();
    }

    [Fact]
    public void RolePermissionRepository_ShouldReturnCorrectInstance()
    {
        var repository = repositoryManager.RolePermissionRepository;

        repository.Should().NotBeNull();
        repository.Should().BeOfType<RolePermissionRepository>();
    }

    [Fact]
    public void RoleRepository_ShouldReturnCorrectInstance()
    {
        var repository = repositoryManager.RoleRepository;

        repository.Should().NotBeNull();
        repository.Should().BeOfType<RoleRepository>();
    }

    [Fact]
    public void SystemSettingsRepository_ShouldReturnCorrectInstance()
    {
        var repository = repositoryManager.SystemSettingsRepository;

        repository.Should().NotBeNull();
        repository.Should().BeOfType<SystemSettingsRepository>();
    }

    [Fact]
    public void UserRepository_ShouldReturnCorrectInstance()
    {
        var repository = repositoryManager.UserRepository;

        repository.Should().NotBeNull();
        repository.Should().BeOfType<UserRepository>();
    }

    [Fact]
    public void UserRoleRepository_ShouldReturnCorrectInstance()
    {
        var repository = repositoryManager.UserRoleRepository;

        repository.Should().NotBeNull();
        repository.Should().BeOfType<UserRoleRepository>();
    }

    [Fact]
    public void RefreshTokenRepository_ShouldReturnCorrectInstance()
    {
        var repository = repositoryManager.RefreshTokenRepository;

        repository.Should().NotBeNull();
        repository.Should().BeAssignableTo<IRefreshTokenRepository>();
    }

    [Fact]
    public void PasswordResetTokenRepository_ShouldReturnCorrectInstance()
    {
        var repository = repositoryManager.PasswordResetTokenRepository;

        repository.Should().NotBeNull();
        repository.Should().BeAssignableTo<IPasswordResetTokenRepository>();
    }

    [Fact]
    public void Repositories_ShouldBeLazyLoaded()
    {
        var auditTrail = repositoryManager.AuditTrailRepository;
        var user = repositoryManager.UserRepository;
        var role = repositoryManager.RoleRepository;

        auditTrail.Should().NotBeNull();
        user.Should().NotBeNull();
        role.Should().NotBeNull();
    }

    [Fact]
    public void AuditTrailRepository_MultipleCalls_ShouldReturnSameInstance()
    {
        var repository1 = repositoryManager.AuditTrailRepository;
        var repository2 = repositoryManager.AuditTrailRepository;

        repository1.Should().BeSameAs(repository2);
    }

    [Fact]
    public void IIdempotencyRecordRepository_MultipleCalls_ShouldReturnSameInstance()
    {
        var repository1 = repositoryManager.IIdempotencyRecordRepository;
        var repository2 = repositoryManager.IIdempotencyRecordRepository;

        repository1.Should().BeSameAs(repository2);
    }

    [Fact]
    public void PermissionRepository_MultipleCalls_ShouldReturnSameInstance()
    {
        var repository1 = repositoryManager.PermissionRepository;
        var repository2 = repositoryManager.PermissionRepository;

        repository1.Should().BeSameAs(repository2);
    }

    [Fact]
    public void RolePermissionRepository_MultipleCalls_ShouldReturnSameInstance()
    {
        var repository1 = repositoryManager.RolePermissionRepository;
        var repository2 = repositoryManager.RolePermissionRepository;

        repository1.Should().BeSameAs(repository2);
    }

    [Fact]
    public void RoleRepository_MultipleCalls_ShouldReturnSameInstance()
    {
        var repository1 = repositoryManager.RoleRepository;
        var repository2 = repositoryManager.RoleRepository;

        repository1.Should().BeSameAs(repository2);
    }

    [Fact]
    public void SystemSettingsRepository_MultipleCalls_ShouldReturnSameInstance()
    {
        var repository1 = repositoryManager.SystemSettingsRepository;
        var repository2 = repositoryManager.SystemSettingsRepository;

        repository1.Should().BeSameAs(repository2);
    }

    [Fact]
    public void UserRepository_MultipleCalls_ShouldReturnSameInstance()
    {
        var repository1 = repositoryManager.UserRepository;
        var repository2 = repositoryManager.UserRepository;

        repository1.Should().BeSameAs(repository2);
    }

    [Fact]
    public void UserRoleRepository_MultipleCalls_ShouldReturnSameInstance()
    {
        var repository1 = repositoryManager.UserRoleRepository;
        var repository2 = repositoryManager.UserRoleRepository;

        repository1.Should().BeSameAs(repository2);
    }

    [Fact]
    public void RefreshTokenRepository_MultipleCalls_ShouldReturnSameInstance()
    {
        var repository1 = repositoryManager.RefreshTokenRepository;
        var repository2 = repositoryManager.RefreshTokenRepository;

        repository1.Should().BeSameAs(repository2);
    }

    [Fact]
    public void PasswordResetTokenRepository_MultipleCalls_ShouldReturnSameInstance()
    {
        var repository1 = repositoryManager.PasswordResetTokenRepository;
        var repository2 = repositoryManager.PasswordResetTokenRepository;

        repository1.Should().BeSameAs(repository2);
    }

    [Fact]
    public async Task SaveChanges_WithDefaultCancellationToken_ShouldCallDbContextSaveChangesAsync()
    {
        var user = new User { Id = Guid.NewGuid(), Email = "test@example.com", DisplayName = "Test User", PasswordHash = "hashed-password" };
        dbContext.Users.Add(user);

        await repositoryManager.SaveChanges();

        dbContext.Users.Should().Contain(user);
    }

    [Fact]
    public async Task SaveChanges_WithCancellationToken_ShouldCallDbContextSaveChangesAsync()
    {
        var user = new User { Id = Guid.NewGuid(), Email = "test@example.com", DisplayName = "Test User", PasswordHash = "hashed-password" };
        dbContext.Users.Add(user);
        var cancellationToken = new CancellationToken();

        await repositoryManager.SaveChanges(cancellationToken);

        dbContext.Users.Should().Contain(user);
    }

    [Fact]
    public async Task SaveChanges_WithNoChanges_ShouldNotThrow()
    {
        await repositoryManager.SaveChanges();
    }

    [Fact]
    public async Task SaveChanges_MultipleCalls_ShouldWorkCorrectly()
    {
        var user1 = new User { Id = Guid.NewGuid(), Email = "test1@example.com", DisplayName = "Test1 User", PasswordHash = "hashed-password-1" };
        var user2 = new User { Id = Guid.NewGuid(), Email = "test2@example.com", DisplayName = "Test2 User", PasswordHash = "hashed-password-2" };
        dbContext.Users.Add(user1);

        await repositoryManager.SaveChanges();
        dbContext.Users.Add(user2);
        await repositoryManager.SaveChanges();

        dbContext.Users.Should().HaveCount(2);
    }

    [Fact]
    public async Task RepositoryManager_ShouldWorkAsUnitOfWork()
    {
        var user = new User { Id = Guid.NewGuid(), Email = "test@example.com", DisplayName = "Test User", PasswordHash = "hashed-password" };
        var role = new Role { Id = Guid.NewGuid(), Name = "Test Role" };
        
        dbContext.Users.Add(user);
        dbContext.Roles.Add(role);
        
        await repositoryManager.SaveChanges();

        dbContext.Users.Should().Contain(user);
        dbContext.Roles.Should().Contain(role);
    }

    [Fact]
    public async Task RepositoryManager_WithTransaction_ShouldWorkCorrectly()
    {
        var user = new User { Id = Guid.NewGuid(), Email = "test@example.com", DisplayName = "Test User", PasswordHash = "hashed-password" };
        var role = new Role { Id = Guid.NewGuid(), Name = "Test Role" };
        
        dbContext.Users.Add(user);
        dbContext.Roles.Add(role);
        
        await repositoryManager.SaveChanges();

        dbContext.Users.Should().Contain(user);
        dbContext.Roles.Should().Contain(role);
    }

    [Fact]
    public void RepositoryManager_WithDisposedContext_ShouldHandleGracefully()
    {
        dbContext.Dispose();

        Action act = () => _ = repositoryManager.AuditTrailRepository;
        act.Should().Throw<ObjectDisposedException>();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            dbContext?.Dispose();
        }
        base.Dispose(disposing);
    }
}