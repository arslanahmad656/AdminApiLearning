using Aro.Booking.Application.Repository;
using Aro.Booking.Domain.Entities;
using Aro.Common.Domain.Entities;
using Aro.Common.Infrastructure.Repository;
using Aro.Common.Infrastructure.Repository.Context;

namespace Aro.Booking.Infrastructure.Repository;

public class RoomFilesRepository(AroDbContext dbContext) : RepositoryBase<RoomFiles>(dbContext), IRoomFilesRepository
{
    private readonly AroDbContext dbContext = dbContext;
    public IQueryable<IRoomFilesRepository.RoomFile> GetById(Guid id)
    {
        var dbContext = this.dbContext;
        var query = from pf in dbContext.Set<RoomFiles>()
                    join p in dbContext.Set<Room>() on pf.RoomId equals p.Id
                    join f in dbContext.Set<FileResource>() on pf.FileId equals f.Id
                    where pf.Id == id
                    select new IRoomFilesRepository.RoomFile(pf, p, f);

        return query;
    }

    public IQueryable<IRoomFilesRepository.RoomFile> GetByRoomId(Guid roomId)
    {
        var dbContext = this.dbContext;
        var query = from pf in dbContext.Set<RoomFiles>()
                    join p in dbContext.Set<Room>() on pf.RoomId equals p.Id
                    join f in dbContext.Set<FileResource>() on pf.FileId equals f.Id
                    where p.Id == roomId
                    select new IRoomFilesRepository.RoomFile(pf, p, f);

        return query;
    }

    public Task Create(RoomFiles roomFiles, CancellationToken cancellationToken = default) => base.Add(roomFiles, cancellationToken);

    public new void Update(RoomFiles roomFiles) => base.Update(roomFiles);

    public new void Delete(RoomFiles roomFiles) => base.Delete(roomFiles);
}