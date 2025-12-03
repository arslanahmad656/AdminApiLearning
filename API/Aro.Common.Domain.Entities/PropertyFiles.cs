namespace Aro.Common.Domain.Entities;

public class PropertyFiles : IEntity
{
    public Guid Id { get; set; }

    public Guid PropertyId { get; set; }

    public Guid FileId { get; set; }
}
