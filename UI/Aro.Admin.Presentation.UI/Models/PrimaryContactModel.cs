namespace Aro.Admin.Presentation.UI.Models;

public record PrimaryContactModel
{
    public Guid? Id { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? CountryCode { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsEditMode { get; set; }
}
