using Aro.Booking.Domain.Entities;
using Aro.Common.Domain.Entities;

namespace Aro.Booking.Application.Repository;

public interface IPropertyFilesRepository
{
    IQueryable<PropertyFile> GetById(Guid id);

    IQueryable<PropertyFile> GetByPropertyId(Guid propertyId);

    Task Create(PropertyFiles propertyFiles, CancellationToken cancellationToken = default);

    void Update(PropertyFiles propertyFiles);

    void Delete(PropertyFiles propertyFiles);

    public record PropertyFile(PropertyFiles Entity, Property Property, FileResource File);
}

