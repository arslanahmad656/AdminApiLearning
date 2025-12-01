using Aro.Booking.Domain.Entities;

namespace Aro.Booking.Application.Repository;

public interface IContactRepository
{
    IQueryable<Contact> GetById(Guid id);

    IQueryable<Contact> GetByUserId(Guid userId);

    Task Create(Contact contact, CancellationToken cancellationToken = default);

    void Update(Contact contact);

    void Delete(Contact contact);
}

