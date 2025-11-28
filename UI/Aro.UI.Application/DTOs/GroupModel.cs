namespace Aro.UI.Application.DTOs;

public record GroupModel
{
    public Guid? Id { get; set; }
    public string? GroupName { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public Guid? PrimaryContactId { get; set; }
}

