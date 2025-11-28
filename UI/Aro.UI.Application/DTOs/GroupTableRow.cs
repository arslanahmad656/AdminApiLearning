namespace Aro.UI.Application.DTOs;
public record GroupTableRow
{
    public Guid Id { get; set; }
    public string GroupName { get; set; }
    public Guid PrimaryContactId { get; set; }
    public string PrimaryContactName { get; set; }
    public string PrimaryContactEmail { get; set; }
    public string WebsiteURL { get; set; }
    public bool IsActive { get; set; }
}

