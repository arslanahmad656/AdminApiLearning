using Aro.Common.Domain.Entities;

namespace Aro.Common.Application.Repository;

public interface IFileResourceRepository
{
    IQueryable<FileResource> GetById(Guid id);

    IQueryable<FileResource> GetByUri(string uri);

    IQueryable<FileResource> GetAll();

    Task Create(FileResource fileResource, CancellationToken cancellationToken = default);

    void Update(FileResource fileResource);

    void Delete(FileResource fileResource);
}

