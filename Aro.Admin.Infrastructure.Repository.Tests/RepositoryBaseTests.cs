using Aro.Admin.Domain.Entities;
using Aro.Admin.Infrastructure.Repository.Context;
using Aro.Admin.Infrastructure.Repository.Repositories;
using Aro.Admin.Tests.Common;
using Aro.Common.Domain.Entities;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Aro.Admin.Infrastructure.Repository.Tests;

public class RepositoryBaseTests : TestBase
{
    private AroAdminApiDbContext dbContext;
    private TestRepository repository;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public RepositoryBaseTests()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
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
        repository = new TestRepository(dbContext);
    }

    [Fact]
    public async Task Add_ShouldMarkEntityForAddition()
    {
        var user = fixture.Create<User>();

        await repository.Add(user, CancellationToken.None);

        dbContext.Entry(user).State.Should().Be(EntityState.Added);
    }

    [Fact]
    public void Delete_ShouldMarkEntityForDeletion()
    {
        var user = fixture.Create<User>();
        dbContext.Users.Add(user);
        dbContext.SaveChanges();

        repository.Delete(user);

        dbContext.Entry(user).State.Should().Be(EntityState.Deleted);
    }

    [Fact]
    public void DeleteRange_ShouldMarkMultipleEntitiesForDeletion()
    {
        var users = fixture.CreateMany<User>(3).ToList();
        dbContext.Users.AddRange(users);
        dbContext.SaveChanges();

        repository.DeleteRange(users);

        foreach (var user in users)
        {
            dbContext.Entry(user).State.Should().Be(EntityState.Deleted);
        }
    }

    [Fact]
    public void Update_ShouldMarkEntityForUpdate()
    {
        var user = fixture.Create<User>();
        dbContext.Users.Add(user);

        repository.Update(user);

        dbContext.Entry(user).State.Should().Be(EntityState.Modified);
    }

    [Fact]
    public void FindByCondition_WithoutFilter_ShouldReturnAllEntities()
    {
        var users = fixture.CreateMany<User>(3).ToList();
        dbContext.Users.AddRange(users);
        dbContext.SaveChanges();

        var result = repository.FindByCondition();

        result.Should().HaveCount(3);
        result.Should().ContainEquivalentOf(users[0]);
        result.Should().ContainEquivalentOf(users[1]);
        result.Should().ContainEquivalentOf(users[2]);
    }

    [Fact]
    public void FindByCondition_WithFilter_ShouldReturnFilteredEntities()
    {
        var testEmail = fixture.Create<string>();
        var otherEmail = fixture.Create<string>();
        
        var users = new[]
        {
            fixture.Build<User>().With(u => u.Email, testEmail).Create(),
            fixture.Build<User>().With(u => u.Email, otherEmail).Create(),
            fixture.Build<User>().With(u => u.Email, testEmail).Create()
        };
        
        dbContext.Users.AddRange(users);
        dbContext.SaveChanges();

        var result = repository.FindByCondition(u => u.Email == testEmail);

        result.Should().HaveCount(2);
        result.Should().OnlyContain(u => u.Email == testEmail);
    }

    [Fact]
    public void FindByCondition_WithOrderBy_ShouldReturnOrderedEntities()
    {
        var emails = fixture.CreateMany<string>(3).OrderBy(e => e).ToList();
        
        var users = new[]
        {
            fixture.Build<User>().With(u => u.Email, emails[0]).Create(),
            fixture.Build<User>().With(u => u.Email, emails[1]).Create(),
            fixture.Build<User>().With(u => u.Email, emails[2]).Create()
        };
        
        dbContext.Users.AddRange(users);
        dbContext.SaveChanges();

        var result = repository.FindByCondition(orderBy: q => q.OrderBy(u => u.Email));

        result.Should().HaveCount(3);
        result.First().Email.Should().Be(emails[0]);
        result.Last().Email.Should().Be(emails[2]);
    }

    [Fact]
    public void FindByCondition_WithInclude_ShouldIncludeRelatedEntities()
    {
        var user = fixture.Create<User>();
        user.UserRoles = fixture.CreateMany<UserRole>(2).ToList();
        dbContext.Users.Add(user);
        dbContext.SaveChanges();

        var result = repository.FindByCondition(include: q => q.Include(u => u.UserRoles));

        result.Should().HaveCount(1);
        var foundUser = result.First();
        foundUser.UserRoles.Should().HaveCount(2);
    }

    [Fact]
    public void FindByCondition_WithTrackChangesFalse_ShouldReturnNoTrackingEntities()
    {
        var user = fixture.Create<User>();
        dbContext.Users.Add(user);
        dbContext.SaveChanges();

        var result = repository.FindByCondition(trackChanges: false);

        result.Should().HaveCount(1);
        dbContext.Entry(result.First()).State.Should().Be(EntityState.Detached);
    }

    [Fact]
    public void FindByCondition_WithAllParameters_ShouldApplyAllFilters()
    {
        var testEmail = fixture.Create<string>();
        var otherEmail = fixture.Create<string>();
        
        var users = new[]
        {
            fixture.Build<User>().With(u => u.Email, testEmail).With(u => u.IsActive, true).Create(),
            fixture.Build<User>().With(u => u.Email, testEmail).With(u => u.IsActive, false).Create(),
            fixture.Build<User>().With(u => u.Email, otherEmail).With(u => u.IsActive, true).Create(),
            fixture.Build<User>().With(u => u.Email, testEmail).With(u => u.IsActive, true).Create()
        };
        
        dbContext.Users.AddRange(users);
        dbContext.SaveChanges();

        var result = repository.FindByCondition(
            filter: u => u.Email == testEmail && u.IsActive,
            orderBy: q => q.OrderBy(u => u.Email),
            trackChanges: false
        );

        result.Should().HaveCount(2);
        result.Should().OnlyContain(u => u.Email == testEmail && u.IsActive);
        result.First().Email.Should().Be(testEmail);
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