using Aro.Booking.Domain.Entities;
using Aro.Common.Domain.Entities;

namespace Aro.Booking.Application.Repository;

public interface IGroupFilesRepository
{
    IQueryable<GroupFile> GetById(Guid id);

    IQueryable<GroupFile> GetByGroupId(Guid groupId);

    Task Create(GroupFiles groupFiles, CancellationToken cancellationToken = default);

    void Update(GroupFiles groupFiles);

    void Delete(GroupFiles groupFiles);

    public record GroupFile(GroupFiles Entity, Group Group, FileResource File);
}
