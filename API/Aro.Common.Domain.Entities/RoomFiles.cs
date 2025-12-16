namespace Aro.Common.Domain.Entities;

public class RoomFiles : IEntity
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public Guid FileId { get; set; }
    public int OrderIndex { get; set; }
    public bool IsThumbnail { get; set; }
}
