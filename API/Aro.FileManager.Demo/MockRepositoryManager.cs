using Aro.Common.Application.Repository;
using Aro.Common.Domain.Entities;

namespace Aro.FileManager.Demo;

/// <summary>
/// Mock implementation of IRepositoryManager for demonstration purposes
/// </summary>
public class MockRepositoryManager : IRepositoryManager
{
    private readonly MockFileResourceRepository _fileResourceRepository = new();

    public IAuditTrailRepository AuditTrailRepository => throw new NotImplementedException();
    public IUserRepository UserRepository => throw new NotImplementedException();
    public IPermissionRepository PermissionRepository => throw new NotImplementedException();
    public IRolePermissionRepository RolePermissionRepository => throw new NotImplementedException();
    public IRoleRepository RoleRepository => throw new NotImplementedException();
    public IUserRoleRepository UserRoleRepository => throw new NotImplementedException();
    public IFileResourceRepository FileResourceRepository => _fileResourceRepository;

    public ICountryRepository CountryRepository => throw new NotImplementedException();
}

/// <summary>
/// Mock implementation of IFileResourceRepository for demonstration purposes
/// </summary>
public class MockFileResourceRepository : IFileResourceRepository
{
    private readonly List<FileResource> _storage = new();

    public IQueryable<FileResource> GetById(Guid id)
    {
        return _storage.Where(f => f.Id == id).AsQueryable();
    }

    public IQueryable<FileResource> GetByUri(string uri)
    {
        return _storage.Where(f => f.Uri == uri).AsQueryable();
    }

    public IQueryable<FileResource> GetAll()
    {
        return _storage.AsQueryable();
    }

    public Task Create(FileResource fileResource, CancellationToken cancellationToken = default)
    {
        _storage.Add(fileResource);
        return Task.CompletedTask;
    }

    public void Update(FileResource fileResource)
    {
        var existing = _storage.FirstOrDefault(f => f.Id == fileResource.Id);
        if (existing != null)
        {
            existing.Name = fileResource.Name;
            existing.Uri = fileResource.Uri;
            existing.Description = fileResource.Description;
            existing.Metadata = fileResource.Metadata;
            existing.UpdatedOn = fileResource.UpdatedOn;
        }
    }

    public void Delete(FileResource fileResource)
    {
        _storage.Remove(fileResource);
    }
}

