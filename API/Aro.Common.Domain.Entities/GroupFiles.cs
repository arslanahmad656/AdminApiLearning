namespace Aro.Common.Domain.Entities;

public class GroupFiles : IEntity
{
    public Guid Id { get; set; }

    public Guid GroupId { get; set; }

    public Guid FileId { get; set; }
}