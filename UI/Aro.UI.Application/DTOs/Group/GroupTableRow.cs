namespace Aro.UI.Application.DTOs.Group;

public record GroupTableRow
{
    public Guid Id { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public Guid PrimaryContactId { get; set; }
    public string PrimaryContactName { get; set; } = string.Empty;
    public string PrimaryContactEmail { get; set; } = string.Empty;
    public string WebsiteURL { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string LogoUrl { get; set; } = string.Empty;
}

