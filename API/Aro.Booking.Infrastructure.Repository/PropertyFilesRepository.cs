using Aro.Booking.Application.Repository;
using Aro.Booking.Domain.Entities;
using Aro.Common.Domain.Entities;
using Aro.Common.Infrastructure.Repository;
using Aro.Common.Infrastructure.Repository.Context;

namespace Aro.Booking.Infrastructure.Repository;

public class PropertyFilesRepository(AroDbContext dbContext) : RepositoryBase<PropertyFiles>(dbContext), IPropertyFilesRepository
{
    private readonly AroDbContext dbContext = dbContext;
    public IQueryable<IPropertyFilesRepository.PropertyFile> GetById(Guid id)
    {
        var dbContext = this.dbContext;
        var query = from pf in dbContext.Set<PropertyFiles>()
                    join p in dbContext.Set<Property>() on pf.PropertyId equals p.Id
                    join f in dbContext.Set<FileResource>() on pf.FileId equals f.Id
                    where pf.Id == id
                    select new IPropertyFilesRepository.PropertyFile(pf, p, f);

        return query;
    }

    public IQueryable<IPropertyFilesRepository.PropertyFile> GetByPropertyId(Guid propertyId)
    {
        var dbContext = this.dbContext;
        var query = from pf in dbContext.Set<PropertyFiles>()
                    join p in dbContext.Set<Property>() on pf.PropertyId equals p.Id
                    join f in dbContext.Set<FileResource>() on pf.FileId equals f.Id
                    where p.Id == propertyId
                    select new IPropertyFilesRepository.PropertyFile(pf, p, f);

        return query;
    }

    public Task Create(PropertyFiles propertyFiles, CancellationToken cancellationToken = default) => base.Add(propertyFiles, cancellationToken);

    public new void Update(PropertyFiles propertyFiles) => base.Update(propertyFiles);

    public new void Delete(PropertyFiles propertyFiles) => base.Delete(propertyFiles);
}