using Aro.Common.Domain.Entities;

namespace Aro.Booking.Domain.Entities;

public class Policy : IEntity
{
    public Guid Id { get; set; }

    public Guid PropertyId { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public bool IsActive { get; set; }

    public int DisplayOrder { get; set; }

    public Property Property { get; set; }
}
