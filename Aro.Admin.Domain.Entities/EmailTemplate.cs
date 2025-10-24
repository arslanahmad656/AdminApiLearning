namespace Aro.Admin.Domain.Entities;

public class EmailTemplate : IEntity
{
    public Guid Id { get; set; }
    public string Identifier { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public bool IsHTML { get; set; }
}
