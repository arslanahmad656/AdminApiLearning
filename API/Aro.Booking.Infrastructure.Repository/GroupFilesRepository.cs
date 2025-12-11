using Aro.Booking.Application.Repository;
using Aro.Booking.Domain.Entities;
using Aro.Common.Domain.Entities;
using Aro.Common.Infrastructure.Repository;
using Aro.Common.Infrastructure.Repository.Context;

namespace Aro.Booking.Infrastructure.Repository;

public class GroupFilesRepository(AroDbContext dbContext) : RepositoryBase<GroupFiles>(dbContext), IGroupFilesRepository
{
    private readonly AroDbContext dbContext = dbContext;
    public IQueryable<IGroupFilesRepository.GroupFile> GetById(Guid id)
    {
        var dbContext = this.dbContext;
        var query = from pf in dbContext.Set<GroupFiles>()
                    join p in dbContext.Set<Group>() on pf.GroupId equals p.Id
                    join f in dbContext.Set<FileResource>() on pf.FileId equals f.Id
                    where pf.Id == id
                    select new IGroupFilesRepository.GroupFile(pf, p, f);

        return query;
    }

    public IQueryable<IGroupFilesRepository.GroupFile> GetByGroupId(Guid groupId)
    {
        var dbContext = this.dbContext;
        var query = from pf in dbContext.Set<GroupFiles>()
                    join p in dbContext.Set<Group>() on pf.GroupId equals p.Id
                    join f in dbContext.Set<FileResource>() on pf.FileId equals f.Id
                    where p.Id == groupId
                    select new IGroupFilesRepository.GroupFile(pf, p, f);

        return query;
    }

    public Task Create(GroupFiles groupFiles, CancellationToken cancellationToken = default) => base.Add(groupFiles, cancellationToken);

    public new void Update(GroupFiles groupFiles) => base.Update(groupFiles);

    public new void Delete(GroupFiles groupFiles) => base.Delete(groupFiles);
}
