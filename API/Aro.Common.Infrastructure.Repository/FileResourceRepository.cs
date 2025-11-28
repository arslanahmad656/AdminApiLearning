using Aro.Common.Application.Repository;
using Aro.Common.Domain.Entities;
using Aro.Common.Infrastructure.Repository.Context;

namespace Aro.Common.Infrastructure.Repository;

public class FileResourceRepository(AroDbContext dbContext) : RepositoryBase<FileResource>(dbContext), IFileResourceRepository
{
    public IQueryable<FileResource> GetById(Guid id) => FindByCondition(filter: f => f.Id == id);

    public IQueryable<FileResource> GetByUri(string uri) => FindByCondition(filter: f => f.Uri == uri);

    public IQueryable<FileResource> GetAll() => FindByCondition();

    public Task Create(FileResource fileResource, CancellationToken cancellationToken = default) => Add(fileResource, cancellationToken);

    public new void Update(FileResource fileResource) => base.Update(fileResource);

    public new void Delete(FileResource fileResource) => base.Delete(fileResource);
}

