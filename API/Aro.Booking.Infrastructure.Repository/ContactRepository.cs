using Aro.Booking.Application.Repository;
using Aro.Booking.Domain.Entities;
using Aro.Common.Infrastructure.Repository;
using Aro.Common.Infrastructure.Repository.Context;

namespace Aro.Booking.Infrastructure.Repository;

//public class ContactRepository(AroDbContext dbContext) : RepositoryBase<Contact>(dbContext), IContactRepository
//{
//    public IQueryable<Contact> GetById(Guid id) => FindByCondition(filter: c => c.Id == id);

//    public IQueryable<Contact> GetByUserId(Guid userId) => FindByCondition(filter: c => c.UserId == userId);

//    public Task Create(Contact contact, CancellationToken cancellationToken = default) => base.Add(contact, cancellationToken);

//    public new void Update(Contact contact) => base.Update(contact);

//    public new void Delete(Contact contact) => base.Delete(contact);
//}

