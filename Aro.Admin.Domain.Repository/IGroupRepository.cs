using Aro.Admin.Domain.Entities;

namespace Aro.Admin.Domain.Repository;

public interface IGroupRepository
{
    IQueryable<Group> GetById(Guid id);

    IQueryable<Group> GetAll();

    Task<bool> GroupsExist(CancellationToken cancellationToken = default);

    Task Create(Group group, CancellationToken cancellationToken = default);

    void Update(Group group);

    void Delete(Group group);
}
