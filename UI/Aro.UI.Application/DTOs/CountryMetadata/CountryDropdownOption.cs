namespace Aro.UI.Application.DTOs.CountryMetadata;

public sealed record CountryDropdownOption
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string FlagUrl { get; init; } = string.Empty;
}
