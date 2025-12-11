using Aro.Booking.Domain.Entities;

namespace Aro.Booking.Application.Repository;

public interface IPolicyRepository
{
    IQueryable<Policy> GetAll();

    IQueryable<Policy> GetById(Guid id);

    Task Create(Policy policy, CancellationToken cancellationToken = default);

    void Update(Policy policy);

    void Delete(Policy policy);
}

