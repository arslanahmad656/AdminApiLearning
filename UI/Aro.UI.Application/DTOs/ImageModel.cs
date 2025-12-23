namespace Aro.UI.Application.DTOs;

public class ImageModel
{
    // For existing image
    public Guid Id { get; set; } = Guid.Empty;
    public string Url { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public string ContentBase64 { get; set; } = string.Empty;
    public int OrderIndex { get; set; } = 0;
    public bool IsThumbnail { get; set; } = false;
}
